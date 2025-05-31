using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public List<StoveControl> furnaces;
    public float fireInterval = 15f;

    private void Start()
    {
        StartCoroutine(RandomFireRoutine());
    }

    IEnumerator RandomFireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);

            if (furnaces.Count > 0)
            {
                int index = Random.Range(0, furnaces.Count);
                furnaces[index].StartFire();
            }
        }
    }
}
