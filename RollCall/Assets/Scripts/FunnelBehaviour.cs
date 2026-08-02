using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelBehaviour : MonoBehaviour
{
    [SerializeField] private Collider2D holdingCollider;

    [Header("Time spent in funnel")]
    [SerializeField] private float minimumDuration = 2f;
    [SerializeField] private float maximumDuration = 6f;

    private readonly Dictionary<Collider2D, Coroutine> marbleTimers = new();

    private readonly HashSet<Collider2D> releasedMarbles = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D marbleRigidbody = other.attachedRigidbody;

        if (marbleRigidbody == null ||
            !marbleRigidbody.CompareTag("Marble"))
        {
            return;
        }

        if (marbleTimers.ContainsKey(other) ||
            releasedMarbles.Contains(other))
        {
            return;
        }

        SetFunnelDuration(other);
    }

    private void SetFunnelDuration(Collider2D marbleCollider)
    {
        float duration = Random.Range(
            minimumDuration,
            maximumDuration
        );

        string marbleName =
            marbleCollider.attachedRigidbody.name;

        Debug.Log(
            $"{marbleName} will leave the funnel " +
            $"after {duration:F2} seconds"
        );

        Coroutine timer = StartCoroutine(
            ReleaseAfterDuration(
                marbleCollider,
                duration
            )
        );

        marbleTimers.Add(marbleCollider, timer);
    }

    private IEnumerator ReleaseAfterDuration(
        Collider2D marbleCollider,
        float duration
    )
    {
        yield return new WaitForSeconds(duration);

        marbleTimers.Remove(marbleCollider);

        if (marbleCollider == null)
        {
            yield break;
        }

        releasedMarbles.Add(marbleCollider);

        // Only this marble can now pass through.
        Physics2D.IgnoreCollision(
            marbleCollider,
            holdingCollider,
            true
        );

        Debug.Log(
            $"{marbleCollider.attachedRigidbody.name} " +
            "is leaving the funnel"
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!releasedMarbles.Remove(other))
        {
            return;
        }

        // Restore the collision after the marble has left.
        Physics2D.IgnoreCollision(
            other,
            holdingCollider,
            false
        );
    }

    public void ResetFunnel()
    {
        StopAllCoroutines();

        foreach (Collider2D marbleCollider in releasedMarbles)
        {
            if (marbleCollider != null)
            {
                Physics2D.IgnoreCollision(
                    marbleCollider,
                    holdingCollider,
                    false
                );
            }
        }

        marbleTimers.Clear();
        releasedMarbles.Clear();
    }
}