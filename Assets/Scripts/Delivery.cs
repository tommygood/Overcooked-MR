using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    public bool IsReadyForDelivery { get; set; } = false;
    // TODO: DI
    [SerializeField]
    private GameObject courierPrefab;

    private Rigidbody rb;

    public void Start()
    {
        Debug.Log("[Delivery] Delivery spawned: " + gameObject.name);
        this.rb = GetComponent<Rigidbody>();
        StartCoroutine(WaitForDelivery());
    }

    private IEnumerator WaitForDelivery()
    {
        yield return new WaitUntil(() => IsReadyForDelivery);
        Debug.Log("[Delivery] Delivery is ready for pickup");
        while (this.rb.velocity.magnitude > 0.1f)
        {
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("[Delivery] Courier is being spawned.");
        var anchor = FindObjectsByType<CourierAnchor>(FindObjectsSortMode.None)
            .FirstOrDefault(a => a.Ty == CourierAnchor.Type.Start);
        if (anchor == null)
        {
            Debug.LogError("[Delivery] No start anchor found for courier");
        }

        var courierGo = Instantiate(
            courierPrefab,
            anchor.transform.position,
            Quaternion.identity);
        var courier = courierGo.GetComponent<Courier>();
        if (courier == null)
        {
            Debug.LogError("[Delivery] No courier found on prefab");
        }
        courier.SetDestination(transform);
        Debug.Log("[Delivery] Courier destination set to: " + gameObject.name);
    }
}
