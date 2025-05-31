using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WashPlate : NetworkBehaviour
{
    [Networked]
    [OnChangedRender(nameof(OnCleanProgressChanged))]
    public int CleanProgress { get; set; }

    [Networked]
    [OnChangedRender(nameof(OnIsCleanChanged))]
    public bool IsClean { get; set; }

    [SerializeField]
    private Material cleanMaterial;
    [SerializeField]
    private Material dirtyMaterial;

    private Renderer plateRenderer;

    void Start()
    {
        this.plateRenderer = GetComponentInChildren<Renderer>();
    }

    public override void Spawned()
    {
        base.Spawned();
        CleanProgress = 0;
        IsClean = true;
        this.updateVisual();
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
