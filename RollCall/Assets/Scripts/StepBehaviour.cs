using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepBehaviouir : MonoBehaviour
{
    [SerializeField] private List<Rigidbody2D> pushers;

    [Header("Movement")]
    [SerializeField]
    private Vector2 movementOffset =
        new Vector2(0f, 0.5f);

    [SerializeField] private float speed = 2f;

    [Header("Timing")]
    [SerializeField] private float pauseAtTop = 0.1f;
    [SerializeField] private float delayBetweenStarts = 0.15f;
    [SerializeField] private float delayBeforeRepeating = 0.2f;

    private readonly List<Vector2> startLocalPositions = new();

    private int activePushers;

    private void Awake()
    {
        foreach (Rigidbody2D pusher in pushers)
        {
            pusher.bodyType = RigidbodyType2D.Kinematic;
            pusher.gravityScale = 0f;

            pusher.interpolation =
                RigidbodyInterpolation2D.Interpolate;

            // Store the position relative to this prefab,
            // rather than as a world coordinate.
            Vector2 localPosition =
                transform.InverseTransformPoint(
                    pusher.transform.position
                );

            startLocalPositions.Add(localPosition);
        }
    }

    private void Start()
    {
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        while (true)
        {
            for (int i = 0; i < pushers.Count; i++)
            {
                activePushers++;

                StartCoroutine(
                    RunPusherCycle(
                        pushers[i],
                        startLocalPositions[i]
                    )
                );

                yield return new WaitForSeconds(
                    delayBetweenStarts
                );
            }

            yield return new WaitUntil(
                () => activePushers == 0
            );

            yield return new WaitForSeconds(
                delayBeforeRepeating
            );
        }
    }

    private IEnumerator RunPusherCycle(
        Rigidbody2D pusher,
        Vector2 startLocalPosition
    )
    {
        Vector2 raisedLocalPosition =
            startLocalPosition + movementOffset;

        yield return MoveToLocalPosition(
            pusher,
            raisedLocalPosition
        );

        yield return new WaitForSeconds(
            pauseAtTop
        );

        yield return MoveToLocalPosition(
            pusher,
            startLocalPosition
        );

        activePushers--;
    }

    private IEnumerator MoveToLocalPosition(
        Rigidbody2D pusher,
        Vector2 targetLocalPosition
    )
    {
        while (true)
        {
            // Recalculate the world target every frame.
            // This accounts for the whole prefab being moved.
            Vector2 targetWorldPosition =
                transform.TransformPoint(
                    targetLocalPosition
                );

            if (Vector2.Distance(
                pusher.position,
                targetWorldPosition
            ) <= 0.01f)
            {
                pusher.MovePosition(
                    targetWorldPosition
                );

                yield break;
            }

            Vector2 nextPosition = Vector2.MoveTowards(
                pusher.position,
                targetWorldPosition,
                speed * Time.fixedDeltaTime
            );

            pusher.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }
    }
}