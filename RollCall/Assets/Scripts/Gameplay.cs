using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MarbleEntry
{
    [Header("Physical marble")]
    public GameObject marble;

    [Header("Results UI")]
    public GameObject marbleIcon;
    public GameObject winnerIcon;
    public GameObject loserIcon;
}

public class Gameplay : MonoBehaviour
{
    [SerializeField] private List<MarbleEntry> marbleEntries;
    [SerializeField] public GameObject goButton;
    public TMPro.TextMeshProUGUI goButtonText;
    private RaceStartBlock startBlock;
    private RaceFinishBlock finishBlock;
    private FinishLine finishLine;

    private readonly List<GameObject> winners = new();
    private readonly List<GameObject> losers = new();

    [SerializeField]
    private TMPro.TextMeshProUGUI winnerAnnouncement;

    public enum GameplayState
    {
        Building,
        Ready,
        Racing,
        Finished
    }

    public GameplayState CurrentState { get; private set; }
    private void Start()
    {
        EnterBuildMode();
    }

    public void EnterBuildMode()
    {
        CurrentState = GameplayState.Building;

        winners.Clear();
        losers.Clear();

        foreach (MarbleEntry entry in marbleEntries)
        {
            Rigidbody2D rb =
                entry.marble.GetComponent<Rigidbody2D>();

            rb.simulated = false;
            entry.marble.SetActive(false);
        }

        ResetResults();

        goButton.SetActive(false);
    }

    public bool PrepareRace()
    {
        startBlock =
            FindAnyObjectByType<RaceStartBlock>();

        finishBlock =
            FindAnyObjectByType<RaceFinishBlock>();

        if (startBlock == null)
        {
            Debug.LogWarning(
                "Add a start block before racing"
            );

            return false;
        }

        if (finishBlock == null)
        {
            Debug.LogWarning(
                "Add a finish block before racing"
            );

            return false;
        }

        if (startBlock.Gate == null ||
            startBlock.MarbleStartPoint == null)
        {
            Debug.LogWarning(
                "The start block is not configured correctly"
            );

            return false;
        }

        if (finishBlock.FinishLine == null)
        {
            Debug.LogWarning(
                "The finish block is not configured correctly"
            );

            return false;
        }

        finishLine = finishBlock.FinishLine;

        finishLine.Initialise(
            this,
            winnerAnnouncement
        );

        CurrentState = GameplayState.Ready;

        ResetRace();

        goButton.SetActive(true);
        goButtonText.text = "GO";

        return true;
    }
    public void ResetRace()
    {
        if (startBlock == null || finishLine == null)
        {
            Debug.LogWarning(
                "Course must contain a Start and Finish!"
            );

            EnterBuildMode();
            return;
        }

        winners.Clear();
        losers.Clear();

        startBlock.Gate.CloseImmediately();
        finishLine.ResetFinish();

        LoadMarbles();
        ResetResults();

        CurrentState = GameplayState.Ready;
    }

    private void SetVisible(GameObject icon, bool visible)
    {
        CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = icon.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        icon.SetActive(true);
    }

    private void LoadMarbles()
    {
        Transform startPoint =
            startBlock.MarbleStartPoint;

        foreach (MarbleEntry entry in marbleEntries)
        {
            GameObject marble = entry.marble;
            Rigidbody2D rb =
                marble.GetComponent<Rigidbody2D>();

            marble.SetActive(true);

            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            marble.transform.SetPositionAndRotation(
                startPoint.position,
                Quaternion.identity
            );
        }
    }
    private void ResetResults()
    {
        foreach (MarbleEntry entry in marbleEntries)
        {
            SetVisible(entry.winnerIcon, false);
            SetVisible(entry.loserIcon, false);
        }
    }

    public void StartRace()
    {
        if (CurrentState != GameplayState.Ready)
        {
            Debug.LogWarning(
                "The course is not ready to race"
            );

            return;
        }

        CurrentState = GameplayState.Racing;

        foreach (MarbleEntry entry in marbleEntries)
        {
            GameObject marble = entry.marble;
            Rigidbody2D rb =
                marble.GetComponent<Rigidbody2D>();

            rb.simulated = true;

            MarbleBehaviourRandomiser randomiser =
                marble.GetComponent<MarbleBehaviourRandomiser>();

            if (randomiser != null)
            {
                randomiser.RandomizeForRace();
            }
        }

        startBlock.Gate.Open();
    }

    public void AddWinner(GameObject marble)
    {
        MarbleEntry entry = FindEntry(marble);

        if (entry == null)
        {
            Debug.LogWarning(
                $"No MarbleEntry found for {marble.name}"
            );

            return;
        }

        if (!winners.Contains(marble))
        {
            winners.Add(marble);
            Debug.Log($"{marble.name} added as a winner");
        }

        entry.winnerIcon.SetActive(true);
    }

    public void AddLoser(GameObject marble)
    {
        MarbleEntry entry = FindEntry(marble);

        if (entry == null)
        {
            Debug.LogWarning(
                $"No MarbleEntry found for {marble.name}"
            );

            return;
        }

        if (!losers.Contains(marble))
        {
            losers.Add(marble);
            Debug.Log($"{marble.name} added as a loser");
        }

        entry.loserIcon.SetActive(true);
    }

    public void SetRaceWinner(GameObject winningMarble)
    {
        foreach (MarbleEntry entry in marbleEntries)
        {
            bool isWinner = entry.marble == winningMarble;

            SetVisible(entry.winnerIcon, isWinner);
            SetVisible(entry.loserIcon, !isWinner);
        }

        Debug.Log($"{winningMarble.name} set as the winner");
    }

    private MarbleEntry FindEntry(GameObject marble)
    {
        return marbleEntries.Find(
            entry => entry.marble == marble
        );
    }
}