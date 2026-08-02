using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [SerializeField] private Collider2D collider;
    [SerializeField] private Gameplay gameplay;

    public void Start()
    {
    }


    public void OnTrigger(Collider other)
    {
        if (other.gameObject.name == "Marble")
        {
            gameplay.AddLoser(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
