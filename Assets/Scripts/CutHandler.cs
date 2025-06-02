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

    public int cut_num_need = 5;
    private int cut_num_current = 0;

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
        cutMappings.Add(new CutMapping("Apple", Resources.Load<GameObject>("Apple_cut")));
        cutMappings.Add(new CutMapping("Carrot", Resources.Load<GameObject>("Carrot_cut")));
        cutMappings.Add(new CutMapping("Lettuce", Resources.Load<GameObject>("Lettuce_cut")));
        cutMappings.Add(new CutMapping("Steak", Resources.Load<GameObject>("Steak_cut")));
        cutMappings.Add(new CutMapping("Turkey", Resources.Load<GameObject>("Turkey_cut")));
        cutMappings.Add(new CutMapping("Tomato", Resources.Load<GameObject>("Tomato_cut")));
    }

    void Update()
    {   
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
        }
        else
        {
            Debug.LogWarning($"No cutPrefab mapping found for object: {gameObject.name}");
        }

        Destroy(gameObject);
    }

    private GameObject GetCutPrefabByName()
    {
        foreach (var mapping in cutMappings)
        {
            if (gameObject.name.ToLower().Contains(mapping.originalName.ToLower()))
            {
                return mapping.cutPrefab;
            }
        }
        return null;
    }
}
