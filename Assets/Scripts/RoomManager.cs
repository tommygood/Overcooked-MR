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

    private int ingredientIndex = 0;
    public int ingredientNum;

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
        // HACK: de-deup
        bool deliveryDetectorSpawned = false;
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
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    // The position of the couch is in the center
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y / 3f, 0);
                    newObject.transform.position += offset;

                    newObject.transform.position += gas_stove_set.position;
                    newObject.transform.Rotate(gas_stove_set.rotation.eulerAngles);
                    newObject.transform.localScale *= gas_stove_set.scale;

                    this.unparenting(newObject);
                    /*
                    // HACK
                    if (newObject.TryGetComponent(out StoveSetSpawner stoveSetSpawner))
                    {
                        stoveSetSpawner.SpawnChildObject();
                    }
                    */
                    Debug.Log("[Debug] Gas_Stove_Set spawned");
                }
                if (grandChild.gameObject.name == "BED_EffectMesh")
                {
                    // cutting plate
                    PrefabWithTransform cutting_plate = utensils.Find(x => x.name == "Cutting_Plate");
                    var newObject = Runner.Spawn(cutting_plate.networkPrefabRef);
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

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
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    // Place sink on the floor
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y, 0);
                    newObject.transform.position -= offset;

                    float objectHeight = 0;
                    foreach (Transform c in newObject.transform)
                    {
                        if (c.gameObject.name == "Prop_Sink_02")
                        {
                            objectHeight = c.GetComponent<Renderer>().bounds.size.y;
                        }
                    }
                    newObject.transform.position += new Vector3(0, objectHeight / 2, 0);

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
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    newObject.transform.position += casher.position;
                    newObject.transform.Rotate(casher.rotation.eulerAngles);
                    newObject.transform.localScale *= casher.scale;
                    Debug.Log("[Debug] Casher spawned");
                }
                if (grandChild.gameObject.name == "LAMP_EffectMesh")
                {
                    Destroy(grandChild.gameObject.GetComponent<BoxCollider>());

                    if (ingredientIndex < ingredientNum)
                    {
                        // ingredients
                        PrefabWithTransform ingredient = null;

                        switch (ingredientIndex)
                        {
                            case 0:
                                ingredient = utensils.Find(x => x.name == "AppleBox");
                                break;
                            case 1:
                                ingredient = utensils.Find(x => x.name == "BowlBox");
                                break;
                            case 2:
                                ingredient = utensils.Find(x => x.name == "BreadTray");
                                break;
                            case 3:
                                ingredient = utensils.Find(x => x.name == "CarrotBox");
                                break;
                            case 4:
                                ingredient = utensils.Find(x => x.name == "CheeseTray");
                                break;
                            case 5:
                                ingredient = utensils.Find(x => x.name == "HamburgerBunTray");
                                break;
                            case 6:
                                ingredient = utensils.Find(x => x.name == "LettuceBox");
                                break;
                            case 7:
                                ingredient = utensils.Find(x => x.name == "PlateBox");
                                break;
                            case 8:
                                ingredient = utensils.Find(x => x.name == "PumpkinBox");
                                break;
                            case 9:
                                ingredient = utensils.Find(x => x.name == "SearedGroundBeefTray");
                                break;
                            case 10:
                                ingredient = utensils.Find(x => x.name == "SteakTray");
                                break;
                            case 11:
                                ingredient = utensils.Find(x => x.name == "TomatoBox");
                                break;
                            case 12:
                                ingredient = utensils.Find(x => x.name == "TortillaTray");
                                break;
                            case 13:
                                ingredient = utensils.Find(x => x.name == "TrykeyBreastTray");
                                break;
                        }

                        Debug.Log(ingredient.name);

                        ingredientIndex++;

                        var newObject = Runner.Spawn(ingredient.networkPrefabRef);
                        newObject.transform.position = grandChild.gameObject.transform.position;
                        newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                        // Place ingredient on the floor
                        Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                        Vector3 size = bounds.size;
                        Vector3 offset = new Vector3(0, size.y, 0);
                        newObject.transform.position -= offset;

                        float objectHeight = 0;
                        foreach (Transform c in newObject.transform)
                        {
                            if (c.gameObject.name == "Container_C")
                            {
                                objectHeight = c.GetComponent<Renderer>().bounds.size.y;
                            }
                        }
                        // newObject.transform.position += new Vector3(0, objectHeight / 2, 0);

                        newObject.transform.position += ingredient.position;
                        newObject.transform.Rotate(ingredient.rotation.eulerAngles);
                        newObject.transform.localScale *= ingredient.scale;

                        // this.unparenting(newObject);
                        /*
                        // HACK
                        if (newObject.TryGetComponent(out IngredientSpawner ingredientSpawner))
                        {
                            ingredientSpawner.SpawnChildObject();
                        }
                        */

                        Debug.Log("[Debug] " + ingredient.name + " spawned");
                    }
                }
                if (grandChild.gameObject.name == "FLOOR_EffectMesh")
                {
                    // pole
                    PrefabWithTransform pole = utensils.Find(x => x.name == "Pole");
                    var newObject = Runner.Spawn(pole.networkPrefabRef);
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    newObject.transform.position += pole.position;
                    newObject.transform.Rotate(pole.rotation.eulerAngles);
                    newObject.transform.localScale *= pole.scale;
                    Debug.Log("[Debug] Pole spawned");
                }
                if (grandChild.gameObject.name == "FLOOR_EffectMesh")
                {
                    // pole
                    PrefabWithTransform floor = utensils.Find(x => x.name == "Floor");
                    var newObject = Runner.Spawn(floor.networkPrefabRef);
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    newObject.transform.position += floor.position;
                    newObject.transform.Rotate(floor.rotation.eulerAngles);
                    newObject.transform.localScale *= floor.scale;
                    Debug.Log("[Debug] Floor spawned");
                }
                if (grandChild.gameObject.name == "TABLE_EffectMesh")
                {
                    // Place a delivery ring on the table
                    PrefabWithTransform delivery_ring = utensils.Find(x => x.name == "Delivery_Ring");
                    var newObject = Runner.Spawn(delivery_ring.networkPrefabRef);
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    newObject.transform.position += delivery_ring.position;
                    newObject.transform.Rotate(delivery_ring.rotation.eulerAngles);
                    newObject.transform.localScale *= delivery_ring.scale;

                    // Add the table to the table list
                    tables.Add(grandChild.gameObject);
                    Debug.Log("[Debug] Delivery_Ring spawned");
                }

                if (grandChild.gameObject.name == "DOOR_FRAME_EffectMesh")
                {
                    var collider = grandChild.gameObject.GetComponent<Collider>();
                    Destroy(collider);
                }

                if (grandChild.gameObject.name == "DOOR_FRAME_EffectMesh" && !deliveryDetectorSpawned)
                {
                    deliveryDetectorSpawned = true;

                    PrefabWithTransform deliveryDetector = utensils.Find(x => x.name == "DeliveryDetector");
                    var newObject = Runner.Spawn(deliveryDetector.networkPrefabRef);
                    newObject.transform.position = grandChild.gameObject.transform.position;
                    newObject.transform.rotation = grandChild.gameObject.transform.rotation;

                    newObject.transform.position += deliveryDetector.position;
                    newObject.transform.Rotate(deliveryDetector.rotation.eulerAngles);
                    newObject.transform.localScale *= deliveryDetector.scale;
                    Debug.Log("[Debug] DeliveryDetector spawned");
                }
            }

        }
    }

    private void unparenting(NetworkObject networkObject)
    {
        StartCoroutine(this._unparenting(networkObject));
    }

    private IEnumerator _unparenting(NetworkObject networkObject)
    {
        yield return new WaitForSeconds(1);
        foreach (Transform c in networkObject.gameObject.transform)
        {
            if (!c.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (c.TryGetComponent(out NetworkObject no))
            {
                var originalScale = c.lossyScale;
                var ingredientObject = Runner.Spawn(
                    no,
                    c.position,
                    c.rotation
                );
                // Runner.Despawn(no);
                no.gameObject.SetActive(false);
                ingredientObject.transform.localScale = originalScale;
            }
            else
            {
                Debug.LogWarning("NetworkObject not found on: " + c.name);
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

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            // Initialize the room when the master client spawns
            StartCoroutine(Initialization());
        }
        else
        {
            Debug.LogWarning($"{nameof(RoomManager)} can only be enabled by the master client.");
        }
    }

    // Initialize the room
    private IEnumerator Initialization()
    {
        yield return WaitForRoomCreated();

        SetupRoom();
    }

    public void EnableMRUKManager() { }
}
