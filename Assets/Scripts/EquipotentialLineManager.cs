
using System.Collections.Generic;
using UnityEngine;

public class EquipotentialLineManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 40; // A finer grid is better for smooth lines
    public float gridSpacing = 0.5f;

    [Header("Potential Settings")]
    public float coulombConstant = 8.987f;
    public Color lineColor = Color.green;
    // The specific potential values for which we will draw lines
    public List<float> potentialLevels = new List<float> { -20, -10, -5, 5, 10, 20 };

    private float[,] potentials; // 2D array to store the potential at each grid point

    void OnDrawGizmos()
    {
        if (PointCharge.allCharges == null) return;

        // Initialize the grid
        potentials = new float[gridSize + 1, gridSize + 1];
        Vector2 startPos = (Vector2)transform.position -
                           new Vector2(gridSize * gridSpacing / 2f,
                                       gridSize * gridSpacing / 2f);

        // 1. Calculate the potential at every grid point
        for (int x = 0; x <= gridSize; x++)
        {
            for (int y = 0; y <= gridSize; y++)
            {
                Vector2 point = startPos + new Vector2(x * gridSpacing, y * gridSpacing);
                potentials[x, y] = CalculatePotentialAtPoint(point);
            }
        }

        // 2. Draw the contour lines for each potential level
        Gizmos.color = lineColor;
        foreach (float level in potentialLevels)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    // Get the four corners of the current cell
                    Vector2 p1 = startPos + new Vector2(x * gridSpacing, y * gridSpacing);
                    Vector2 p2 = startPos + new Vector2((x + 1) * gridSpacing, y * gridSpacing);
                    Vector2 p3 = startPos + new Vector2((x + 1) * gridSpacing, (y + 1) * gridSpacing);
                    Vector2 p4 = startPos + new Vector2(x * gridSpacing, (y + 1) * gridSpacing);

                    // Get the potential at the four corners
                    float v1 = potentials[x, y];
                    float v2 = potentials[x + 1, y];
                    float v3 = potentials[x + 1, y + 1];
                    float v4 = potentials[x, y + 1];

                    DrawContourLine(level, p1, p2, p3, p4, v1, v2, v3, v4);
                }
            }
        }
    }

    float CalculatePotentialAtPoint(Vector2 point)
    {
        float cumulativePotential = 0;
        foreach (PointCharge charge in PointCharge.allCharges)
        {
            float distance = Vector2.Distance(point, charge.transform.position);

            // IMPORTANT: If inside a charge, potential is constant and equal to surface potential
            if (distance < charge.radius)
            {
                distance = charge.radius;
            }

            // V = k * q / r
            cumulativePotential += (coulombConstant * charge.charge) / distance;
        }
        return cumulativePotential;
    }

    void DrawContourLine(float level, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float v1, float v2, float v3, float v4)
    {
        List<Vector2> intersectionPoints = new List<Vector2>();

        // Check Top Edge (p4 -> p3)
        if ((level > v4 && level < v3) || (level < v4 && level > v3))
            intersectionPoints.Add(Vector2.Lerp(p4, p3, Mathf.InverseLerp(v4, v3, level)));
        // Check Bottom Edge (p1 -> p2)
        if ((level > v1 && level < v2) || (level < v1 && level > v2))
            intersectionPoints.Add(Vector2.Lerp(p1, p2, Mathf.InverseLerp(v1, v2, level)));
        // Check Left Edge (p1 -> p4)
        if ((level > v1 && level < v4) || (level < v1 && level > v4))
            intersectionPoints.Add(Vector2.Lerp(p1, p4, Mathf.InverseLerp(v1, v4, level)));
        // Check Right Edge (p2 -> p3)
        if ((level > v2 && level < v3) || (level < v2 && level > v3))
            intersectionPoints.Add(Vector2.Lerp(p2, p3, Mathf.InverseLerp(v2, v3, level)));

        // A cell will typically have 0 or 2 intersection points.
        if (intersectionPoints.Count >= 2)
        {
            Gizmos.DrawLine(intersectionPoints[0], intersectionPoints[1]);
        }
        // A rare saddle point case can have 4.
        if (intersectionPoints.Count >= 4)
        {
            Gizmos.DrawLine(intersectionPoints[2], intersectionPoints[3]);
        }
    }
}
