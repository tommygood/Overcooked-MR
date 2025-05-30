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

    /*
        3D Furniture:
        COUCH - COUCH_EffectMesh
        BED - BED_EffectMesh
        SCREEN - SCREEN_EffectMesh
        TABLE - TABLE_EffectMesh
        LAMP - LAMP_EffectMesh
        PLANT - PLANT_EffectMesh
        STORAGE - STORAGE_EffectMesh
        OTHER - OTHER_EffectMesh
    
        2D Furniture:
        WINDOW_FRAME - WINDOW_FRAME_EffectMesh
        WALL_ART - WALL_ART_EffectMesh
        DOOR_FRAME - DOOR_FRAME_EffectMesh

        Other:
        WALL_FACE - WALL_FACE_EffectMesh
        FLOOR - FLOOR_EffectMesh
        CEILING - CEILING_EffectMesh
    */

    /*
        object-furniture pair
        gas stove + pan + pot + spatula - couch
        cutting plate - bed
        sink - plant
        cashier - storage
        ingredients - floor
    */

    private void SetupRoom()
    {
        foreach (Transform child in room.gameObject.transform)
        {
            Debug.Log("[Debug] " + child.gameObject.name);
            foreach (Transform grandChild in child)
            {
                if (grandChild.gameObject.name == "TABLE_EffectMesh")
                {
                    /*
                    // pan
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
                    */
                }
                if (grandChild.gameObject.name == "COUCH_EffectMesh")
                {
                    // gas stove set
                    PrefabWithTransform gas_stove_set = utensils.Find(x => x.name == "Gas_Stove_Set");
                    var newObject = Instantiate(gas_stove_set.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    // The position of the couch is in the center
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y / 2f, 0);
                    newObject.transform.position += offset;

                    newObject.transform.position += gas_stove_set.position;
                    newObject.transform.Rotate(gas_stove_set.rotation.eulerAngles);
                    newObject.transform.localScale *= gas_stove_set.scale;
                    Debug.Log("[Debug] Gas_Stove_Set spawned");
                }
                if (grandChild.gameObject.name == "BED_EffectMesh")
                {
                    // cutting plate
                    PrefabWithTransform cutting_plate = utensils.Find(x => x.name == "Cutting_Plate");
                    var newObject = Instantiate(cutting_plate.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    newObject.transform.position += cutting_plate.position;
                    newObject.transform.Rotate(cutting_plate.rotation.eulerAngles);
                    newObject.transform.localScale *= cutting_plate.scale;
                    Debug.Log("[Debug] Cutting_Plate spawned");
                }
                if (grandChild.gameObject.name == "PLANT_EffectMesh")
                {
                    Destroy(grandChild.gameObject.GetComponent<BoxCollider>());
                    // sink
                    PrefabWithTransform sink = utensils.Find(x => x.name == "Sink");
                    var newObject = Instantiate(sink.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    newObject.transform.position += sink.position;
                    newObject.transform.Rotate(sink.rotation.eulerAngles);
                    newObject.transform.localScale *= sink.scale;
                    Debug.Log("[Debug] Sink spawned");
                }
                if (grandChild.gameObject.name == "STORAGE_EffectMesh")
                {
                    // casher
                    PrefabWithTransform casher = utensils.Find(x => x.name == "Casher");
                    var newObject = Instantiate(casher.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    // The position of the storage is in the center
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y / 2f, 0);
                    newObject.transform.position += offset;

                    newObject.transform.position += casher.position;
                    newObject.transform.Rotate(casher.rotation.eulerAngles);
                    newObject.transform.localScale *= casher.scale;
                    Debug.Log("[Debug] Casher spawned");
                }
                if (grandChild.gameObject.name == "FLOOR_EffectMesh")
                {
                    // ingredients
                    PrefabWithTransform ingredients = utensils.Find(x => x.name == "Ingredients");
                    var newObject = Instantiate(ingredients.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    newObject.transform.position += ingredients.position;
                    newObject.transform.Rotate(ingredients.rotation.eulerAngles);
                    newObject.transform.localScale *= ingredients.scale;
                    Debug.Log("[Debug] Ingredients spawned");
                }
                if (grandChild.gameObject.name == "FLOOR_EffectMesh")
                {
                    // pole
                    PrefabWithTransform pole = utensils.Find(x => x.name == "Pole");
                    var newObject = Instantiate(pole.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    newObject.transform.position += pole.position;
                    newObject.transform.Rotate(pole.rotation.eulerAngles);
                    newObject.transform.localScale *= pole.scale;
                    Debug.Log("[Debug] Pole spawned");
                }
                if (grandChild.gameObject.name == "TABLE_EffectMesh")
                {
                    // Place a delivery ring on the table
                    PrefabWithTransform delivery_ring = utensils.Find(x => x.name == "Delivery_Ring");
                    var newObject = Instantiate(delivery_ring.prefab, grandChild.transform);
                    newObject.transform.SetParent(grandChild.transform);

                    newObject.transform.position += delivery_ring.position;
                    newObject.transform.Rotate(delivery_ring.rotation.eulerAngles);
                    newObject.transform.localScale *= delivery_ring.scale;

                    //// Place the table number on the table
                    //Debug.Log(grandChild.gameObject.name);
                    //var tableNumber = Instantiate(new TextMeshProUGUI(), grandChild.transform);
                    //tableNumber.text = (tables.IndexOf(grandChild.gameObject) + 1).ToString();
                    //tableNumber.transform.SetParent(grandChild.transform);

                    // Add the table to the table list
                    tables.Add(grandChild.gameObject);
                    Debug.Log("[Debug] Delivery_Ring spawned");
                }
            }

        }
    }

    private IEnumerator WaitForRoomCreated()
    {
        while (true)
        {
            room = FindAnyObjectByType<MRUKRoom>();
            Debug.Log("[Debug] Waiting for the room created...");
            if (room) break;
            yield return null;
        }
        Debug.Log("[Debug] The room is ready");
    }

    // Initialize the room
    private IEnumerator Initialization()
    {
        yield return WaitForRoomCreated();
        
        SetupRoom();
    }

    public void EnableMRUKManager()
    {
        Debug.Log($"{nameof(RoomManager)} has been enabled due to scene availability");
        StartCoroutine(Initialization());
    }
}
