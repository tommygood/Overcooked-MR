using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

public class Courier : MonoBehaviour
{
    public float speed = 0.5f;

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
            transform.position = Vector3.MoveTowards(transform.position, destination.position, Time.deltaTime * this.speed);
            yield return null;
        }
        Debug.Log("Courier reached the destination: " + destination.name);

        // Pick up the delivery
        Debug.Log("Courier picking up delivery at: " + destination.name);
        Collider[] colliders = Physics.OverlapSphere(destination.position, 0.5f);
        Delivery delivery = null;
        foreach (var collider in colliders)
        {
            delivery = collider.GetComponent<Delivery>();
            if (delivery != null)
            {
                Debug.Log("Courier found delivery: " + delivery.name);
                break;
            }
        }
        if (delivery.gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true; // Prevent physics interactions while moving
            // Destroy(rb);
        }
        delivery.transform.SetParent(transform);

        while (Vector3.Distance(transform.position, endPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, Time.deltaTime * this.speed);
            yield return null;
        }
        Debug.Log("Courier reached the end point: " + endPoint.name);

        Destroy(gameObject);
    }
}
