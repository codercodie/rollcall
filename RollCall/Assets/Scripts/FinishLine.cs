using UnityEngine;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerAnnouncement;
    [SerializeField] private Gameplay gameplay;

    private bool winnerRegistered;

    public void Initialise(
        Gameplay gameplayManager,
        TextMeshProUGUI announcement
    )
    {
        gameplay = gameplayManager;
        winnerAnnouncement = announcement;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (winnerRegistered)
        {
            return;
        }

        Rigidbody2D marbleRigidbody = other.attachedRigidbody;

        if (marbleRigidbody == null)
        {
            return;
        }

        GameObject marble = marbleRigidbody.gameObject;

        if (!marble.CompareTag("Marble"))
        {
            return;
        }

        winnerRegistered = true;

        gameplay.SetRaceWinner(marble);

        winnerAnnouncement.color = GetColour(marble.name);
        winnerAnnouncement.text = $"{marble.name} WINS!";
        winnerAnnouncement.gameObject.SetActive(true);

        Debug.Log($"{marble.name} wins!");

        gameplay.goButton.SetActive(true);
        gameplay.goButtonText.text = "Reset";
    }

    public void ResetFinish()
    {
        winnerRegistered = false;

        if (winnerAnnouncement == null)
        {
            return;
        }

        winnerAnnouncement.text = "";
        winnerAnnouncement.gameObject.SetActive(false);
    }

    private Color GetColour(string marbleName)
    {
        string hexColour = marbleName switch
        {
            "WHITE" => "#FFFFFF",
            "RED" => "#FF0000",
            "BLUE" => "#00ABFF",
            "GREEN" => "#058B00",
            "YELLOW" => "#FFF200",
            "PURPLE" => "#722DFA",
            "PINK" => "#EB4EED",
            "ORANGE" => "#FF7F00",
            "BLACK" => "#0E0C0C",
            _ => "#FFFFFF"
        };

        if (ColorUtility.TryParseHtmlString(
            hexColour,
            out Color colour
        ))
        {
            return colour;
        }

        return Color.white;
    }
}