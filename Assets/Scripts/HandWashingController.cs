using UnityEngine;
using Fusion;
using System.Linq;

public class HandWashingController : NetworkBehaviour
{
    [SerializeField]
    private NetworkPrefabRef dishPrefab;

    private BoxCollider zoneCollider;

    public override void Spawned()
    {
        Runner.Spawn(dishPrefab, transform.position + Vector3.up * 1, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
        {
            // This is where you can initialize the dish if needed
            Debug.Log($"[Network] Dish spawned with ID: {obj.Id}");
        });
        Debug.Log($"[Network] HandWashingController spawned with ID: {Object.Id}");

        this.zoneCollider = GetComponent<BoxCollider>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        var castResults = Physics.BoxCastAll(transform.position, this.zoneCollider.size / 2, Vector3.up);
        // Debug.Log($"[Network] HandWashingController FixedUpdateNetwork - Casted {castResults.Length} objects in the zone.");
        var hasHandInZone = castResults.Any(hit => isHand(hit.collider.gameObject));
        if (!hasHandInZone) return;

        foreach (var hit in castResults)
        {
            if (hit.collider.gameObject.TryGetComponent(out WashPlate plate))
            {
                plate.CleanProgress = Mathf.Min(plate.CleanProgress + 1, 100);
            }
        }
    }

    private bool isHand(GameObject obj)
    {
        return obj.name == "TipCollider" || obj.name == "FingerTrackingHandCollider";
    }
}
