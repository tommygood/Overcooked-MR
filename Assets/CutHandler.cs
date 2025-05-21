using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "LeftHand")
        {
            Debug.Log("Left hand has entered the trigger!");
            // You can add more logic here for what should happen when LeftHand enters the trigger
        }
        else
        {
            Debug.Log("QQ" + other.name);
        }
    }
}
