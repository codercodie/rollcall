using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MarbleCollisionRandomiser : MonoBehaviour
{
    [SerializeField] private LayerMask trackLayers;

    [Header("Collision variation")]
    [SerializeField]
    private Vector2 horizontalImpulseRange =
        new(-0.20f, 0.50f);

    [SerializeField]
    private Vector2 verticalImpulseRange =
        new(0f, 0.25f);

    [SerializeField]
    private Vector2 spinChangeRange =
        new(-25f, 35f);

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        bool isTrackLayer =
            (trackLayers.value & (1 << collisionLayer)) != 0;

        if (!isTrackLayer)
        {
            return;
        }

        Vector2 randomImpulse = new Vector2(
            Random.Range(
                horizontalImpulseRange.x,
                horizontalImpulseRange.y
            ),
            Random.Range(
                verticalImpulseRange.x,
                verticalImpulseRange.y
            )
        );

        rb.AddForce(
            randomImpulse,
            ForceMode2D.Impulse
        );

        rb.angularVelocity += Random.Range(
            spinChangeRange.x,
            spinChangeRange.y
        );
    }
}