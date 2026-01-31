using UnityEngine;

public class Room1ChairMovement : MonoBehaviour, IRoomResettable
{
    // Chair tilt based on left stick (UD). On fail, fall to fixed angle.
    [Header("Links")]
    public Room1Controls controls;
    public Transform pivot;

    [Header("Tilt")]
    public Vector3 tiltAxis = Vector3.right;
    public float maxTiltAngle = 12f;
    public float tiltSmoothing = 10f;

    [Header("Fail Fall")]
    public Vector3 fallAxis = Vector3.right;
    public float fallAngle = -90f;
    public float fallSmoothing = 6f;

    float currentAngle;
    bool fallen;
    Quaternion startRotation;

    void Start()
    {
        if (!pivot) pivot = transform;
        if (!controls) controls = FindObjectOfType<Room1Controls>();
        if (controls != null)
        {
            controls.onFail.AddListener(HandleFail);
        }
        startRotation = pivot ? pivot.localRotation : Quaternion.identity;
    }

    void Update()
    {
        Vector3 axis = tiltAxis.sqrMagnitude > 0.0001f ? tiltAxis.normalized : Vector3.right;
        if (!fallen)
        {
            // Normal tilt
            float input = controls ? controls.UDInput : 0f;
            float targetAngle = Mathf.Clamp(input, -1f, 1f) * maxTiltAngle;
            float t = 1f - Mathf.Exp(-tiltSmoothing * Time.deltaTime);
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, t);
            pivot.localRotation = Quaternion.AngleAxis(currentAngle, axis);
        }
        else
        {
            // Fall animation
            Vector3 fallN = fallAxis.sqrMagnitude > 0.0001f ? fallAxis.normalized : Vector3.right;
            float t = 1f - Mathf.Exp(-fallSmoothing * Time.deltaTime);
            currentAngle = Mathf.Lerp(currentAngle, fallAngle, t);
            pivot.localRotation = Quaternion.AngleAxis(currentAngle, fallN);
        }
    }

    void HandleFail()
    {
        if (fallen) return;
        fallen = true;
    }

    public void ResetRoom()
    {
        // Restore starting rotation
        fallen = false;
        currentAngle = 0f;
        if (pivot) pivot.localRotation = startRotation;
    }
}
