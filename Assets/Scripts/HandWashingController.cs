using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class HandWashingController : NetworkBehaviour
{
    [SerializeField]
    private NetworkPrefabRef dishPrefab;

    [SerializeField]
    private GameObject debugVisual;

    [SerializeField]
    private GameObject debugPlateVisual;

    [Networked]
    [OnChangedRender(nameof(updateDebugVisual))]
    public bool IsHandInZone { get; set; } = false;

    private List<WashPlate> platesInZone = new List<WashPlate>();

    private void updateDebugVisual()
    {
        if (this.debugVisual == null)
        {
            Debug.LogWarning("[Network] HandWashingController updateDebugVisual - Debug visual is not set, trying to find it in the hierarchy.");
            this.debugVisual = transform.Find("Visual")?.gameObject;
            if (this.debugVisual == null)
            {
                Debug.LogError("[Network] HandWashingController updateDebugVisual - Debug visual is not set.");
                return;
            }
        }

        this.debugVisual.SetActive(IsHandInZone);
    }

    public override void Spawned()
    {
        Runner.Spawn(dishPrefab, transform.position + Vector3.up * 1, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
        {
            // This is where you can initialize the dish if needed
            Debug.Log($"[Network] Dish spawned with ID: {obj.Id}");
        });
        Debug.Log($"[Network] HandWashingController spawned with ID: {Object.Id}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if(this.isHand(other.gameObject))
        {
            IsHandInZone = true;
            Debug.Log($"[Network] HandWashingController OnTriggerEnter - Hand detected: {other.gameObject.name}");
        }
        else if (other.gameObject.TryGetComponent(out WashPlate plate))
        {
            if (!platesInZone.Contains(plate))
            {
                platesInZone.Add(plate);
                Debug.Log($"[Network] HandWashingController OnTriggerEnter - Plate detected: {plate.name}");
            }
        }
        else
        {
            Debug.Log($"[Network] HandWashingController OnTriggerEnter - Other object detected: {other.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        
        if (this.isHand(other.gameObject))
        {
            IsHandInZone = false;
            Debug.Log($"[Network] HandWashingController OnTriggerExit - Hand exited: {other.gameObject.name}");
        }
        else if (other.gameObject.TryGetComponent(out WashPlate plate))
        {
            if (platesInZone.Contains(plate))
            {
                platesInZone.Remove(plate);
                Debug.Log($"[Network] HandWashingController OnTriggerExit - Plate exited: {plate.name}");
            }
        }
        else
        {
            Debug.Log($"[Network] HandWashingController OnTriggerExit - Other object exited: {other.gameObject.name}");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        // this.debugPlateVisual?.SetActive(false);

        foreach (var plate in this.platesInZone)
        {
            plate.CleanProgress = Mathf.Min(plate.CleanProgress + 1, 100);
            // this.debugPlateVisual?.SetActive(true);
        }
    }

    private bool isHand(GameObject obj)
    {
        return obj.name == "TipCollider" || obj.name == "FingerTrackingHandCollider";
    }
}
