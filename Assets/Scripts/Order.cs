using UnityEngine;

public class Order : MonoBehaviour
{
    public Order belowIngredient;

    void Update()
    {
        RaycastHit hit;

        // 向下發射短距離 Ray，確認下面是否有 Order 或盤子
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f))
        {
            Order lower = hit.collider.GetComponent<Order>();
            if (lower != null)
            {
                belowIngredient = lower;
                return;
            }

            if (hit.collider.CompareTag("Plate"))
            {
                belowIngredient = null; // 下面是盤子，不是食材
                return;
            }
        }

        // 沒有東西在下面，重設
        belowIngredient = null;
    }
}
