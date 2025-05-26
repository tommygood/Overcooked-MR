using UnityEngine;

public class Order : MonoBehaviour
{
    public Order belowIngredient;

    public void UpdateBelowReference()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1f))
        {
            Order below = hitInfo.collider.GetComponent<Order>();
            if (below != null && below != this)
            {
                belowIngredient = below;
                Debug.Log($"{gameObject.name} 疊在 {below.gameObject.name} 上");
            }
        }
    }
}
