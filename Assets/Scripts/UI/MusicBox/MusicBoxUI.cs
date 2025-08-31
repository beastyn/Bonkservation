using Gameplay;
using Gameplay.MusicBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI.MusicBox
{
    // Manages the Music Box UI, including loading and displaying music sheets.
    // This class is responsible for initializing the music box interface and handling user interactions.
    // It interacts with other components such as AudioManager for sound playback and GameManager for game state management.
    // Future enhancements may include adding features like saving/loading user-created music sheets and integrating with a scoring system.
    public class MusicBoxUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] TextAsset jsonSheet;
        [SerializeField] SONotesLibrary notesLibrary;

        [Header("Cylinder & Comb Containers")]
        [SerializeField] RectTransform cylinderParent;//where all pins and beats numbers are
        [SerializeField] ScrollRect cylinderScrollRect;
        [SerializeField] RectTransform scrollablePinsContent; //where pins reside
        [SerializeField] RectTransform beatsParent; // where beats number labels
        [SerializeField] RectTransform combParent; // parent for comb teeth UI

        [Header("Prefabs")]
        [SerializeField] Button pinCellPrefab; // a Button with Image (off/on highlight)
        [SerializeField] TextMeshProUGUI beatLinePrefab; // a thin Image (vertical/horizontal line)
        [SerializeField] Button combToothPrefab; // a Button representing a tooth (with Image/Line)

        [Header("Design")]
        [SerializeField] Color inactivePinColor;
        [SerializeField] Color activePinColor;

        [Header("Controls")]
        public Button playButton; // plays player's pins
        public Button replayButton; // plays target/preview pins
        public Button submitButton; // validate player vs target
        public Text titleText;
        public Text resultText;


        [Header("Playback Visuals")]
        public Image playhead; // optional vertical/horizontal bar
        public float playheadThickness = 4f;


        // Internal
        MusicBoxSheet musicSheet;
        bool[,] playerGrid; // rows x beats
        bool[,] targetGrid; // rows x beats

        List<List<Button>> cells = new(); // [row][beat]
        List<Button> teeth = new(); // [row]

        AudioSource audioSource; // one-off source; for polyphony, pool sources

        void Start()
        {
            this.audioSource = this.gameObject.AddComponent<AudioSource>();
            LoadSheet();
            BuildUI();
           Cursor.visible = true;
            //WireButtons();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void LoadSheet()
        {
            if (jsonSheet == null) { Debug.LogError("MusicBoxUI: No JSON assigned"); return; }
            this.musicSheet = JsonUtility.FromJson<MusicBoxSheet>(jsonSheet.text);
            if (this.musicSheet == null) { Debug.LogError("MusicBoxUI: Bad JSON"); return; }


            this.playerGrid = new bool[this.musicSheet.teeth.Length, this.musicSheet.beats];
            this.targetGrid = new bool[this.musicSheet.teeth.Length, this.musicSheet.beats];


            // Build target grid from targetPins
            if (this.musicSheet.targetPins != null)
            {
                var rowIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int toothIndex = 0; toothIndex < this.musicSheet.teeth.Length; toothIndex++)
                    rowIndex[this.musicSheet.teeth[toothIndex].id] = toothIndex;

                foreach (var targetPin in this.musicSheet.targetPins)
                {
                    if (!rowIndex.TryGetValue(targetPin.tooth, out int toothIndex)) continue;
                    foreach (var beat in targetPin.beats)
                    {
                        int clampedBeat = Mathf.Clamp(beat - 1, 0, this.musicSheet.beats - 1);
                        this.targetGrid[toothIndex, clampedBeat] = true;
                    }
                }
            }
        }

        void BuildUI()
        {
            if (this.titleText) titleText.text = this.musicSheet.title;

            for (int toothIndex = 0; toothIndex < this.musicSheet.teeth.Length; toothIndex++)
            {
                var pinsList = new List<Button>(this.musicSheet.beats);
                for (int beatIndex = 0; beatIndex < this.musicSheet.beats; beatIndex++)
                {
                    var cell = Instantiate(this.pinCellPrefab, this.scrollablePinsContent);
                    cell.name = $"Cell_{this.musicSheet.teeth[toothIndex].id}_{beatIndex + 1}";

                    var localToothIndex = toothIndex;
                    var localBeatIndex = beatIndex;

                    this.SetCellVisual(cell, false);
                    cell.onClick.AddListener(() => ToggleCell(localToothIndex, localBeatIndex, cell));
                    pinsList.Add(cell);

                    var beatLine = Instantiate(this.beatLinePrefab, this.beatsParent);
                    beatLine.text = (this.musicSheet.beats - localBeatIndex).ToString();
                }
                this.cells.Add(pinsList);
            }

            var cylinderCellGrid = this.scrollablePinsContent.gameObject.GetComponent<GridLayoutGroup>();
            var cylinderWidth = this.musicSheet.teeth.Length * (cylinderCellGrid.cellSize.x + cylinderCellGrid.spacing.x);
            var cylinderContentHeight =this.musicSheet.beats *(cylinderCellGrid.cellSize.y + cylinderCellGrid.spacing.y);
            var pinsConentRect = this.scrollablePinsContent.parent.GetComponent<RectTransform>();

            this.cylinderParent.sizeDelta = new Vector2(cylinderWidth + cylinderCellGrid.cellSize.x + cylinderCellGrid.spacing.x, this.cylinderParent.sizeDelta.y);
            pinsConentRect.sizeDelta = new Vector2(cylinderWidth, cylinderContentHeight);
            this.scrollablePinsContent.sizeDelta = new Vector2(cylinderWidth, cylinderContentHeight);


            Canvas.ForceUpdateCanvases();
            this.cylinderScrollRect.verticalNormalizedPosition = 0f;
            // Create comb teeth (bottom). Length visualized by scaling width (or height if vertical)

            var maxToothHeight = 0f;
            for (int i = 0; i < this.musicSheet.teeth.Length; i++)
            {
                var tooth = Instantiate(this.combToothPrefab, this.combParent);
                tooth.name = $"Tooth_{  this.musicSheet.teeth[i].id}";
                var txt = tooth.GetComponentInChildren<Text>();
                if (txt) txt.text = this.musicSheet.teeth[i].display;
                // scale by combLength
                float k = Mathf.Max(0.2f, this.musicSheet.teeth[i].combLength);
                var rect = tooth.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * k);

                int toothIndex = i;
                //tooth.onClick.AddListener(() => AuditionRow(rr));
                this.teeth.Add(tooth);
                maxToothHeight = Mathf.Max(maxToothHeight, rect.sizeDelta.y);
            }

            this.combParent.sizeDelta = new Vector2(this.combParent.sizeDelta.x, maxToothHeight);
            // Optional: hide playhead initially
            if (playhead) playhead.enabled = false;
        }

        void SetCellVisual(Button cell, bool active)
        {
            var img = cell.GetComponent<Image>();
            if (img) img.color = active ? this.activePinColor : this.inactivePinColor;
        }

        void ToggleCell(int toothIndex, int beatIndex, Button cell)
        {
            this.playerGrid[toothIndex, beatIndex] = !this.playerGrid[toothIndex, beatIndex];
            SetCellVisual(cell, this.playerGrid[toothIndex, beatIndex]);
        }

        void WireButtons()
        {
            if (this.playButton) playButton.onClick.AddListener(() => StartCoroutine(PlayRoutine(this.playerGrid)));
            /*if (replayButton)
            {
                // prefer previewPins if provided, else target
                if (this.musicSheet.previewPins != null && this.musicSheet.previewPins.Length > 0)
                {
                    bool[,] preview = BuildGridFromPins(_sheet.previewPins);
                    replayButton.onClick.AddListener(() => StartCoroutine(PlayRoutine(preview)));
                }
                else replayButton.onClick.AddListener(() => StartCoroutine(PlayRoutine(_targetGrid)));
            }*/
            if (submitButton) submitButton.onClick.AddListener(ValidateSubmission);
        }

        IEnumerator PlayRoutine(bool[,] grid)
        {
            if (this.musicSheet == null) yield break;
            float beatDur = 60f / Mathf.Max(1, this.musicSheet.bpm);
            if (playhead) playhead.enabled = true;

            for (int beatIndex = 0; beatIndex < this.musicSheet.beats; beatIndex++)
            {
                // move playhead to column c
          /*      if (playhead)
                {
                    // assumes playhead is a child aligned over the grid; adjust anchoredPosition.x as needed
                    var rt = playhead.rectTransform;
                    var cellWidth = (gridParent as RectTransform).rect.width / Mathf.Max(1, _sheet.beats);
                    rt.sizeDelta = new Vector2(playheadThickness, rt.sizeDelta.y);
                    rt.anchoredPosition = new Vector2(c * cellWidth + cellWidth * 0.5f, rt.anchoredPosition.y);
                }*/


                // play all notes in this column
                for (int toothIndex = 0; toothIndex < this.musicSheet.teeth.Length; toothIndex++)
                {
                    if (grid[toothIndex, beatIndex])
                    {
                        var clip = this.notesLibrary ? this.notesLibrary.Get(this.musicSheet.teeth[toothIndex].audioKey) : null;
                        if (clip != null) this.audioSource.PlayOneShot(clip);
                       // StartCoroutine(FlashCell(toothIndex, beatIndex));
                      //  StartCoroutine(FlashToothtoothIndexr));
                    }
                }
                yield return new WaitForSeconds(beatDur);
            }


            if (playhead) playhead.enabled = false;
        }

        void OnDestroy()
        {
            foreach (Transform transform in this.cylinderParent)
                Destroy(transform.gameObject);
            foreach (Transform transform in combParent)
                Destroy(transform.gameObject);
            this.cells.Clear();
            this.teeth.Clear();
        }
    }
}