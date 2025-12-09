using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [System.Serializable]
    public class CompassTarget
    {
        public GameObject prefab;          // Prefab used only for matching name
        public Color needleColor = Color.white;
    }

    [Header("Compass Settings")]
    public List<CompassTarget> potentialTargets = new List<CompassTarget>();

    [Tooltip("Needle object that rotates (the arrow model).")]
    public Transform Needle;

    [Tooltip("Image for tinting the needle (if the needle uses UI/Image). Optional.")]
    public Image NeedleImage;

    [Tooltip("Renderer if the needle is a 3D mesh. Optional.")]
    public Renderer NeedleRenderer;

    [Tooltip("How fast the needle turns.")]
    public float lookDampening = 5f;

    private Transform bestTarget;

    private const float minX = 0f;
    private const float maxX = 0f;
    private const float minY = -180f;
    private const float maxY = 180f;

    private void LateUpdate()
    {
        UpdateTarget();
        RotateTowardTarget();
    }

    // ---------------------------------------------------------
    // Find closest matching target in the scene
    // ---------------------------------------------------------
    private void UpdateTarget()
    {
        float closestDist = Mathf.Infinity;
        bestTarget = null;

        foreach (var entry in potentialTargets)
        {
            if (entry.prefab == null) continue;

            string targetName = entry.prefab.name;

            // Find ANY object in the scene with that name
            GameObject found = GameObject.Find(targetName);
            if (found == null) continue;

            float dist = Vector3.Distance(transform.position, found.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = found.transform;

                ApplyNeedleColor(entry.needleColor);
            }
        }
    }

    // ---------------------------------------------------------
    // Color the needle through UI or Renderer
    // ---------------------------------------------------------
    private void ApplyNeedleColor(Color c)
    {
        if (NeedleImage != null)
            NeedleImage.color = c;

        if (NeedleRenderer != null)
        {
            foreach (var mat in NeedleRenderer.materials)
                mat.color = c;
        }
    }

    // ---------------------------------------------------------
    // Rotate the compass toward the chosen target
    // ---------------------------------------------------------
    private void RotateTowardTarget()
    {
        if (Needle == null || bestTarget == null)
            return;

        Vector3 dir = bestTarget.position - Needle.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        // Extract Euler angles
        Vector3 euler = targetRot.eulerAngles;

        // Lock X / Z EXACTLY as required
        euler.x = Mathf.Clamp(0f, minX, maxX);
        euler.z = 0f;

        // Clamp Y
        float y = euler.y;
        if (y > 180f) y -= 360f;
        y = Mathf.Clamp(y, minY, maxY);
        euler.y = y;

        Quaternion finalRot = Quaternion.Euler(euler);

        Needle.rotation = Quaternion.Slerp(
            Needle.rotation,
            finalRot,
            Time.deltaTime * lookDampening
        );
    }
}
