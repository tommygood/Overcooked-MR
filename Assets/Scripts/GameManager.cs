using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform playerTransform;
    public int foodId = 56;

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Z))
    {
        Debug.Log("Z按下，找最近的盤子或碗");

        PlateController nearestPlate = FindNearestPlateOrBowl();
        if (nearestPlate == null)
        {
            Debug.Log("找不到盤子或碗！");
            return;
        }


        Order top = FindTopIngredientOnPlate(nearestPlate.transform);
        if (top != null)
        {   
            // 印出堆疊內容
            Debug.Log("堆疊內容：");
            Order current = top;
            int level = 1;
            while (current != null)
            {
                Debug.Log($"  {level}：{current.tag}");
                current = current.belowIngredient;
                level++;
            }

            bool isCorrect = nearestPlate.CheckRecipeFromTop(top, foodId);
            Debug.Log("是否正確組合：" + isCorrect);
        }
        else
        {
            Debug.Log("上面沒有食材！");
        }
    } 
} 

    private PlateController FindNearestPlateOrBowl()
{
    GameObject[] plates = GameObject.FindGameObjectsWithTag("Plate");
    //GameObject[] bowls = GameObject.FindGameObjectsWithTag("Bowl");

    float minDistance = float.MaxValue;
    PlateController closest = null;

    foreach (GameObject obj in plates)
    {
        TryUpdateClosest(obj, ref minDistance, ref closest);
    }

    //foreach (GameObject obj in bowls)
    //{
    //    TryUpdateClosest(obj, ref minDistance, ref closest);
    //}

    return closest;
}


   private void TryUpdateClosest(GameObject obj, ref float minDistance, ref PlateController closest)
{

    PlateController controller = obj.GetComponent<PlateController>();
    if (controller == null)
    {
        return;
    }

    float dist = Vector3.Distance(playerTransform.position, obj.transform.position);

    if (dist < minDistance)
    {
        minDistance = dist;
        closest = controller;
    }
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
