using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ReceiveWater : MonoBehaviour
{
    [SerializeField]
    private GameObject waterEffect;

    private bool hasWater 
    {
        get => _hasWater;
        set
        {
            _hasWater = value;
            OnHasWaterChanged();
        }
    }
    private bool _hasWater = false;

    public void ReceiveWaterFromFaucet()
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
