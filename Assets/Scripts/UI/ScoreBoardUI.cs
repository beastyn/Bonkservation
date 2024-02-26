using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Managers.UI;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] GameObject emptyInfo;
    [SerializeField] GameObject highScore;
    [SerializeField] GameObject lastScore;
    [SerializeField] GameObject close;
    [SerializeField] GameObject quit;
    [SerializeField] GameObject restart;

    [Header("High Score")]
    [SerializeField] TextMeshProUGUI highDaysScore;
    [SerializeField] TextMeshProUGUI highBonksHeader;
    [SerializeField] TextMeshProUGUI[] highIdolsRecords;

    [Header("Last Score")]
    [SerializeField] TextMeshProUGUI lastDaysScore;
    [SerializeField] TextMeshProUGUI lastBonksHeader;
    [SerializeField] TextMeshProUGUI[] lastIdolsRecords;

    void OnEnable()
    {
        var currentscene = SceneManager.GetActiveScene();

        var sceneInsex = currentscene.buildIndex;

        this.close.SetActive(sceneInsex == 0);
        this.quit.SetActive(sceneInsex == 1);
        this.restart.SetActive(sceneInsex == 1);

        if (ManagersSOHolder.ScoreManagerSO.LastScore == null)
        {
            this.emptyInfo.SetActive(true);
            this.highScore.SetActive(false);
            this.lastScore.SetActive(false);
            return;
        }

        this.emptyInfo.SetActive(false);
        this.highScore.SetActive(true);
        this.lastScore.SetActive(true);

        this.highDaysScore.SetText($"for <size=200%>{ManagersSOHolder.ScoreManagerSO.LastScore.Score}</size> days");
        this.lastDaysScore.SetText($"for <size=200%>{ManagersSOHolder.ScoreManagerSO.LastScore.Score}</size> days");

        foreach (var idol in this.highIdolsRecords)
        {
            idol.gameObject.SetActive(false);
        }
        foreach (var idol in this.lastIdolsRecords)
        {
            idol.gameObject.SetActive(false);
        }

        var highBonksHeaderText = ManagersSOHolder.ScoreManagerSO.HighScore.Bonks.Count == 0 ? "No bonks dealt. Whaaaa?" : "You have kindly dealt bonks:";
        var lastBonksHeaderText = ManagersSOHolder.ScoreManagerSO.LastScore.Bonks.Count == 0 ? "No bonks dealt. Whaaaa?" : "You have kindly dealt bonks:";
        this.highBonksHeader.SetText(highBonksHeaderText);
        this.lastBonksHeader.SetText(lastBonksHeaderText);

        for (var i = 0; i < ManagersSOHolder.ScoreManagerSO.HighScore.Bonks.Count; i++)
        {
            this.highIdolsRecords[i].SetText($"{ManagersSOHolder.ScoreManagerSO.HighScore.Bonks[i].Name} -> <size=200%>{ManagersSOHolder.ScoreManagerSO.LastScore.Bonks[i].Times}</size> times");
            this.highIdolsRecords[i].gameObject.SetActive(true);
        }
        for (var i = 0; i < ManagersSOHolder.ScoreManagerSO.LastScore.Bonks.Count; i++)
        {
            this.lastIdolsRecords[i].SetText($"{ManagersSOHolder.ScoreManagerSO.LastScore.Bonks[i].Name} -> <size=200%>{ManagersSOHolder.ScoreManagerSO.LastScore.Bonks[i].Times}</size> times");
            this.lastIdolsRecords[i].gameObject.SetActive(true);
        }
    }

    public void RestartObservation() => SceneManager.LoadScene(1);
    public void LoadTitleScreen() => SceneManager.LoadScene(0);

    public void Close() => UIManager.CloseUI(this.gameObject);
}
