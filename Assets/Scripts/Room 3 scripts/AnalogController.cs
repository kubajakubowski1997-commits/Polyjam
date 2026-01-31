using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Room2ControlsUI
/// 
/// Sterowanie dwoma UI Image analogami gamepada:
/// - lewy analog -> cursorLeft
/// - prawy analog -> cursorRight
/// 
/// INPUT SYSTEM:
/// - Action Map: "Player Room 3"
/// - Actions:
///     - "leftStickMove"  (Vector2 / Left Stick)
///     - "rightStickMove" (Vector2 / Right Stick)
/// - PlayerInput -> Behavior: Send Messages
/// </summary>
public class AnalogController : MonoBehaviour
{
    [Header("Cursors")]
    public RectTransform cursorLeft;
    public RectTransform cursorRight;

    [Header("Movement")]
    [Tooltip("Prędkość ruchu w pikselach na sekundę.")]
    public float sideSpeed = 1000f;

    [Header("Input tuning")]
    public float deadzone = 0.15f;
    public float steerSmoothing = 12f;

    // Surowe wartości z gałek
    private Vector2 leftRaw;
    private Vector2 rightRaw;

    // Wartości po deadzone + smoothing
    private Vector2 leftSmooth;
    private Vector2 rightSmooth;

    // --- Input System: Send Messages ---
    // Lewy analog → akcja "leftStickMove"
    public void OnLeftStickMove(InputValue value)
    {
        leftRaw = value.Get<Vector2>();
    }

    // Prawy analog → akcja "rightStickMove"
    public void OnRightStickMove(InputValue value)
    {
        rightRaw = value.Get<Vector2>();
    }

    private void Update()
    {
        // --- Lewy analog ---
        if (cursorLeft != null)
        {
            Vector2 target = ApplyDeadzone(leftRaw);
            leftSmooth = ApplySmoothing(leftSmooth, target);
            MoveCursor(cursorLeft, leftSmooth);
        }

        // --- Prawy analog ---
        if (cursorRight != null)
        {
            Vector2 target = ApplyDeadzone(rightRaw);
            rightSmooth = ApplySmoothing(rightSmooth, target);
            MoveCursor(cursorRight, rightSmooth);
        }
    }

    // Deadzone
    private Vector2 ApplyDeadzone(Vector2 input)
    {
        Vector2 result = input;
        result.x = Mathf.Abs(input.x) < deadzone ? 0f : input.x;
        result.y = Mathf.Abs(input.y) < deadzone ? 0f : input.y;
        return result;
    }

    // Smoothing
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

    // Przesuwanie cursora
    private void MoveCursor(RectTransform cursor, Vector2 input)
    {
        Vector2 delta = input * sideSpeed * Time.deltaTime;
        cursor.anchoredPosition += delta;

        // ograniczenie do ekranu
        Vector2 clampedPos = cursor.anchoredPosition;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -Screen.width / 2f, Screen.width / 2f);
        clampedPos.y = Mathf.Clamp(clampedPos.y, -Screen.height / 2f, Screen.height / 2f);
        cursor.anchoredPosition = clampedPos;
    }
}
