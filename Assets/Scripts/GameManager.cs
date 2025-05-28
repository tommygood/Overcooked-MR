using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlateController plate; // 拖入含有 PlateController 的盤子物件

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z被按下");

            Order top = FindTopIngredientOnPlate();
            if (top != null)
            {
                Debug.Log("最上層食材：" + top.name);
                plate.CheckRecipeFromTop(top);
            }
            else
            {
                Debug.Log("未放置任何食材！");
            }
        }
    }

    private Order FindTopIngredientOnPlate()
    {
        // 根據盤子位置搜尋周圍區域的食材（要設定 plate transform）
        Vector3 center = plate.transform.position + Vector3.up * 0.5f;
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
