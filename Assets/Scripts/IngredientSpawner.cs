using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class IngredientSpawner : NetworkBehaviour
{
    [System.Serializable]
    private class IngredientSpawnData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotationEuler;
        public Vector3 scale;
        public NetworkPrefabRef prefabRef;
    }

    [System.Serializable]
    public struct TransformData
    {
        public GameObject obj;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string name;

        public TransformData(GameObject obj, Vector3 pos, Quaternion rot, Vector3 scl, string name)
        {
            this.obj = obj;
            this.position = pos;
            this.rotation = rot;
            this.scale = scl;
            this.name = name;
        }
    }

    [SerializeField]
    private List<IngredientSpawnData> ingredientTransformData = new List<IngredientSpawnData>();

    List<TransformData> spawnedIngredients = new List<TransformData>();

    private void CollectData()
    {
        ingredientTransformData.Clear();

        foreach (Transform child in transform)
        {
            IngredientSpawnData newData = new IngredientSpawnData
            {
                name = child.gameObject.name,
                position = child.localPosition,
                rotationEuler = child.localRotation.eulerAngles,
                scale = child.localScale
            };

            ingredientTransformData.Add(newData);
        }
    }

    public void SpawnChildObject()
    {
        foreach (var ingredientData in ingredientTransformData)
        {
            var ingredientObject = Runner.Spawn(ingredientData.prefabRef);

            ingredientObject.transform.position = transform.position;
            ingredientObject.transform.rotation = transform.rotation;
            ingredientObject.transform.localScale = transform.localScale;
            //ingredientObject.transform.SetParent(transform, worldPositionStays: true);

            ingredientObject.transform.position += ingredientData.position;
            ingredientObject.transform.rotation *= Quaternion.Euler(ingredientData.rotationEuler);
            ingredientObject.transform.localScale = Vector3.Scale(ingredientObject.transform.localScale, ingredientData.scale);

            TransformData data = new TransformData(
                ingredientObject.gameObject,
                ingredientObject.transform.position,
                ingredientObject.transform.rotation,
                ingredientObject.transform.localScale,
                ingredientData.name
            );

            Debug.Log(ingredientData.name + " position: " + ingredientObject.transform.position);

            spawnedIngredients.Add(data);
        }
    }

    public void Start()
    {
        // CollectData();
    }

    public void ApplyTransform()
    {
        foreach (var data in spawnedIngredients)
        {
            data.obj.transform.position = data.position;
            data.obj.transform.rotation = data.rotation;
            data.obj.transform.localScale = data.scale;
            Debug.Log(data.name + " position: " + data.obj.transform.position);
        }
    }

    public IEnumerator ApplyTransformAfterSeconds(float second)
    {
        yield return new WaitForSeconds(second);
        ApplyTransform();
    } 
}
