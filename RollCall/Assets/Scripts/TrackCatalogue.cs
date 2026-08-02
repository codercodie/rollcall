using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Track Piece Catalogue",
    menuName = "Roll Call/Track Piece Catalogue"
)]
public class TrackCatalogue : ScriptableObject
{
    [SerializeField]
    private List<TrackPieceDefinition> pieces = new();

    public IReadOnlyList<TrackPieceDefinition> Pieces =>
        pieces;

    public TrackPieceDefinition FindById(string id)
    {
        return pieces.Find(piece => piece.Id == id);
    }
}