using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class CarEvent : MonoBehaviour
{
    private Vector3 carPosition = Vector3.zero;
    private Quaternion carRotation = Quaternion.identity;
    private PrometeoCarController prometeoCarController;
    private bool isReady = false;

    void Start()
    {
        gameObject.SetActive(true);
        prometeoCarController = GetComponent<PrometeoCarController>();
        for (int i = 0; i < 1000; i++)
        {
            prometeoCarController.GoForward();
        }
        //StartCoroutine(Initialization());
        //StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        while (true)
        {
            //gameObject.SetActive(true);
            InvokeCarEvent();
            yield return new WaitForSeconds(5f);
        }
    }

    public void InvokeCarEvent()
    {
        if (!isReady) return;

        gameObject.transform.position = carPosition;
        gameObject.transform.rotation = carRotation;
        for (int i = 0; i < 100; i++)
        {
            prometeoCarController.GoForward();
        }

        //gameObject.SetActive(false);
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
