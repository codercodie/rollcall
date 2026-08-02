using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedTrackPiece :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    [SerializeField]
    private TrackPieceDefinition definition;

    public TrackPieceDefinition Definition => definition;

    public string PieceId =>
        definition != null ? definition.Id : "";

    private BuildManager buildManager;
    private Vector3 positionBeforeDrag;
    private bool isDragging;

    private void Awake()
    {
        buildManager = FindAnyObjectByType<BuildManager>();
    }

    public void Initialise(
        TrackPieceDefinition newDefinition
    )
    {
        definition = newDefinition;

        if (buildManager == null)
        {
            buildManager = FindAnyObjectByType<BuildManager>();
        }
    }

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (buildManager == null ||
            !buildManager.CanBuild)
        {
            return;
        }

        Debug.Log($"{name} selected");
    }

    public void OnBeginDrag(
        PointerEventData eventData
    )
    {
        if (buildManager == null ||
            !buildManager.CanBuild)
        {
            return;
        }

        isDragging = true;
        positionBeforeDrag = transform.position;
    }

    public void OnDrag(
        PointerEventData eventData
    )
    {
        if (!isDragging)
        {
            return;
        }

        buildManager.MovePlacedPiece(
            transform,
            eventData.position
        );
    }

    public void OnEndDrag(
        PointerEventData eventData
    )
    {
        if (!isDragging)
        {
            return;
        }

        isDragging = false;

        // Return it to its previous location if released
        // on top of the catalogue menu.
        if (buildManager.IsOverTrackMenu(
            eventData.position
        ))
        {
            transform.position = positionBeforeDrag;
        }
    }
}