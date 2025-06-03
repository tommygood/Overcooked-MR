using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutMapping
{
    public string originalName;
    public GameObject cutPrefab;

    public CutMapping(string name, GameObject prefab)
    {
        originalName = name;
        cutPrefab = prefab;
    }
}

public class CutHandler : MonoBehaviour
{
    private GameObject LeftHandAnchor;
    private bool is_cutter_gesture = false;
    public float cutter_duration = 5f;
    private float cutter_duration_passed = 0f;

    public int cut_num_need = 1;
    private int cut_num_current = 0;

    public int cut_delay = 2; // delay for next cut detection
    private float cut_delay_passed = 2f;

    //[Header("Cut Mapping Settings")]
    //public List<CutMapping> cutMappings;
    private List<CutMapping> cutMappings = new List<CutMapping>();

    void Start()
    {
        // Find the left hand anchor by name at the start
        LeftHandAnchor = GameObject.Find("LeftHandAnchor");
        if (LeftHandAnchor == null)
        {
            Debug.LogError("LeftHandAnchor not found! Make sure it's named correctly in the scene.");
        }
        cutMappings.Add(new CutMapping("Apple", Resources.Load<GameObject>("Prefabs/Apple_cut")));
        cutMappings.Add(new CutMapping("Carrot", Resources.Load<GameObject>("Prefabs/Carrot_cut")));
        cutMappings.Add(new CutMapping("Lettuce", Resources.Load<GameObject>("Prefabs/Lettuce_cut")));
        cutMappings.Add(new CutMapping("Steak", Resources.Load<GameObject>("Prefabs/Steak_cut")));
        cutMappings.Add(new CutMapping("Turkey", Resources.Load<GameObject>("Prefabs/Turkey_cut")));
        cutMappings.Add(new CutMapping("Tomato", Resources.Load<GameObject>("Prefabs/Tomato_cut")));
    }

    void Update()
    {
        if (cut_delay_passed < cut_delay)
        {
            cut_delay_passed += Time.deltaTime;
            return; // Wait for the delay before checking gestures
        }

        // If for some reason it's destroyed or not yet found, try again
        if (LeftHandAnchor == null)
        {
            LeftHandAnchor = GameObject.Find("LeftHandAnchor");
            return;
        }
        // Get Z rotation in degrees
        float zRotation = LeftHandAnchor.transform.rotation.eulerAngles.z;

        // Normalize angle to [0, 360)
        if (zRotation > 180f) zRotation -= 360f;

        // Check if within gesture threshold (between 80 and 100 degrees)
        if (zRotation >= 80f && zRotation <= 100f)
        {
            is_cutter_gesture = true;
            cutter_duration_passed = 0f;
        }

        // If in cutter gesture, track duration
        if (is_cutter_gesture)
        {
            cutter_duration_passed += Time.deltaTime;

            if (cutter_duration_passed > cutter_duration)
            {
                is_cutter_gesture = false;
                cutter_duration_passed = 0f;
            }

            if (cut_delay_passed >= cut_delay)
            {
                cut_delay_passed = 0f; // Reset delay after gesture detected
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "TipCollider" || other.name == "FingerTrackingHandCollider")
        {
            Debug.Log("Left hand has entered the trigger!");

            if (is_cutter_gesture)
            {
                Debug.Log("Cutter Detection !!!");

                // Get the world contact point and convert it to local space
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                Vector3 localPoint = transform.InverseTransformPoint(contactPoint);

                float totalWidth = transform.localScale.x;

                // Flip logic: if hit on the left, remove left side; if right, remove right
                bool isLeftSide = localPoint.x < 0;


                // How much to keep
                float cutRatio = Mathf.Clamp01(Mathf.Abs(localPoint.x) / (0.5f * totalWidth));
                float newWidth = totalWidth * cutRatio;

                if (newWidth > 0.01f)
                {
                    Vector3 newScale = transform.localScale;
                    newScale.x = newWidth;
                    transform.localScale = newScale;

                    Vector3 shiftDirection = isLeftSide ? transform.right : -transform.right;
                    float shiftAmount = (totalWidth - newWidth) * 0.5f;
                    transform.position += shiftDirection * shiftAmount;

                    Debug.Log($"Cut from {(isLeftSide ? "left" : "right")} at localX={localPoint.x:F3}, new width={newWidth:F3}");
                    cut_num_current += 1;
                }

                if (cut_num_current >= cut_num_need)
                {
                    Debug.Log("Meet the need for cutting number ! Enable the cut model.");
                    ReplaceWithCutObject();
                }
            }
        }
    }

    private void ReplaceWithCutObject()
    {
        GameObject cutPrefab = GetCutPrefabByName();
        if (cutPrefab != null)
        {
            Instantiate(cutPrefab, transform.position, transform.rotation);
            Debug.Log("replace with cutPrefab: " + cutPrefab.name);
        }
        else
        {
            Debug.LogWarning($"No cutPrefab mapping found for object: {gameObject.name}");
        }

        Destroy(gameObject);
        Debug.Log("CutHandler: Object replaced with cut prefab and original destroyed.");
    }

    private GameObject GetCutPrefabByName()
    {
        foreach (var mapping in cutMappings)
        {
            if (gameObject.name.ToLower().Contains(mapping.originalName.ToLower()))
            {
                Debug.Log($"Found cut mapping for {gameObject.name}: {mapping.cutPrefab.name}");
                return mapping.cutPrefab;
            }
        }
        return null;
    }
}
