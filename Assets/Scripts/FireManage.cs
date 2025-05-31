using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FireManager : NetworkBehaviour
{
    public float fireInterval = 15f;

    [Networked]
    private TickTimer fireTimer { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            return; // 確保只有擁有狀態權限的物件可以處理固定更新
        }

        if (fireTimer.Expired(Runner))
        {
            fireTimer = TickTimer.CreateFromSeconds(Runner, fireInterval);

            StoveControl[] furnaces = FindObjectsByType<StoveControl>(FindObjectsSortMode.None);

            if (furnaces.Length == 0)
            {
                return;
            }

            int index = Random.Range(0, furnaces.Length);
            furnaces[index].Rpc_StartFire();
        }

        if (!fireTimer.IsRunning)
        {
            fireTimer = TickTimer.CreateFromSeconds(Runner, fireInterval);
        }
    }
}
