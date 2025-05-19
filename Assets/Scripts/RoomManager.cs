using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class PrefabWithTransform
{
    public string name;  // Name of the prefab
    public GameObject prefab;  // Prefab reference
    public Vector3 position = new Vector3(0, 0, 0);  // Position of the prefab
    public Quaternion rotation = Quaternion.Euler(0, 0, 0);  // Rotation of the prefab
    public float scale = 1.0f;  // Scale of the prefab
}

public class RoomManager : MonoBehaviour
{
    public MRUKRoom room;

    public List<GameObject> tables;

    public List<PrefabWithTransform> utensils;
    /*
        0: Pan
        1: Pot
        2: Ladle
        3: Sink
    */

    private void SetupTables()
    {
        foreach (Transform child in room.gameObject.transform)
        {
            Debug.Log(child.gameObject.name);
            foreach (Transform grandChild in child)
            {
                // if (grandChild.gameObject.name == "TABLE_EffectMesh")
                // {
                //     tables.Add(grandChild.gameObject);
                //     Debug.Log(grandChild.gameObject.name);
                //     var tableNumber = Instantiate(new TextMeshProUGUI(), grandChild.transform);
                //     tableNumber.text = (tables.IndexOf(grandChild.gameObject) + 1).ToString();
                //     tableNumber.transform.SetParent(grandChild.transform);
                //     var newCube = Instantiate(cube, grandChild.transform);
                //     newCube.transform.SetParent(grandChild.transform);
                // }

                var text = Instantiate(new GameObject(), grandChild.transform).AddComponent<TextMeshPro>();
                text.text = grandChild.transform.position.ToString();
                text.alignment = TextAlignmentOptions.Center;
                text.transform.localScale *= 0.05f;
                text.transform.SetParent(grandChild.transform);

                if (grandChild.gameObject.name == "TABLE_EffectMesh")
                {
                    // Add objects to tables
                    PrefabWithTransform pan = utensils.Find(x => x.name == "Pan");
                    var newObject = Instantiate(pan.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    // Adjust the position of spawned objects
                    // Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    // Vector3 size = bounds.size;
                    // Vector3 offset = new Vector3(size.x / 2f - size.x / 10f, 0, size.z / 2f - size.z / 10f);
                    // newObject.transform.position += offset;

                    newObject.transform.position += pan.position;
                    newObject.transform.Rotate(pan.rotation.eulerAngles);
                    newObject.transform.localScale *= pan.scale;
                }
                if (grandChild.gameObject.name == "COUCH_EffectMesh")
                {
                    // Add objects to couches
                    PrefabWithTransform pot = utensils.Find(x => x.name == "Pot");
                    var newObject = Instantiate(pot.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    // Adjust the position of spawned objects
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y / 2f, 0);
                    newObject.transform.position += offset;

                    newObject.transform.position += pot.position;
                    newObject.transform.Rotate(pot.rotation.eulerAngles);
                    newObject.transform.localScale *= pot.scale;
                }
            }

        }
    }

    private IEnumerator WaitForRoomCreated()
    {
        while (true)
        {
            room = FindAnyObjectByType<MRUKRoom>();
            if (room) break;
            yield return null;
        }
        Debug.Log("The room is ready");
    }

    // Initialize the room
    private IEnumerator Initialization()
    {
        yield return WaitForRoomCreated();
        
        SetupTables();
    }

    public void EnableMRUKManager()
    {
        Debug.Log($"{nameof(RoomManager)} has been enabled due to scene availability");
        StartCoroutine(Initialization());
    }
}
