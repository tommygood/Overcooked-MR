using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

public class Courier : MonoBehaviour
{
    private bool hasDestination = false;

    public void SetDestination(Transform destination)
    {
        if (this.hasDestination)
        {
            Debug.LogWarning("Courier already has a destination set. Ignoring new destination.");
            return;
        }
        this.hasDestination = true;

        Transform endPoint = null;
        foreach (var anchor in FindObjectsOfType<CourierAnchor>())
        {
            if (anchor.Ty == CourierAnchor.Type.End)
            {
                endPoint = anchor.transform;
                break;
            }
        }
        if (endPoint == null)
        {
            Debug.LogError("No end anchor found for courier.");
            return;
        }

        Debug.Log("Courier set to move to destination: " + destination.name);
        StartCoroutine(MoveToDestination(destination, endPoint));
    }

    private IEnumerator MoveToDestination(Transform destination, Transform endPoint)
    {
        while (Vector3.Distance(transform.position, destination.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.position, Time.deltaTime * 5f);
            yield return null;
        }
        Debug.Log("Courier reached the destination: " + destination.name);

        // TODO: pick up the delivery
        Debug.Log("Courier picking up delivery at: " + destination.name);

        while (Vector3.Distance(transform.position, endPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, Time.deltaTime * 5f);
            yield return null;
        }
        Debug.Log("Courier reached the end point: " + endPoint.name);

        Destroy(gameObject);
    }
}
