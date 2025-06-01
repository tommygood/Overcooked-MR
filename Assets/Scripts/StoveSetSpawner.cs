using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StoveSetSpawner : NetworkBehaviour
{

    [System.Serializable]
    private class StoveSpawnData
    {
        public Vector3 position;
        public Vector3 rotationEuler;
        public Vector3 scale;
        public NetworkPrefabRef prefabRef;
    }

    [SerializeField]
    private StoveSpawnData[] stoveSetTransformData;

    public override void Spawned()
    {
        base.Spawned();
        if (Object.HasStateAuthority)
        {
            spawnChildObject();
        }
    }

    private void spawnChildObject()
    {
        foreach (var stoveData in stoveSetTransformData)
        {
            var stoveObject = Runner.Spawn(stoveData.prefabRef);

            stoveObject.transform.position = transform.position;
            stoveObject.transform.rotation = transform.rotation;
            stoveObject.transform.localScale = transform.localScale;

            stoveObject.transform.position += stoveData.position;
            stoveObject.transform.rotation *= Quaternion.Euler(stoveData.rotationEuler);
            stoveObject.transform.localScale = Vector3.Scale(stoveObject.transform.localScale, stoveData.scale);
        }
    }
}
