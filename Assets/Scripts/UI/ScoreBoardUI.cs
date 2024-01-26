using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI daysScore;
    [SerializeField] TextMeshProUGUI bonksHeader;
    [SerializeField] TextMeshProUGUI[] idolsRecords;

    void OnEnable()
    {
        if (ManagersSOHolder.ScoreManagerSO.LastScore == null) return;

        this.daysScore.SetText($"<size=200%>{ManagersSOHolder.ScoreManagerSO.LastScore.Score}</size> days");

        foreach (var idol in this.idolsRecords)
            idol.gameObject.SetActive(false);

        var bonksHeaderText = ManagersSOHolder.ScoreManagerSO.LastScore.Bonks.Count == 0 ? "No bonks dealt. Whaaaa?" : "You have kindly dealt bonks:";
        this.bonksHeader.SetText(bonksHeaderText);

        for (var i = 0; i < ManagersSOHolder.ScoreManagerSO.LastScore.Bonks.Count; i++)
        {
            this.idolsRecords[i].SetText($"{ManagersSOHolder.ScoreManagerSO.LastScore.Bonks[i].Name}: <size=200%>{ManagersSOHolder.ScoreManagerSO.LastScore.Bonks[i].Times}</size> times");
            idolsRecords[i].gameObject.SetActive(true);
        }
    }

    public void RestartObservation() => SceneManager.LoadScene(0);
}
