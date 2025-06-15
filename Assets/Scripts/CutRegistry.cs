using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[CreateAssetMenu(menuName = "OvercookedMR/CutRegistry")]
public class CutRegistry : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string Name;
        public GameObject Prefab;
    }

    public Entry[] All;
}
