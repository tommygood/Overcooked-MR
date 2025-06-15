using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashPlate : MonoBehaviour
{
    public int CleanProgress
    {
        get => cleanProgress;
        set
        {
            cleanProgress = Mathf.Clamp(value, 0, 100);
            OnCleanProgressChanged();
        }
    }
    private int cleanProgress = 100;

    public bool IsClean
    {
        get => isClean;
        set
        {
            isClean = value;
            OnIsCleanChanged();
        }
    }
    private bool isClean = true;

    [SerializeField]
    private Material cleanMaterial;
    [SerializeField]
    private Material dirtyMaterial;

    private Renderer plateRenderer;

    void Start()
    {
        this.plateRenderer = GetComponentInChildren<Renderer>();
    }

    public void OnCleanProgressChanged()
    {
        Debug.Log($"Plate clean progress updated: {CleanProgress}% - IsClean: {IsClean}");

        if (CleanProgress >= 100)
        {
            IsClean = true;
        }
        else
        {
            IsClean = false;
        }
    }

    public void OnIsCleanChanged()
    {
        Debug.Log($"Plate is now {(IsClean ? "clean" : "dirty")}");
        this.updateVisual();

        if (this.IsClean)
        {
            SoundManager.Instance.PlaySFX(SoundRegistry.SoundID.BowlCleaned, transform.position);
        }
    }

    private void updateVisual()
    {
        if (plateRenderer != null)
        {
            plateRenderer.material = IsClean ? cleanMaterial : dirtyMaterial;
        }
        else
        {
            Debug.LogWarning("Plate renderer not found. Cannot update material.");
        }
    }
}
