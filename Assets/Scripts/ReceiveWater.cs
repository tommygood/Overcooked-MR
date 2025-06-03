using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ReceiveWater : NetworkBehaviour
{
    [SerializeField]
    private GameObject waterEffect;
    [Networked]
    [OnChangedRender(nameof(OnHasWaterChanged))]
    private bool hasWater { get; set; } = false;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ReceiveWaterFromFaucet()
    {
        this.hasWater = true;
    }

    public void OnHasWaterChanged()
    {
        if (this.hasWater)
        {
            Debug.Log("Received water from faucet.");
            StartCoroutine(this.showWaterEffect());
        }
        else
        {
            Debug.Log("No water received.");
            this.waterEffect.SetActive(false);
        }
    }

    private IEnumerator showWaterEffect()
    {
        SoundManager.Instance.PlaySFX(SoundRegistry.SoundID.Pour, transform.position);
        yield return new WaitForSeconds(0.5f);
        this.waterEffect.SetActive(true);
    }
}
