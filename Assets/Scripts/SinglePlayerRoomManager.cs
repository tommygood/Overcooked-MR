using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

[System.Serializable]
public class PrefabInfo
{
    public string name;
    public GameObject prefab;
    public Vector3 position = new Vector3(0, 0, 0);
    public Quaternion rotation = Quaternion.Euler(0, 0, 0);
    public float scale = 1.0f;
}

public class SinglePlayerRoomManager : MonoBehaviour
{

    public MRUKRoom room;

    public List<GameObject> tables;

    public List<PrefabInfo> utensils;

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

    void Start()
    {
        StartCoroutine(Initialization());
    }

    private void SetupRoom()
    {
        // HACK: de-dup
        bool deliveryDetectorSpawned = false;
        foreach (Transform child in room.gameObject.transform)
        {
            Debug.Log("[Debug] " + child.gameObject.name);
            foreach (Transform grandChild in child)
            {
                if (grandChild.gameObject.name == "COUCH_EffectMesh")
                {
                    // gas stove set
                    PrefabInfo gas_stove_set = utensils.Find(x => x.name == "GasStoveSet");
                    if (gas_stove_set == null)
                    {
                        Debug.Log("[Debug] GasStoveSet prefab not found!");
                        continue;
                    }
                    var newObject = Instantiate(gas_stove_set.prefab, grandChild.gameObject.transform);

                    // The position of the couch is in the center
                    Bounds bounds = grandChild.GetComponent<Renderer>().bounds;
                    Vector3 size = bounds.size;
                    Vector3 offset = new Vector3(0, size.y / 3f, 0);
                    newObject.transform.position += offset;

                    newObject.transform.position += gas_stove_set.position;
                    newObject.transform.Rotate(gas_stove_set.rotation.eulerAngles);
                    newObject.transform.localScale *= gas_stove_set.scale;

                    Debug.Log("[Debug] GasStoveSet spawned");
                }
                if (grandChild.gameObject.name == "BED_EffectMesh")
                {
                    // cutting plate
                    PrefabInfo cutting_plate = utensils.Find(x => x.name == "CuttingPlate");
                    if (cutting_plate == null)
                    {
                        Debug.Log("[Debug] CuttingPlate prefab not found!");
                        continue;
                    }
                    var newObject = Instantiate(cutting_plate.prefab, grandChild.gameObject.transform);

                    newObject.transform.position += cutting_plate.position;
                    newObject.transform.Rotate(cutting_plate.rotation.eulerAngles);
                    newObject.transform.localScale *= cutting_plate.scale;
                    Debug.Log("[Debug] CuttingPlate spawned");
                }
                if (grandChild.gameObject.name == "PLANT_EffectMesh")
                {
                    Destroy(grandChild.gameObject.GetComponent<BoxCollider>());
                    // sink
                    PrefabInfo sink = utensils.Find(x => x.name == "Sink");
                    if (sink == null)
                    {
                        Debug.Log("[Debug] Sink prefab not found!");
                        continue;
                    }
                    var newObject = Instantiate(sink.prefab, grandChild.gameObject.transform);

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
                    PrefabInfo casher = utensils.Find(x => x.name == "Casher");
                    if (casher == null)
                    {
                        Debug.Log("[Debug] Casher prefab not found!");
                        continue;
                    }
                    var newObject = Instantiate(casher.prefab, grandChild.gameObject.transform);

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
                        PrefabInfo ingredient = null;

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
                                ingredient = utensils.Find(x => x.name == "TurkeyBreastTray");
                                break;
                        }
                        if (ingredient == null)
                        {
                            Debug.Log("[Debug] Ingredient prefab not found for index: " + ingredientIndex);
                            continue;
                        }

                        ingredientIndex++;

                        var newObject = Instantiate(ingredient.prefab, grandChild.gameObject.transform);

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

                        Debug.Log("[Debug] " + ingredient.name + " spawned");
                    }
                }
                if (grandChild.gameObject.name == "TABLE_EffectMesh")
                {
                    // Place a delivery ring on the table
                    PrefabInfo delivery_ring = utensils.Find(x => x.name == "DeliveryRing");
                    if (delivery_ring == null)
                    {
                        Debug.Log("[Debug] DeliveryRing prefab not found!");
                        continue;
                    }

                    var newObject = Instantiate(delivery_ring.prefab, grandChild.gameObject.transform);
                    newObject.transform.position += delivery_ring.position;
                    newObject.transform.Rotate(delivery_ring.rotation.eulerAngles);
                    newObject.transform.localScale *= delivery_ring.scale;

                    // Add the table to the table list
                    tables.Add(grandChild.gameObject);
                    Debug.Log("[Debug] DeliveryRing spawned");
                }

                if (grandChild.gameObject.name == "DOOR_FRAME_EffectMesh")
                {
                    var collider = grandChild.gameObject.GetComponent<Collider>();
                    Destroy(collider);
                }

                if (grandChild.gameObject.name == "DOOR_FRAME_EffectMesh" && !deliveryDetectorSpawned)
                {
                    deliveryDetectorSpawned = true;
                    PrefabInfo deliveryDetector = utensils.Find(x => x.name == "DeliveryDetector");
                    var newObject = Instantiate(deliveryDetector.prefab, grandChild.gameObject.transform);
                    newObject.transform.position += deliveryDetector.position;
                    newObject.transform.Rotate(deliveryDetector.rotation.eulerAngles);
                    newObject.transform.localScale *= deliveryDetector.scale;
                    Debug.Log("[Debug] DeliveryDetector spawned");
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

    public void EnableMRUKManager() { }
}
