using UnityEngine;

public class DeliveryDetectorArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Delivery>(out var delivery))
        {
            Debug.Log("Delivery detected in area: " + gameObject.name);
            delivery.IsReadyForDelivery = true;
        }
    }
}
