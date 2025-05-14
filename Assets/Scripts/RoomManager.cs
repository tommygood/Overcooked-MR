using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public MRUKRoom room;

    public GameObject redCube;
    public GameObject blueCube;

    public List<GameObject> tables;

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
                if (grandChild.gameObject.name == "TABLE_EffectMesh")
                {
                    // Add test cube to tables
                    var newCube = Instantiate(blueCube, grandChild.transform);
                    newCube.transform.SetParent(grandChild.transform);

                    // Adjust the position of spawned objects
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(size.x / 2f - size.x / 10f, 0, size.z / 2f - size.z / 10f);
                    newCube.transform.position += offset;
                }
                if (grandChild.gameObject.name == "COUCH_EffectMesh")
                {
                    // Add test cube to tables
                    var newCube = Instantiate(redCube, grandChild.transform);
                    newCube.transform.SetParent(grandChild.transform);

                    // Adjust the position of spawned objects
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y / 2f, 0);
                    newCube.transform.position += offset;
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
