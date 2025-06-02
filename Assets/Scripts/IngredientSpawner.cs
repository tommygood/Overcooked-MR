using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class IngredientSpawner : NetworkBehaviour
{
    [System.Serializable]
    private class IngredientSpawnData
    {
        public Vector3 position;
        public Vector3 rotationEuler;
        public Vector3 scale;
        public NetworkPrefabRef prefabRef;
    }

    [SerializeField]
    private IngredientSpawnData[] ingredientTransformData;

    public void SpawnChildObject()
    {
        foreach (var ingredientData in ingredientTransformData)
        {
            var ingredientObject = Runner.Spawn(ingredientData.prefabRef);

            ingredientObject.transform.position = transform.position;
            ingredientObject.transform.rotation = transform.rotation;
            ingredientObject.transform.localScale = transform.localScale;

            ingredientObject.transform.position += ingredientData.position;
            ingredientObject.transform.rotation *= Quaternion.Euler(ingredientData.rotationEuler);
            ingredientObject.transform.localScale = Vector3.Scale(ingredientObject.transform.localScale, ingredientData.scale);
        }
    }
}
