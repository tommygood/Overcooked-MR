using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FireManager : MonoBehaviour
{
    public float fireInterval = 15f;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.fireInterval);

            StoveControl[] furnaces = FindObjectsByType<StoveControl>(FindObjectsSortMode.None);

            if (furnaces.Length == 0)
            {
                yield return null;
                continue;
            }

            int index = Random.Range(0, furnaces.Length);
            furnaces[index].StartFire();
        }
    }
}
