using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AnalogController : MonoBehaviour
{
    [Header("Cursors")]
    public RectTransform cursorLeft;
    public RectTransform cursorRight;

    [Header("Movement")]
    public float sideSpeed = 1000f;

    [Header("Input tuning")]
    public float deadzone = 0.15f;
    public float steerSmoothing = 12f;

    private Vector2 leftRaw;
    private Vector2 rightRaw;

    private Vector2 leftSmooth;
    private Vector2 rightSmooth;

    // --- Input System ---
    public void OnLeftStickMove(InputValue value)
    {
        leftRaw = value.Get<Vector2>();
    }

    public void OnRightStickMove(InputValue value)
    {
        rightRaw = value.Get<Vector2>();
    }

    // --- Shooting ---
    public float maxDistance = 50f;

    private void Update()
    {
        Camera activeCam = Camera.main; // FIX: użyj Camera.main lub podmień na aktywną Cinemachine

        // --- Lewy analog ---
        if (cursorLeft != null)
        {
            Vector2 target = ApplyDeadzone(leftRaw);
            leftSmooth = ApplySmoothing(leftSmooth, target);
            MoveCursor(cursorLeft, leftSmooth);

            if (Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame)
            {
                GameObject hitObj = ShootFromCrosshair(cursorLeft, "GoodThought", activeCam); // FIX
                if (hitObj != null)
                {
                    Debug.Log("Left Trigger hit: " + hitObj.name);

                    // FIX: bezpieczne wywołanie OnHit
                    var good = hitObj.GetComponent<GoodThoughtMovement>();
                    if (good != null) good.OnHit();
                }
            }
        }
        
        // --- Prawy analog ---
        if (cursorRight != null)
        {
            Vector2 target = ApplyDeadzone(rightRaw);
            rightSmooth = ApplySmoothing(rightSmooth, target);
            MoveCursor(cursorRight, rightSmooth);

            // Prawy trigger Fire2 → strzela tylko do BadThought
            if (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame)
            {
                // FIX: strzał do BadThought
                GameObject hitObj = ShootFromCrosshair(cursorRight, "BadThought", Camera.main);
                if (hitObj != null)
                {
                    Debug.Log("Right Trigger hit: " + hitObj.name);

                    // Bezpieczne wywołanie OnHit
                    var bad = hitObj.GetComponent<BadThoughtProjectile>();
                    if (bad != null) bad.OnHit();
                }
            }
        }

    }

    private Vector2 ApplyDeadzone(Vector2 input)
    {
        Vector2 result = input;
        result.x = Mathf.Abs(input.x) < deadzone ? 0f : input.x;
        result.y = Mathf.Abs(input.y) < deadzone ? 0f : input.y;
        return result;
    }

    private Vector2 ApplySmoothing(Vector2 current, Vector2 target)
    {
        if (steerSmoothing > 0f)
        {
            float t = 1f - Mathf.Exp(-steerSmoothing * Time.deltaTime);
            return Vector2.Lerp(current, target, t);
        }
        else
        {
            return target;
        }
    }

    private void MoveCursor(RectTransform cursor, Vector2 input)
    {
        Vector2 delta = input * sideSpeed * Time.deltaTime;
        cursor.anchoredPosition += delta;

        Vector2 clampedPos = cursor.anchoredPosition;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -Screen.width / 2f, Screen.width / 2f);
        clampedPos.y = Mathf.Clamp(clampedPos.y, -Screen.height / 2f, Screen.height / 2f);
        cursor.anchoredPosition = clampedPos;
    }

    // FIX: dodany parametr Camera do raycast
    private GameObject ShootFromCrosshair(RectTransform crosshair, string targetName, Camera cam)
    {
        if (cam == null || crosshair == null) return null;

        Vector3 screenPos = crosshair.position;
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider != null && hit.collider.name.Contains(targetName))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }
}
