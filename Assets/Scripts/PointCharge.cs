using System.Collections.Generic;
using UnityEngine;

public class PointCharge : MonoBehaviour
{
    public float charge = 1.0f;
    // NEW: Add a radius to define the charge's size.
    public float radius = 0.5f;

    public static List<PointCharge> allCharges = new List<PointCharge>();

    void OnEnable()
    {
        if (!allCharges.Contains(this))
        {
            allCharges.Add(this);
        }
    }

    void OnDisable()
    {
        allCharges.Remove(this);
    }

    // NEW: This will draw a helpful circle in the editor to show the radius.
    // It only draws when the object is selected.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
