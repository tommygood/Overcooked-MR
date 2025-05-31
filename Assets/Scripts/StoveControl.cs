using System.Collections;
using UnityEngine;

public class StoveControl : MonoBehaviour
{
    public GameObject fireEffectPrefab;
    private GameObject currentFire;
    private bool isOnFire = false;
    public bool canMove = true;

    private float waterContactTime = 0f;
    private bool inContactWithWater = false;

    public void StartFire()
    {
        if (isOnFire)
        {
            Debug.Log("火已經存在，不重複產生");
            return;
        }

        if (fireEffectPrefab == null)
        {
            Debug.LogError("fireEffectPrefab未設");
            return;
        }

        Debug.Log("StartFire() 被呼叫");
        currentFire = Instantiate(fireEffectPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform);
        isOnFire = true;
        canMove = false;
    }

    private void Update()
    {
        if (isOnFire && inContactWithWater)
        {
            waterContactTime += Time.deltaTime;

            if (waterContactTime >= 3f)
            {
                Debug.Log("水接觸達3秒，熄火");
                ExtinguishFire();
            }
        }
        else
        {
            waterContactTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOnFire && other.CompareTag("Water_T"))
        {
            Debug.Log("碰到Water");
            inContactWithWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("離開Water特效");
            inContactWithWater = false;
        }
    }

    void ExtinguishFire()
    {
        Debug.Log("火已熄滅");
        if (currentFire)
            Destroy(currentFire);

        isOnFire = false;
        canMove = true;
        waterContactTime = 0f;
        inContactWithWater = false;
    }
}
