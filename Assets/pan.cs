using UnityEngine;

public class PanTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cookable_Beef"))
        {
            Rigidbody meatRb = other.GetComponent<Rigidbody>();
            if (meatRb != null && meatRb.GetComponent<FixedJoint>() == null)
            {
                FixedJoint joint = meatRb.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = GetComponentInParent<Rigidbody>(); // 鍋子的 Rigidbody
                joint.breakForce = 100; // 如果你想之後能移除這個 joint
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cookable_Beef"))
        {
            FixedJoint joint = other.GetComponent<FixedJoint>();
            if (joint != null)
            {
                Destroy(joint);
            }
        }
    }
}
