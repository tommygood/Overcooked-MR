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

    public override void Spawned()
    {
        base.Spawned();
        CleanProgress = 0;
        IsClean = true;
    }

    public void OnCleanProgressChanged()
    {
        if (!Object.HasStateAuthority) return;

        if (CleanProgress >= 100)
        {
            IsClean = true;
            CleanProgress = 100;
        }
        else
        {
            IsClean = false;
        }

        Debug.Log($"Plate clean progress updated: {CleanProgress}% - IsClean: {IsClean}");
    }

    public void OnIsCleanChanged()
    {
        // TODO: update visual
        Debug.Log($"Plate is now {(IsClean ? "clean" : "dirty")}");
    }
}
