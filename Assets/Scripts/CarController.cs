using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class CarController : MonoBehaviour
{
    private Vector3 carPosition;
    private Quaternion carRotation;

    public float interval;
    public float goForwardDuration;
    public float TurnLeftDuration;

    public Vector3 position;
    public Quaternion rotation;
    public float scale;

    public GameObject car;
    public PrometeoCarController prometeoCarController;

    void Start()
    {
        StartCoroutine(MainRoutine());
    }

    private IEnumerator MainRoutine()
    {
        yield return StartCoroutine(Initialization());
        yield return StartCoroutine(InvokeCarEvent());
    }

    private IEnumerator InvokeCarEvent()
    {
        while (true)
        {
            car.SetActive(true);
            car.transform.position = carPosition;
            car.transform.rotation = carRotation;
            StartCoroutine(GoForward());
            // StartCoroutine(TurnLeft());
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator TurnLeft()
    {
        float endTime = Time.time + TurnLeftDuration;

        while (Time.time < endTime)
        {
            prometeoCarController.TurnLeft();
            yield return null;
        }
    }

    private IEnumerator GoForward()
    {
        float endTime = Time.time + goForwardDuration;

        while (Time.time < endTime)
        {
            prometeoCarController.GoForward();
            yield return null;
        }
        car.transform.position = carPosition;
        car.transform.rotation = carRotation;
        car.SetActive(false);
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
                            carPosition = grandChild.position + position;
                            carRotation = grandChild.rotation * rotation;
                            car.transform.localScale *= scale;

                            car.transform.position = carPosition;
                            car.transform.rotation = carRotation;
                        }
                    }
                }
                car.SetActive(false);
                break;
            }
            yield return null;
        }
    }
}
