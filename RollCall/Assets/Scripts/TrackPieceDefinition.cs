using UnityEngine;

[CreateAssetMenu(
    fileName = "New Track Piece",
    menuName = "Roll Call/Track Piece"
)]
public class TrackPieceDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite menuIcon;
    [SerializeField] private GameObject prefab;

    [Header("Placement limits")]
    [Tooltip("Set to 0 for unlimited")]
    [Min(0)]
    [SerializeField] private int maximumPerCourse;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite MenuIcon => menuIcon;
    public GameObject Prefab => prefab;
    public int MaximumPerCourse => maximumPerCourse;
}