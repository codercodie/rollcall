using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Transform trackContainer;

    [Header("Menu")]
    [SerializeField] private RectTransform trackMenu;
    [SerializeField] private Canvas canvas;

    [Header("Placement")]
    [SerializeField] private bool canBuild = true;
    [SerializeField] private bool snapToGrid = true;

    [Min(0.01f)]
    [SerializeField] private float gridSize = 0.25f;

    [Header("Preview")]
    [Range(0f, 1f)]
    [SerializeField] private float previewOpacity = 0.6f;

    public bool CanBuild => canBuild;

    private GameObject previewObject;
    private Collider2D[] previewColliders;
    private SpriteRenderer[] previewRenderers;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Gameplay gameplay;

    private readonly List<Color> originalColours = new();

    public void Start()
    {
        menuPanel.SetActive(false);
    }

    public void SetBuildMode(bool enabled)
    {
        canBuild = enabled;

        if (!enabled)
        {
            CancelPlacement();
        }
    }


    public void BeginPlacement(
    TrackPieceDefinition definition,
    Vector2 screenPosition
)
    {
        if (!canBuild || definition == null)
        {
            return;
        }

        if (!CanPlacePiece(definition))
        {
            Debug.LogWarning(
                $"You can only place " +
                $"{definition.MaximumPerCourse} " +
                $"{definition.DisplayName}"
            );

            return;
        }

        if (definition.Prefab == null)
        {
            Debug.LogWarning(
                $"{definition.name} has no prefab assigned"
            );

            return;
        }

        CancelPlacement();

        previewObject = Instantiate(
            definition.Prefab,
            trackContainer
        );

        PlacedTrackPiece placedPiece =
            previewObject.GetComponent<PlacedTrackPiece>();

        if (placedPiece == null)
        {
            placedPiece =
                previewObject.AddComponent<PlacedTrackPiece>();
        }

        placedPiece.Initialise(definition);

        PreparePreview();
        MovePreview(screenPosition);
    }

    private void PreparePreview()
    {
        previewColliders =
            previewObject.GetComponentsInChildren<Collider2D>();

        previewRenderers =
            previewObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (Collider2D pieceCollider in previewColliders)
        {
            pieceCollider.enabled = false;
        }

        originalColours.Clear();

        foreach (SpriteRenderer renderer in previewRenderers)
        {
            originalColours.Add(renderer.color);

            Color previewColour = renderer.color;
            previewColour.a *= previewOpacity;

            renderer.color = previewColour;
        }
    }

    public void MovePreview(Vector2 screenPosition)
    {
        if (!canBuild || previewObject == null)
        {
            return;
        }

        previewObject.transform.position =
            GetWorldPosition(screenPosition);
    }

    public void FinishPlacement(Vector2 screenPosition)
    {
        if (previewObject == null)
        {
            return;
        }

        if (IsPointerOverTrackMenu(screenPosition))
        {
            CancelPlacement();
            return;
        }

        EnablePlacedPiece();

        previewObject = null;
        previewColliders = null;
        previewRenderers = null;

        originalColours.Clear();
    }

    private void EnablePlacedPiece()
    {
        foreach (Collider2D pieceCollider in previewColliders)
        {
            if (pieceCollider != null)
            {
                pieceCollider.enabled = true;
            }
        }

        for (int i = 0; i < previewRenderers.Length; i++)
        {
            if (previewRenderers[i] != null &&
                i < originalColours.Count)
            {
                previewRenderers[i].color =
                    originalColours[i];
            }
        }
    }

    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;
        previewColliders = null;
        previewRenderers = null;

        originalColours.Clear();
    }

    private bool IsPointerOverTrackMenu(
        Vector2 screenPosition
    )
    {
        if (trackMenu == null)
        {
            return false;
        }

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            trackMenu,
            screenPosition,
            uiCamera
        );
    }

    private float SnapToGrid(float value)
    {
        return Mathf.Round(value / gridSize) * gridSize;
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (gameplay == null)
        {
            gameplay = FindAnyObjectByType<Gameplay>();
        }
        bool courseIsReady = gameplay.PrepareRace();

        if (!courseIsReady)
        {
            Debug.LogWarning(
                "Build menu remains open because the course is incomplete"
            );

            return;
        }

        menuPanel.SetActive(false);
        SetBuildMode(false);
    }

    public bool CanPlacePiece(
    TrackPieceDefinition definition
)
    {
        if (definition == null)
        {
            return false;
        }

        if (definition.MaximumPerCourse == 0)
        {
            return true;
        }

        int currentCount = 0;

        PlacedTrackPiece[] placedPieces =
            trackContainer.GetComponentsInChildren<
                PlacedTrackPiece
            >(true);

        foreach (PlacedTrackPiece piece in placedPieces)
        {
            if (piece.PieceId == definition.Id)
            {
                currentCount++;
            }
        }

        return currentCount <
               definition.MaximumPerCourse;
    }

    public Vector3 GetWorldPosition(
    Vector2 screenPosition
)
    {
        Vector3 worldPosition =
            gameCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    Mathf.Abs(gameCamera.transform.position.z)
                )
            );

        worldPosition.z = 0f;

        if (snapToGrid)
        {
            worldPosition.x = SnapToGrid(
                worldPosition.x
            );

            worldPosition.y = SnapToGrid(
                worldPosition.y
            );
        }

        return worldPosition;
    }

    public void MovePlacedPiece(
        Transform piece,
        Vector2 screenPosition
    )
    {
        if (!CanBuild || piece == null)
        {
            return;
        }

        piece.position = GetWorldPosition(screenPosition);
    }

    public bool IsOverTrackMenu(Vector2 screenPosition)
    {
        return IsPointerOverTrackMenu(screenPosition);
    }
}