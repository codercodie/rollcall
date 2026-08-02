using System.Collections;
using UnityEngine;

public class StartGate : MonoBehaviour
{
    [SerializeField] private Collider2D gateCollider;
    [SerializeField] private Transform gate;
    [SerializeField] private float openDistance = 0.5f;
    [SerializeField] private float movementDuration = 0.4f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Coroutine movementCoroutine;

    private void Awake()
    {
        closedPosition = gate.position;
        openPosition = closedPosition + Vector3.down * openDistance;

        CloseImmediately();
    }

    public void Open()
    {
        Debug.Log("Gate Open called");

        gateCollider.enabled = false;
        MoveGate(openPosition);
        gate.gameObject.SetActive(false);
    }

    public void Close()
    {
        Debug.Log("Gate Close called");

        gateCollider.enabled = true;
        MoveGate(closedPosition);
    }

    public void CloseImmediately()
    {
        gate.position = closedPosition;
        gateCollider.enabled = true;
    }

    private void MoveGate(Vector3 targetPosition)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(
            MoveGateCoroutine(targetPosition)
        );
    }

    private IEnumerator MoveGateCoroutine(Vector3 targetPosition)
    {
        Vector3 startingPosition = gate.position;
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / movementDuration
            );

            gate.position = Vector3.Lerp(
                startingPosition,
                targetPosition,
                progress
            );

            yield return null;
        }

        gate.position = targetPosition;
        movementCoroutine = null;
    }
}