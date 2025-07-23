using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Gameplay;

public class ScoreBoardUI : BaseWindowUI
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

       /* this.close.SetActive(sceneInsex == 0);
        this.quit.SetActive(sceneInsex == 1);
        this.restart.SetActive(sceneInsex == 1);*/

        if (ManagersSOHolder.SOScoreManager.LastScore == null)
        {
            this.emptyInfo.SetActive(true);
            this.highScore.SetActive(false);
            this.lastScore.SetActive(false);
            return;
        }

        this.emptyInfo.SetActive(false);
        this.highScore.SetActive(true);
        this.lastScore.SetActive(true);

        this.highDaysScore.SetText($"for <size=200%>{ManagersSOHolder.SOScoreManager.HighScore.Score}</size> days");
        this.lastDaysScore.SetText($"for <size=200%>{ManagersSOHolder.SOScoreManager.LastScore.Score}</size> days");

        foreach (var idol in this.highIdolsRecords)
        {
            idol.gameObject.SetActive(false);
        }
        foreach (var idol in this.lastIdolsRecords)
        {
            idol.gameObject.SetActive(false);
        }

        var highBonksHeaderText = ManagersSOHolder.SOScoreManager.HighScore.Bonks.Count == 0 ? "No bonks dealt. Whaaaa?" : "You have kindly dealt bonks:";
        var lastBonksHeaderText = ManagersSOHolder.SOScoreManager.LastScore.Bonks.Count == 0 ? "No bonks dealt. Whaaaa?" : "You have kindly dealt bonks:";
        this.highBonksHeader.SetText(highBonksHeaderText);
        this.lastBonksHeader.SetText(lastBonksHeaderText);

        for (var i = 0; i < ManagersSOHolder.SOScoreManager.HighScore.Bonks.Count; i++)
            this.SetupBonks(this.highIdolsRecords, i, ManagersSOHolder.SOScoreManager.HighScore);
        for (var i = 0; i < ManagersSOHolder.SOScoreManager.LastScore.Bonks.Count; i++)
            this.SetupBonks(this.lastIdolsRecords, i, ManagersSOHolder.SOScoreManager.LastScore);
    }

    void SetupBonks(TextMeshProUGUI[] descriptionScoreRecords, int index, ScoreElement scoreManager)
    {
        descriptionScoreRecords[index].SetText($"{scoreManager.Bonks[index].Name} -> <size=200%>{scoreManager.Bonks[index].Times}</size> times");
        descriptionScoreRecords[index].gameObject.SetActive(true);
    }
}
