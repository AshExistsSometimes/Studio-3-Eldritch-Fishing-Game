using System.Collections.Generic;
using UnityEngine;

public class ChainGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ChainSegment
    {
        public GameObject prefab;    // Prefab to spawn for this segment
        public Vector3 scale = Vector3.one;
        public float followSpeed = 5f; // Speed at which this segment follows the previous
    }

    [Header("Chain Setup")]
    public List<ChainSegment> segments = new List<ChainSegment>();

    [Header("Chain Behavior")]
    public bool facePreviousSegment = true;

    [Header("Runtime Data (Read Only)")]
    public List<Transform> segmentInstances = new List<Transform>();

    [Header("Debug")]
    public bool drawGizmos = true;

    void Start()
    {
        RefreshChain();
    }

    void Update()
    {
        UpdateChainPositions();
    }

    public void RefreshChain()
    {
        ClearChain();

        for (int i = 0; i < segments.Count; i++)
        {
            ChainSegment segData = segments[i];
            if (segData.prefab == null)
            {
                Debug.LogWarning($"Segment {i} has no prefab assigned!");
                continue;
            }

            GameObject segObj = Instantiate(segData.prefab);
            segObj.name = $"ChainSegment_{i}";
            segObj.transform.localScale = segData.scale;
            segObj.transform.position = transform.position;

            segmentInstances.Add(segObj.transform);
        }
    }

    public void ClearChain()
    {
        for (int i = segmentInstances.Count - 1; i >= 0; i--)
        {
            if (segmentInstances[i] != null)
                Destroy(segmentInstances[i].gameObject);
        }
        segmentInstances.Clear();
    }

    private void UpdateChainPositions()
    {
        if (segmentInstances.Count == 0) return;

        for (int i = 0; i < segmentInstances.Count; i++)
        {
            Transform seg = segmentInstances[i];
            Vector3 targetPos;

            if (i == 0)
            {
                // First segment follows the root
                targetPos = transform.position;
            }
            else
            {
                Transform prevSeg = segmentInstances[i - 1];
                Transform attachPoint = prevSeg.Find("AttachmentPoint");
                targetPos = (attachPoint != null) ? attachPoint.position : prevSeg.position;
            }

            // Smoothly move toward target position
            float speed = segments[Mathf.Clamp(i, 0, segments.Count - 1)].followSpeed;
            seg.position = Vector3.MoveTowards(seg.position, targetPos, speed * Time.deltaTime);

            if (facePreviousSegment)
            {
                Vector3 lookTarget = (i == 0) ? transform.position : segmentInstances[i - 1].position;
                if ((lookTarget - seg.position).sqrMagnitude > 0.0001f)
                    seg.rotation = Quaternion.LookRotation(lookTarget - seg.position, Vector3.up);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !drawGizmos) return;
        if (segmentInstances.Count == 0) return;

        Gizmos.color = Color.yellow;
        Vector3 prevPos = transform.position;

        foreach (Transform seg in segmentInstances)
        {
            if (seg == null) continue;
            Gizmos.DrawLine(prevPos, seg.position);
            prevPos = seg.position;
        }
    }
}
