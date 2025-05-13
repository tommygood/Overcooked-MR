using UnityEngine;

public class PanTrigger : MonoBehaviour
{
    public float cookTime = 3f;        // 煮熟時間
    public float burnTime = 15f;       // 燒焦時間

    private GameObject currentCube;
    private float timer = 0f;
    private bool isCooking = false;

    private Renderer cubeRenderer;
    private Color rawColor = Color.white;
    private Color cookedColor = new Color(0.6f, 0.3f, 0.1f);     // 棕色
    private Color burnedColor = new Color(0.2f, 0.1f, 0.05f);    // 深褐色

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cookable"))
        {
            currentCube = other.gameObject;
            cubeRenderer = currentCube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                rawColor = cubeRenderer.material.color; // 記錄原始顏色
            }

            timer = 0f;
            isCooking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentCube)
        {
            isCooking = false;
            timer = 0f;
            currentCube = null;
            cubeRenderer = null;
        }
    }

    void Update()
    {
        if (isCooking && currentCube != null && cubeRenderer != null)
        {
            timer += Time.deltaTime;

            if (timer < cookTime)
            {
                // 從原始顏色漸變成熟
                float t = timer / cookTime;
                cubeRenderer.material.color = Color.Lerp(rawColor, cookedColor, t);
            }
            else if (timer >= cookTime && timer < burnTime)
            {
                // 從熟變燒焦
                float t = (timer - cookTime) / (burnTime - cookTime);
                cubeRenderer.material.color = Color.Lerp(cookedColor, burnedColor, t);
            }
            else if (timer >= burnTime)
            {
                // 燒焦完成，固定為焦黑色
                cubeRenderer.material.color = burnedColor;
                isCooking = false; // 停止加熱
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (GetComponent<Collider>() != null)
        {
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
        }
    }
}
