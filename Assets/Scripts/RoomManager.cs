using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    public MRUKRoom room;

    private void AdjustObjectComponents()
    {
        Debug.Log(room.gameObject.GetType());
        foreach (Transform child in room.gameObject.transform)
        {
            Debug.Log(child.gameObject.name);
            foreach (Transform grandChild in child)
            {
                Debug.Log(grandChild.gameObject.name);
                //grandChild.gameObject.GetComponent<MeshRenderer>().enabled = false;
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

    private IEnumerable WaitForFloorCreated()
    {
        while (true)
        {
            var floor = GameObject.Find("FLOOR_EffectMesh");
            if (floor)
            {
                //floor.layer = LayerMask.NameToLayer("Floor");
                //surface.layerMask = LayerMask.GetMask("Floor");
                break;
            }
            yield return null;
        }
        Debug.Log("The mesh of the room is ready");
    }


    // Initialize the room
    private IEnumerator Initialization()
    {
        yield return WaitForRoomCreated();

        yield return WaitForFloorCreated();

        AdjustObjectComponents();
    }

    public void EnableMRUKManager()
    {
        Debug.Log($"{nameof(RoomManager)} has been enabled due to scene availability");
        StartCoroutine(Initialization());
    }
}
