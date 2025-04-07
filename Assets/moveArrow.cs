using UnityEngine;

public class moveArrow : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    public Vector3 ofset;

    void Update()
    {
        // Follow position with offset
        transform.position = targetObject.position + ofset;

        // Get only Y-axis rotation from target
        float targetY = targetObject.eulerAngles.y;

        // Apply rotation only on Y axis
        transform.rotation = Quaternion.Euler(90f, targetY, 0f);
    }
}
