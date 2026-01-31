using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Room1Controls : MonoBehaviour, IRoomResettable
{
    // Room1: two sliders. Red = player, green = target. Fail when no overlap on both axes.
    [Header("UI")]
    public RectTransform udCurrentMarker;
    public RectTransform lrCurrentMarker;
    public RectTransform udTargetWindow;
    public RectTransform lrTargetWindow;
    public Slider sliderUD;
    public Slider sliderLR;

    [Header("Target Movement")]
    public float targetAmplitude = 0.35f;
    public float targetSpeed = 0.2f;
    public float targetChangeInterval = 0.25f;

    [Header("Input")]
    public float deadzone = 0.12f;
    public float inputScale = 1f;
    public float cursorSpeed = 0.9f;
    public bool useDifficultyScaling = true;

    [Header("Fail")]
    public float failGraceTime = 0.4f;
    public UnityEvent onFail;

    float udRaw;
    float lrRaw;
    float ud;
    float lr;
    float udTarget;
    float lrTarget;
    float targetTimer;
    Vector2 targetVel;
    float outsideTimer;

    public float UDInput => udRaw;

    public void OnBalanceUD(InputValue v) => udRaw = v.Get<float>();
    public void OnBalanceLR(InputValue v) => lrRaw = v.Get<float>();

    void Awake()
    {
        if (onFail == null) onFail = new UnityEvent();
    }

    void Start()
    {
        targetTimer = Random.Range(0.05f, targetChangeInterval);
    }

    void Update()
    {
        // Input with "memory": stick moves marker, release keeps last position.
        // Input -> przesuwanie z „pamięcią” (nie wraca do środka)
        float udD = Mathf.Abs(udRaw) < deadzone ? 0f : udRaw;
        float lrD = Mathf.Abs(lrRaw) < deadzone ? 0f : lrRaw;
        float diff = useDifficultyScaling ? GameManager.Difficulty : 1f;
        float moveSpeed = cursorSpeed * diff;
        ud += udD * inputScale * moveSpeed * Time.deltaTime;
        lr += lrD * inputScale * moveSpeed * Time.deltaTime;
        ud = Mathf.Clamp(ud, -1f, 1f);
        lr = Mathf.Clamp(lr, -1f, 1f);

        // Target: slow random drift (can change direction any time).
        float targetSpeedScaled = useDifficultyScaling ? targetSpeed * diff : targetSpeed;
        if (targetSpeedScaled <= 0f)
        {
            targetVel = Vector2.zero;
        }
        else
        {
            targetTimer -= Time.deltaTime;
            if (targetTimer <= 0f)
            {
                Vector2 jitter = Random.insideUnitCircle * targetSpeedScaled;
                targetVel += jitter;
                if (targetVel.magnitude > targetSpeedScaled)
                {
                    targetVel = targetVel.normalized * targetSpeedScaled;
                }
                targetTimer = targetChangeInterval;
            }

            udTarget += targetVel.x * Time.deltaTime;
            lrTarget += targetVel.y * Time.deltaTime;
        }

        if (targetAmplitude <= 0f)
        {
            udTarget = 0f;
            lrTarget = 0f;
            targetVel = Vector2.zero;
        }
        else if (udTarget < -targetAmplitude)
        {
            udTarget = -targetAmplitude;
            targetVel.x = Mathf.Abs(targetVel.x);
        }
        else if (udTarget > targetAmplitude)
        {
            udTarget = targetAmplitude;
            targetVel.x = -Mathf.Abs(targetVel.x);
        }

        if (lrTarget < -targetAmplitude)
        {
            lrTarget = -targetAmplitude;
            targetVel.y = Mathf.Abs(targetVel.y);
        }
        else if (lrTarget > targetAmplitude)
        {
            lrTarget = targetAmplitude;
            targetVel.y = -Mathf.Abs(targetVel.y);
        }

        // UI positions
        PlaceMarker(udCurrentMarker, sliderUD, To01(ud));
        PlaceMarker(lrCurrentMarker, sliderLR, To01(lr));
        PlaceMarker(udTargetWindow, sliderUD, To01(udTarget));
        PlaceMarker(lrTargetWindow, sliderLR, To01(lrTarget));

        // Overlap on each axis (UD and LR separately)
        bool insideUD = OverlapsAxisBounds(udCurrentMarker, udTargetWindow, sliderUD);
        bool insideLR = OverlapsAxisBounds(lrCurrentMarker, lrTargetWindow, sliderLR);
        bool inside = insideUD && insideLR;

        outsideTimer = inside ? 0f : outsideTimer + Time.deltaTime;
        if (outsideTimer >= failGraceTime)
        {
            outsideTimer = 0f;
            Debug.Log("[Room1] FAIL");
            onFail.Invoke();
        }
    }

    public void ResetRoom()
    {
        // Reset all state to start
        udRaw = 0f;
        lrRaw = 0f;
        ud = 0f;
        lr = 0f;
        udTarget = 0f;
        lrTarget = 0f;
        targetVel = Vector2.zero;
        targetTimer = Random.Range(0.05f, targetChangeInterval);
        outsideTimer = 0f;
    }

    static float To01(float v) => (v + 1f) * 0.5f;

    static void PlaceMarker(RectTransform marker, Slider slider, float value01)
    {
        if (!marker || !slider) return;

        RectTransform r = slider.GetComponent<RectTransform>();
        Vector2 pos = marker.anchoredPosition;

        bool vertical =
            slider.direction == Slider.Direction.BottomToTop ||
            slider.direction == Slider.Direction.TopToBottom;

        if (vertical)
        {
            float h = r.rect.height;
            pos.y = Mathf.Lerp(-h * 0.5f, h * 0.5f, value01);
        }
        else
        {
            float w = r.rect.width;
            pos.x = Mathf.Lerp(-w * 0.5f, w * 0.5f, value01);
        }

        marker.anchoredPosition = pos;
    }

    static bool OverlapsAxisBounds(RectTransform current, RectTransform target, Slider slider)
    {
        if (!current || !target || !slider) return true;

        bool vertical =
            slider.direction == Slider.Direction.BottomToTop ||
            slider.direction == Slider.Direction.TopToBottom;

        Bounds cur = RectTransformUtility.CalculateRelativeRectTransformBounds(slider.transform, current);
        Bounds tar = RectTransformUtility.CalculateRelativeRectTransformBounds(slider.transform, target);

        float curMin = vertical ? cur.min.y : cur.min.x;
        float curMax = vertical ? cur.max.y : cur.max.x;
        float tarMin = vertical ? tar.min.y : tar.min.x;
        float tarMax = vertical ? tar.max.y : tar.max.x;

        return curMax >= tarMin && tarMax >= curMin;
    }
}
