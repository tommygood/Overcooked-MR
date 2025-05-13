using UnityEngine;

public class PanTrigger : MonoBehaviour
{
    public float cookTime = 3f;        // �N���ɶ�
    public float burnTime = 15f;       // �N�J�ɶ�

    private GameObject currentCube;
    private float timer = 0f;
    private bool isCooking = false;

    private Renderer cubeRenderer;
    private Color rawColor = Color.white;
    private Color cookedColor = new Color(0.6f, 0.3f, 0.1f);     // �Ħ�
    private Color burnedColor = new Color(0.2f, 0.1f, 0.05f);    // �`�Ŧ�

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cookable"))
        {
            currentCube = other.gameObject;
            cubeRenderer = currentCube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                rawColor = cubeRenderer.material.color; // �O����l�C��
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
                // �q��l�C�⺥�ܦ���
                float t = timer / cookTime;
                cubeRenderer.material.color = Color.Lerp(rawColor, cookedColor, t);
            }
            else if (timer >= cookTime && timer < burnTime)
            {
                // �q���ܿN�J
                float t = (timer - cookTime) / (burnTime - cookTime);
                cubeRenderer.material.color = Color.Lerp(cookedColor, burnedColor, t);
            }
            else if (timer >= burnTime)
            {
                // �N�J�����A�T�w���J�¦�
                cubeRenderer.material.color = burnedColor;
                isCooking = false; // ����[��
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
