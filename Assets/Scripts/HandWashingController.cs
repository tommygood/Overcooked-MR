using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HandWashingController : NetworkBehaviour
{
    [Tooltip("Tag assigned to player hand colliders (e.g., 'PlayerHand').")]
    [SerializeField] private string handTag = "PlayerHand";

    [SerializeField]
    private NetworkPrefabRef dishPrefab;

    [Networked]
    private NetworkDictionary<NetworkId, NetworkBool> _objectsInZone { get; } = new NetworkDictionary<NetworkId, NetworkBool>();

    private void OnTriggerEnter(Collider other)
    {
        // Only proceed if this is a hand collider and we are the local client with InputAuthority
        if (true /* other.CompareTag(handTag) && other.GetComponentInParent<OVRHand>() != null */ )
        {
            // Get the PlayerRef associated with this hand.
            // This assumes your player character has a NetworkObject and this hand is part of it.
            // You might need a helper method or a reference to your player's NetworkObject.
            NetworkObject playerNetworkObject = other.GetComponentInParent<NetworkObject>();
            if (playerNetworkObject != null && playerNetworkObject.InputAuthority.IsRealPlayer)
            {
                Debug.Log($"[Local] Hand of {playerNetworkObject.InputAuthority} entered {gameObject.name}. Requesting network update.");
                // Send an RPC to the State Authority (Host/Server) to update the state
                Rpc_UpdateZoneState(playerNetworkObject.Id, true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (true /* other.CompareTag(handTag) && other.GetComponentInParent<OVRHand>() != null */)
        {
            NetworkObject playerNetworkObject = other.GetComponentInParent<NetworkObject>();
            if (playerNetworkObject != null)
            {
                Debug.Log($"[Local] Hand of {playerNetworkObject.InputAuthority} exited {gameObject.name}. Requesting network update.");
                Rpc_UpdateZoneState(playerNetworkObject.Id, false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        NetworkObject playerNetworkObject = other.GetComponentInParent<NetworkObject>();
        if (playerNetworkObject == null || !playerNetworkObject.InputAuthority.IsRealPlayer)
        {
            return; // Not a valid hand or not a real player
        }
        Debug.Log($"[Local] Hand of {playerNetworkObject.InputAuthority} is still in {gameObject.name}.");
    }

    public override void Spawned()
    {
        base.Spawned();
        Runner.Spawn(dishPrefab, transform.position + Vector3.up * 3, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
        {
            // This is where you can initialize the dish if needed
            Debug.Log($"[Network] Dish spawned with ID: {obj.Id}");
        });
        Debug.Log($"[Network] HandWashingController spawned with ID: {Object.Id}");
    }

    // RPC: Sent by the client (InputAuthority) to the Host/Server (StateAuthority)
    // Only the InputAuthority for the player's hand can send this.
    // Only the StateAuthority for this HandWashingController will execute it.
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_UpdateZoneState(NetworkId id, bool isInZone)
    {
        // This code only runs on the Host/Server (State Authority)
        if (!Object.HasStateAuthority) return;

        if (isInZone)
        {
            _objectsInZone.Set(id, true);
        }
        else
        {
            _objectsInZone.Remove(id);
        }

        Debug.Log($"[Server] Object {id} is now {(isInZone ? "in" : "out of")} the zone: {_objectsInZone.Count} hands in zone.");
    }

    // Helper to get the NetworkObject of the player's hand parent, so we can get its InputAuthority
    // You might already have a central PlayerManager that maps OVRHand to PlayerRef.
    // This is a common pattern for networked XR input.
    public override void Render()
    {
        // This is where you might have visual feedback that's not networked
        // For example, if the local player's hand is inside, play a haptic.
    }

    public override void FixedUpdateNetwork()
    {
        // This is where you might handle physics or other fixed updates
        // For example, if you want to check if the hand is still in the zone every frame.
    }
}
