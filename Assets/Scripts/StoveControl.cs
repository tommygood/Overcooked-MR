using System.Collections;
using UnityEngine;
using Fusion;

public class StoveControl : NetworkBehaviour
{
    [SerializeField]
    public NetworkPrefabRef fireEffectPrefab;

    [Networked]
    private NetworkObject currentFire { get; set; }
    [Networked]
    private bool isOnFire { get; set; } = false;
    [Networked]
    public bool canMove { get; set; } = true;
    [Networked]
    private TickTimer waterTimer { get; set; }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_StartFire()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning("StartFire() 只能由擁有狀態權限的物件呼叫");
            return;
        }

        if (isOnFire)
        {
            Debug.Log("火已經存在，不重複產生");
            return;
        }

        if (fireEffectPrefab == null)
        {
            Debug.LogError("fireEffectPrefab未設");
            return;
        }

        Debug.Log("StartFire() 被呼叫");
        currentFire = Runner.Spawn(
            fireEffectPrefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity);
        isOnFire = true;
        canMove = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            return; // 確保只有擁有狀態權限的物件可以處理固定更新
        }

        if (waterTimer.Expired(Runner))
        {
            Debug.Log("水接觸時間已到，熄火");
            ExtinguishFire();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water_T") && isOnFire)
        {
            Debug.Log("碰到Water");
            this.waterTimer = TickTimer.CreateFromSeconds(Runner, 3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water_T"))
        {
            Debug.Log("離開Water");
            this.waterTimer = TickTimer.None;
        }
    }

    void ExtinguishFire()
    {
        Debug.Log("火已熄滅");
        if (currentFire)
        {
            Runner.Despawn(currentFire);
            currentFire = null;
        }

        isOnFire = false;
        canMove = true;
        this.waterTimer = TickTimer.None;
    }
}
