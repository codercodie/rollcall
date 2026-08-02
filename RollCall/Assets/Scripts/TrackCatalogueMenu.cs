using UnityEngine;

public class TrackCatalogueMenu : MonoBehaviour
{
    [SerializeField]
    private TrackCatalogue catalogue;

    [SerializeField]
    private TrackPieceMenuItem menuItemPrefab;

    [SerializeField]
    private Transform menuContent;

    [SerializeField]
    private BuildManager buildManager;


    private void Start()
    {
        BuildMenu();
    }

    public void BuildMenu()
    {
        ClearMenu();

        foreach (
            TrackPieceDefinition definition
            in catalogue.Pieces
        )
        {
            TrackPieceMenuItem menuItem = Instantiate(
                menuItemPrefab,
                menuContent
            );

            menuItem.Initialise(
                definition,
                buildManager
            );
        }
    }

    private void ClearMenu()
    {
        foreach (Transform child in menuContent)
        {
            Destroy(child.gameObject);
        }
    }
}