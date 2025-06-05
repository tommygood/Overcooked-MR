using UnityEngine;

public class Order : MonoBehaviour
{
    public Order belowIngredient;

    private void Update()
    {
        UpdateBelowIngredient();
    }

    private void UpdateBelowIngredient()
    {
        // Raycast 向下偵測
        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,                            
            new Vector3(0.05f, 0, 0.05f),            
            new Vector3(-0.05f, 0, 0.05f),           
            new Vector3(0.05f, 0, -0.05f),           
            new Vector3(-0.05f, 0, -0.05f)           
        };

        /*
        foreach (Vector3 offset in offsets)
        {
            Vector3 origin = transform.position + offset + Vector3.up * 0.01f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.2f))
            {
                if (TryDetectBelow(hit.collider)) return;
            }
        }
        */

        if (Physics.BoxCast(
            transform.position + Vector3.up * 0.01f,
            Vector3.one * 0.2f,
            Vector3.down,
            out RaycastHit hit,
            Quaternion.identity,
            maxDistance: 0.2f
        ))
        {
            if (TryDetectBelow(hit.collider)) return;
        }

        // 補一次 OverlapSphere 檢查
        Vector3 center = transform.position + Vector3.down * 0.05f;
        float radius = 0.06f;
        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (Collider col in hits)
        {
            if (TryDetectBelow(col)) return;
        }

        belowIngredient = null;
    }

    private bool TryDetectBelow(Collider col)
    {
        Debug.Log("[Order] Detect obj name: " + col.name);
        Order lower = col.GetComponent<Order>();
        if (lower != null && lower != this)
        {
            belowIngredient = lower;
            return true;
        }

        // 底部結束
        if (col.CompareTag("Plate") || col.CompareTag("Bowl"))
        {
            belowIngredient = null;
            return true;
        }

        return false;
    }

    // 可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + Vector3.down * 0.05f;
        Gizmos.DrawWireSphere(center, 0.06f);

        Gizmos.color = Color.red;
        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(0.05f, 0, 0.05f),
            new Vector3(-0.05f, 0, 0.05f),
            new Vector3(0.05f, 0, -0.05f),
            new Vector3(-0.05f, 0, -0.05f)
        };
        foreach (Vector3 offset in offsets)
        {
            Vector3 origin = transform.position + offset + Vector3.up * 0.01f;
            Gizmos.DrawRay(origin, Vector3.down * 0.2f);
        }
    }
}
