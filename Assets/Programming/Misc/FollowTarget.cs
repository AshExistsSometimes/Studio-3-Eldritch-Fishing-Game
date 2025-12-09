using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Tooltip("The GameObject this object should follow on the X and Z axes.")]
    public GameObject followTarget;

    [Tooltip("The fixed Y height this object should maintain.")]
    public float ObjectHeight = 5f;

    private void LateUpdate()
    {
        if (followTarget == null) return;

        Vector3 targetPos = followTarget.transform.position;
        transform.position = new Vector3(targetPos.x, ObjectHeight, targetPos.z);
    }
}
