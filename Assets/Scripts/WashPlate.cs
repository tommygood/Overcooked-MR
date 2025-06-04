using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WashPlate : NetworkBehaviour
{
    [Networked]
    [OnChangedRender(nameof(OnCleanProgressChanged))]
    public int CleanProgress { get; set; } = 100;

    [Networked]
    [OnChangedRender(nameof(OnIsCleanChanged))]
    public bool IsClean { get; set; } = true;

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
        if (!Object.HasStateAuthority) return;

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
