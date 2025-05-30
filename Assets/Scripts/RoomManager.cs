using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using Fusion;

[System.Serializable]
public class PrefabWithTransform
{
    public string name;  // Name of the prefab
    public NetworkPrefabRef networkPrefabRef;  // Network prefab reference for Fusion
    public Vector3 position = new Vector3(0, 0, 0);  // Position of the prefab
    public Quaternion rotation = Quaternion.Euler(0, 0, 0);  // Rotation of the prefab
    public float scale = 1.0f;  // Scale of the prefab
}

public class RoomManager : NetworkBehaviour
{
    public MRUKRoom room;

    public List<GameObject> tables;

    public List<PrefabWithTransform> utensils;

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
        casher - storage
        ingredients - lamp
        pole - floor
    */

    private void SetupRoom()
    {
        foreach (Transform child in room.gameObject.transform)
        {
            Debug.Log("[Debug] " + child.gameObject.name);
            foreach (Transform grandChild in child)
            {
                if (grandChild.gameObject.name == "COUCH_EffectMesh")
                {
                    // gas stove set
                    PrefabWithTransform gas_stove_set = utensils.Find(x => x.name == "Gas_Stove_Set");
                    var newObject = Runner.Spawn(gas_stove_set.networkPrefabRef);
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
                    var newObject = Runner.Spawn(cutting_plate.networkPrefabRef);
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
                    var newObject = Runner.Spawn(sink.networkPrefabRef);
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
                    var newObject = Runner.Spawn(casher.networkPrefabRef);
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
                if (grandChild.gameObject.name == "LAMP_EffectMesh")
                {
                    Destroy(grandChild.gameObject.GetComponent<BoxCollider>());
                    // ingredients
                    PrefabWithTransform ingredients = utensils.Find(x => x.name == "Ingredients");
                    var newObject = Runner.Spawn(ingredients.networkPrefabRef);
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
                    var newObject = Runner.Spawn(pole.networkPrefabRef);
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
                    var newObject = Runner.Spawn(delivery_ring.networkPrefabRef);
                    newObject.transform.SetParent(grandChild.transform);

                    newObject.transform.position += delivery_ring.position;
                    newObject.transform.Rotate(delivery_ring.rotation.eulerAngles);
                    newObject.transform.localScale *= delivery_ring.scale;

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
        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning($"{nameof(RoomManager)} can only be enabled by the master client.");
            return;
        }

        Debug.Log($"{nameof(RoomManager)} has been enabled due to scene availability");
        StartCoroutine(Initialization());
    }
}
