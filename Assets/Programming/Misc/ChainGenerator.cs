using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChainGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ChainSegment
    {
        public GameObject prefab;         // Prefab for this segment
        public Vector3 scale = Vector3.one;
        public float mass = 1f;           // Rigidbody mass
        public float jointSpring = 0f;    // Optional spring for hinge joint
        public float jointDamper = 0f;    // Optional damper for hinge joint
        public Vector3 initialOffset = Vector3.zero; // Offset at spawn relative to previous segment
    }

    [Header("Chain Setup")]
    public List<ChainSegment> segments = new List<ChainSegment>();

    [Header("Runtime Data")]
    public List<GameObject> segmentInstances = new List<GameObject>();

    [Header("Debug")]
    public bool drawGizmos = true;

    // ------------------------------------------------------------------------------------------
    // UNITY EVENT FUNCTIONS
    // ------------------------------------------------------------------------------------------

    private void Start()
    {
        RefreshChain();
    }

    private void OnEnable()
    {
        if (segmentInstances.Count == 0)
            RefreshChain();
        else
            SetChainActive(true);
    }

    private void OnDisable()
    {
        SetChainActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || segmentInstances.Count == 0) return;

        Gizmos.color = Color.yellow;
        Vector3 prevPos = transform.position;

        foreach (GameObject seg in segmentInstances)
        {
            if (seg == null) continue;
            Gizmos.DrawLine(prevPos, seg.transform.position);
            prevPos = seg.transform.position;
        }
    }

    // ------------------------------------------------------------------------------------------
    // CHAIN CREATION AND MANAGEMENT
    // ------------------------------------------------------------------------------------------

    public void RefreshChain()
    {
        ClearChain();

        Rigidbody previousRb = null;
        Transform previousAttach = null;

        for (int i = 0; i < segments.Count; i++)
        {
            ChainSegment segData = segments[i];
            if (segData.prefab == null)
            {
                Debug.LogWarning($"ChainGenerator: Segment {i} has no prefab assigned!");
                continue;
            }

            // Instantiate segment
            GameObject segObj = Instantiate(segData.prefab);
            segObj.name = $"ChainSegment_{i}";
            segObj.transform.localScale = segData.scale;

            // Auto-add Rigidbody
            Rigidbody rb = segObj.GetComponent<Rigidbody>();
            if (rb == null)
                rb = segObj.AddComponent<Rigidbody>();

            rb.mass = segData.mass;
            rb.angularDamping = 0.05f;
            rb.linearDamping = 0.05f;

            // Auto-add HingeJoint
            HingeJoint joint = segObj.GetComponent<HingeJoint>();
            if (joint == null)
                joint = segObj.AddComponent<HingeJoint>();

            // Root segment special setup
            if (i == 0)
            {
                rb.isKinematic = true; // Root is controlled manually
                previousRb = rb;
                previousAttach = segObj.transform.Find("AttachmentPoint") ?? segObj.transform;

                // Place root at generator position
                segObj.transform.position = transform.position;
            }
            else
            {
                rb.isKinematic = false;

                // Connect hinge to previous segment's Rigidbody
                joint.connectedBody = previousRb;

                // Determine previous attachment point
                Vector3 prevAttachPos = previousAttach != null ? previousAttach.position : previousRb.transform.position;

                // Set hinge anchor relative to this segment
                Transform myAttach = segObj.transform.Find("AttachmentPoint") ?? segObj.transform;
                joint.anchor = segObj.transform.InverseTransformPoint(myAttach.position);

                // Optional spring/damper
                JointSpring spring = joint.spring;
                spring.spring = segData.jointSpring;
                spring.damper = segData.jointDamper;
                joint.spring = spring;
                joint.useSpring = segData.jointSpring > 0;

                // Position segment using previous attachment point plus optional offset
                segObj.transform.position = prevAttachPos + segData.initialOffset;
            }

            segmentInstances.Add(segObj);

            // Update previous for next iteration
            previousRb = rb;
            previousAttach = segObj.transform.Find("AttachmentPoint") ?? segObj.transform;
        }
    }

    public void ClearChain()
    {
        for (int i = segmentInstances.Count - 1; i >= 0; i--)
        {
            if (segmentInstances[i] != null)
                Destroy(segmentInstances[i]);
        }
        segmentInstances.Clear();
    }

    private void SetChainActive(bool active)
    {
        foreach (GameObject seg in segmentInstances)
        {
            if (seg != null)
                seg.SetActive(active);
        }
    }
}