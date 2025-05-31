using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform playerTransform; // 👉 拖入 Player 的 Transform
    public PlateController[] allPlates; // 👉 拖入所有 Plate 的 PlateController

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z按下，找最近的盤子");

            PlateController nearestPlate = FindNearestPlate();
            if (nearestPlate == null)
            {
                Debug.Log("找不到盤子！");
                return;
            }

            Debug.Log("最近的盤子：" + nearestPlate.name);

            Order top = FindTopIngredientOnPlate(nearestPlate.transform);
            if (top != null)
            {
                Debug.Log("最上層食材：" + top.name);
                nearestPlate.CheckRecipeFromTop(top);
            }
            else
            {
                Debug.Log("盤子上沒有食材！");
            }
        }
    }

    private PlateController FindNearestPlate()
    {
        float minDistance = float.MaxValue;
        PlateController closestPlate = null;

        foreach (PlateController plate in allPlates)
        {
            float dist = Vector3.Distance(playerTransform.position, plate.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPlate = plate;
            }
        }

        return closestPlate;
    }

    private Order FindTopIngredientOnPlate(Transform plateTransform)
    {
        Vector3 center = plateTransform.position + Vector3.up * 0.5f;
        Collider[] colliders = Physics.OverlapBox(center, new Vector3(0.5f, 1f, 0.5f));

        Order top = null;
        float highestY = float.MinValue;

        foreach (Collider col in colliders)
        {
            Order ord = col.GetComponent<Order>();
            if (ord != null)
            {
                float y = col.transform.position.y;
                if (y > highestY)
                {
                    highestY = y;
                    top = ord;
                }
            }
        }

        return top;
    }
}
