using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Room2Controls
/// 
/// Runner:
/// - obiekt ZAWSZE porusza się do przodu (oś Z)
/// - gracz steruje tylko w bok (oś X) gałką
/// 
/// INPUT SYSTEM:
/// - Action Map: "Player"
/// - Action: "Steer" (Value / Axis)
/// - Binding: Gamepad Left Stick X
/// - PlayerInput -> Behavior: Send Messages
/// 
/// PlayerInput AUTOMATYCZNIE wywoła:
///     OnSteer(InputValue value)
/// bo akcja nazywa się "Steer"
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Room2Controls : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Stała prędkość do przodu (Z).")]
    public float forwardSpeed = 6f;

    [Tooltip("Maksymalna prędkość w bok (X) przy pełnym wychyleniu gałki.")]
    public float sideSpeed = 4f;

    [Header("Input tuning")]
    [Tooltip("Martwa strefa gałki, żeby nie driftowała.")]
    public float deadzone = 0.15f;

    [Tooltip("Wygładzanie steru. 0 = brak, większe = płynniej.")]
    public float steerSmoothing = 12f;

    private Rigidbody rb;

    // Surowa wartość z gałki (-1..1)
    private float steerRaw;

    // Wartość po deadzone + smoothing
    private float steerSmooth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Żeby fizyka nie przewracała obiektu
        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Opcjonalnie dla płynności (możesz też ustawić w Inspectorze)
        // rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    /// <summary>
    /// Odbiera input z akcji "Steer".
    /// Wywoływane AUTOMATYCZNIE przez PlayerInput (Send Messages).
    /// </summary>
    public void OnSteer(InputValue value)
    {
        steerRaw = value.Get<float>();
    }

    private void FixedUpdate()
    {
        // 1) Deadzone – małe wartości traktujemy jak 0
        float s = Mathf.Abs(steerRaw) < deadzone ? 0f : steerRaw;

        // 2) Wygładzanie – daje "lekko / mocno" + płynność
        if (steerSmoothing > 0f)
        {
            float t = 1f - Mathf.Exp(-steerSmoothing * Time.fixedDeltaTime);
            steerSmooth = Mathf.Lerp(steerSmooth, s, t);
        }
        else
        {
            steerSmooth = s;
        }

        // 3) Ruch: stały przód + bok zależny od wychylenia gałki
        Vector3 velocity = rb.linearVelocity;
        velocity.z = forwardSpeed;
        velocity.x = steerSmooth * sideSpeed;
        rb.linearVelocity = velocity;

        // Jeśli skręca w złą stronę:
        // velocity.x = -steerSmooth * sideSpeed;
    }
}
