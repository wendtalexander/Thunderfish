using System.Collections.Generic;
using UnityEngine;

public class ElectricOrgan : MonoBehaviour
{
    [Header("Organ Configuration")]
    public GameObject pointChargePrefab; // The prefab for the individual charges
    public int chargeCount = 10;
    public float organLength = 4.0f;
    public float baseChargeValue = 50.0f;
    public float chargeRadius = 0.1f;

    // This function is called when the script is loaded or a value is changed in the inspector
    void OnValidate()
    {
        // Use UnityEditor extensions to run this in the editor without playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += _GenerateCharges;
#endif
    }

    void Awake()
    {
        _GenerateCharges();
    }

    private void _GenerateCharges()
    {
        // This check prevents errors if the script is part of a prefab instance
        if (this == null || gameObject == null || !gameObject.scene.isLoaded) return;

        ClearExistingCharges();

        if (pointChargePrefab == null || chargeCount <= 1) return;

        for (int i = 0; i < chargeCount; i++)
        {
            // Calculate the position for this charge along the local x-axis
            float position = Mathf.Lerp(-organLength / 2f, organLength / 2f, (float)i / (chargeCount - 1));
            Vector3 localPos = new Vector3(position, 0, 0);

            // Create a new instance of the charge prefab as a child of this object
            GameObject chargeGO = Instantiate(pointChargePrefab, transform);
            chargeGO.transform.localPosition = localPos;

            PointCharge pointCharge = chargeGO.GetComponent<PointCharge>();
            if (pointCharge != null)
            {
                // Create a simple dipole: positive head, negative tail
                if (i == chargeCount - 1) // The "head"
                {
                    pointCharge.charge = baseChargeValue;
                }
                else if (i == 0) // The "tail"
                {
                    pointCharge.charge = -baseChargeValue;
                }
                else // Charges in the middle are neutral
                {
                    pointCharge.charge = 0;
                }
                pointCharge.radius = chargeRadius;
            }
        }
    }

    private void ClearExistingCharges()
    {
        // Destroy any previously generated charges to avoid duplicates
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        foreach (Transform child in children)
        {
            // Use DestroyImmediate for editor functionality
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }
}
