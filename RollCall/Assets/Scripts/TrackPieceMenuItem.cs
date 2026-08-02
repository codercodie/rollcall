using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackPieceMenuItem :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    private TrackPieceDefinition definition;
    private BuildManager buildManager;

    public void Initialise(
        TrackPieceDefinition newDefinition,
        BuildManager newBuildManager
    )
    {
        definition = newDefinition;
        buildManager = newBuildManager;

        icon.sprite = definition.MenuIcon;
        nameText.text = definition.DisplayName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        buildManager.BeginPlacement(
            definition,
            eventData.position
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        buildManager.MovePreview(
            eventData.position
        );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        buildManager.FinishPlacement(
            eventData.position
        );
    }
}