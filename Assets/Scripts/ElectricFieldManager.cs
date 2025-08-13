using UnityEngine;

public class ElectricFieldManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 20;
    public float gridSpacing = 1.0f;

    [Header("Field Settings")]
    public float coulombConstant = 8.987f;
    public float vectorMagnitudeMultiplier = 0.1f;

    [Header("Color Gradient")]
    public Gradient colorGradient;
    public float maxMagnitudeForColor = 100f;

    void OnDrawGizmos()
    {
        if (PointCharge.allCharges == null) return;

        Vector3 startPos = transform.position -
                           new Vector3(gridSize * gridSpacing / 2f,
                                       gridSize * gridSpacing / 2f, 0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 gridPoint = new Vector2(
                    startPos.x + x * gridSpacing,
                    startPos.y + y * gridSpacing
                );

                // --- NEW: Check if the point is inside any charge ---
                bool isInsideACharge = false;
                foreach (PointCharge charge in PointCharge.allCharges)
                {
                    float distanceToCharge = Vector2.Distance(gridPoint, charge.transform.position);
                    if (distanceToCharge < charge.radius)
                    {
                        isInsideACharge = true;
                        break; // Found one, no need to check others
                    }
                }

                // If it's inside, skip calculation and drawing for this point
                if (isInsideACharge)
                {
                    continue; // Go to the next point in the grid
                }
                // --- End of new logic ---

                Vector2 netElectricField = CalculateFieldAtPoint(gridPoint);

                float fieldMagnitude = netElectricField.magnitude;
                float normalizedMagnitude = Mathf.Clamp01(fieldMagnitude / maxMagnitudeForColor);
                Color fieldColor = colorGradient.Evaluate(normalizedMagnitude);

                DrawFieldVector(gridPoint, netElectricField, fieldColor);
            }
        }
    }

    Vector2 CalculateFieldAtPoint(Vector2 point)
    {
        Vector2 cumulativeField = Vector2.zero;

        foreach (PointCharge charge in PointCharge.allCharges)
        {
            Vector2 chargePos = charge.transform.position;
            Vector2 direction = point - chargePos;
            float distance = direction.magnitude;

            if (distance < 0.01f) continue;

            float magnitude = (coulombConstant * charge.charge) / (distance * distance);
            cumulativeField += direction.normalized * magnitude;
        }
        return cumulativeField;
    }

    void DrawFieldVector(Vector2 point, Vector2 field, Color vectorColor)
    {
        Gizmos.color = vectorColor;
        Vector2 endPoint = point + field.normalized * vectorMagnitudeMultiplier;
        Gizmos.DrawLine(point, endPoint);
    }
}
