using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerIngredientSpawner : MonoBehaviour
{
    public GameObject prefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Collider")
        {
            Debug.Log(gameObject.name + " is touched!");

            var newObject = Instantiate(prefab, transform.position, transform.rotation);
            Vector3 size = gameObject.GetComponent<Renderer>().bounds.size;
            Vector3 offset = new Vector3(0, size.y, 0);
            newObject.transform.position += offset;
        }
    }
}
