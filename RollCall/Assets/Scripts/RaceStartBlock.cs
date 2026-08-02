using UnityEngine;

public class RaceStartBlock : MonoBehaviour
{
    public StartGate startGate;
    [SerializeField] private Transform marbleStartPoint;

    public StartGate Gate => startGate;
    public Transform MarbleStartPoint => marbleStartPoint;
}