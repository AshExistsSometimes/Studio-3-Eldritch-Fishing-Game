using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Harpoon : MonoBehaviour
{
    [Tooltip("Damage dealt when harpoon tip hits an IDamagable.")]
    public float Damage = 10f;

    [Tooltip("Time (seconds) after sticking before harpoon destroys itself.")]
    public float DespawnTime = 5f;

    [HideInInspector]
    public string IgnoredTag = "Boat"; // the tag to ignore for parenting/damage

    private bool stuck = false;

    // Called by tip trigger when it collides with something.
    // The tip's trigger collider should call this (e.g., via an OnTriggerEnter on a child).
    public void OnTriggerEnter(Collider col)
    {
        if (stuck) return;
        if (col == null) return;

        Transform hitTransform = col.transform;

        // If the collider is part of a rigidbody structure (like a serpent link), use the attached rigidbody's transform root for parenting
        if (col.attachedRigidbody != null)
            hitTransform = col.attachedRigidbody.transform;

        // Ignore hitting the boat
        if (!string.IsNullOrEmpty(IgnoredTag) && hitTransform.CompareTag(IgnoredTag))
            return;

        // Parent the harpoon to the hit transform so it appears stuck
        transform.SetParent(hitTransform, worldPositionStays: true);

        // Attempt to deal damage
        IDamagable dmg = hitTransform.GetComponentInParent<IDamagable>();
        if (dmg != null)
        {
            dmg.TakeDamage(Damage);
        }

        // Stop physics motion if any
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        stuck = true;
        StartCoroutine(DespawnAfterDelay());
    }

    /// <summary>Destroy harpoon after DespawnTime seconds.</summary>
    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(DespawnTime);
        Destroy(gameObject);
    }
}

