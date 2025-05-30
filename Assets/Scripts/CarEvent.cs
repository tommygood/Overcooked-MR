using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class CarEvent : MonoBehaviour
{
    private Vector3 carPosition;
    private Quaternion carRotation;
    private PrometeoCarController prometeoCarController;
    private bool isReady = false;

    void Start()
    {
        gameObject.SetActive(false);
        prometeoCarController = GetComponent<PrometeoCarController>();
        StartCoroutine(Initialization());
    }

    IEnumerator Test()
    {
        while (true)
        {
            InvokeCarEvent();
            yield return new WaitForSeconds(5f);
        }
    }

    public void InvokeCarEvent()
    {
        gameObject.transform.position = carPosition;
        gameObject.transform.rotation = carRotation;
        for (int i = 0; i < 30; i++)
        {
            prometeoCarController.GoForward();
        }
    }

    private IEnumerator Initialization()
    {
        while (true)
        {
            MRUKRoom room = FindAnyObjectByType<MRUKRoom>();
            Debug.Log("[Debug] Waiting for the room created...");
            if (room)
            {
                foreach (Transform child in room.gameObject.transform)
                {
                    foreach (Transform grandChild in child)
                    {
                        if (grandChild.gameObject.name == "DOOR_FRAME_EffectMesh")
                        {
                            carPosition = grandChild.position + new Vector3(0.5f, 0, 0);
                            carRotation = grandChild.rotation;
                            transform.localScale *= 0.2f;
                        }
                    }
                }
                isReady = true;
                break;
            }
            yield return null;
        }
    }
}
