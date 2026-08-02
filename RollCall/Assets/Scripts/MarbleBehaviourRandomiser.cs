using UnityEngine;

public class MarbleBehaviourRandomiser : MonoBehaviour
{
    [Header("Random ranges")]
    [SerializeField] private Vector2 gravityRange = new(1.35f, 1.5f);
    [SerializeField] private Vector2 frictionRange = new(0.05f, 0.40f);
    [SerializeField] private Vector2 bouncinessRange = new(0f, 1f);
    [SerializeField] private Vector2 dampingRange = new(-0.5f, 0.5f);
    [SerializeField] private Vector2 startingSpinRange = new(-20f, 35f);

    private Rigidbody2D rb;
    private Collider2D marbleCollider;
    private PhysicsMaterial2D runtimeMaterial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        marbleCollider = GetComponent<Collider2D>();

        runtimeMaterial = new PhysicsMaterial2D("Runtime Marble Material");
        marbleCollider.sharedMaterial = runtimeMaterial;
    }

    public void RandomizeForRace()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.gravityScale = Random.Range(
            gravityRange.x,
            gravityRange.y
        );

        rb.linearDamping = Random.Range(
            dampingRange.x,
            dampingRange.y
        );

        runtimeMaterial.friction = Random.Range(
            frictionRange.x,
            frictionRange.y
        );

        runtimeMaterial.bounciness = Random.Range(
            bouncinessRange.x,
            bouncinessRange.y
        );

        rb.angularVelocity = Random.Range(
            startingSpinRange.x,
            startingSpinRange.y
        );

        float sidewaysSpeed = Random.Range(-0.25f, 0.25f);

        rb.linearVelocity = new Vector2(
            sidewaysSpeed,
            rb.linearVelocity.y
        );

        rb.angularVelocity = Random.Range(
            startingSpinRange.x,
            startingSpinRange.y
        );
    }
}