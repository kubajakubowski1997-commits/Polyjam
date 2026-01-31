using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Room1Controls : MonoBehaviour
{
    [Header("UI")]
    public Slider sliderUD;
    public Slider sliderLR;
    public RectTransform udCurrentMarker;
    public RectTransform lrCurrentMarker;
    public RectTransform udTargetWindow;
    public RectTransform lrTargetWindow;

    [Header("Gameplay")]
    public float tolerance = 0.07f;
    public float failGraceTime = 0.4f;
    public float targetAmplitude = 0.35f;
    public float targetSpeed = 0.12f;

    [Header("Input")]
    public float deadzone = 0.12f;
    public float inputSmoothing = 16f;
    public float inputScale = 0.8f;
    public float cursorSpeed = 0.9f;
    public bool invertUD = false;
    public bool invertLR = false;
    public bool autoInvertFromUI = true;

    float udRaw;
    float lrRaw;
    float ud;
    float lr;
    float udInputSmooth;
    float lrInputSmooth;
    float udTarget;
    float lrTarget;
    float udTargetSmooth;
    float lrTargetSmooth;
    float outsideTimer;
    float seedU;
    float seedL;

    public void OnBalanceUD(InputValue v) => udRaw = v.Get<float>();
    public void OnBalanceLR(InputValue v) => lrRaw = v.Get<float>();

    void Start()
    {
        seedU = Random.value * 1000f;
        seedL = Random.value * 1000f;

        if (autoInvertFromUI)
        {
            if (sliderUD)
            {
                float dotUp = Vector3.Dot(sliderUD.transform.up, Vector3.up);
                if (dotUp < 0f) invertUD = !invertUD;
            }

            if (sliderLR)
            {
                float dotRight = Vector3.Dot(sliderLR.transform.right, Vector3.right);
                if (dotRight < 0f) invertLR = !invertLR;
            }
        }
    }

    void Update()
    {
        float udD = Mathf.Abs(udRaw) < deadzone ? 0f : udRaw;
        float lrD = Mathf.Abs(lrRaw) < deadzone ? 0f : lrRaw;
        if (invertUD) udD = -udD;
        if (invertLR) lrD = -lrD;
        udD *= inputScale;
        lrD *= inputScale;

        if (inputSmoothing > 0f)
        {
            float t = 1f - Mathf.Exp(-inputSmoothing * Time.deltaTime);
            udInputSmooth = Mathf.Lerp(udInputSmooth, udD, t);
            lrInputSmooth = Mathf.Lerp(lrInputSmooth, lrD, t);
        }
        else
        {
            udInputSmooth = udD;
            lrInputSmooth = lrD;
        }

        ud = Mathf.Clamp(ud + udInputSmooth * cursorSpeed * Time.deltaTime, -1f, 1f);
        lr = Mathf.Clamp(lr + lrInputSmooth * cursorSpeed * Time.deltaTime, -1f, 1f);

        udTarget = (Mathf.PerlinNoise(seedU, Time.time * targetSpeed) * 2f - 1f) * targetAmplitude;
        lrTarget = (Mathf.PerlinNoise(seedL, Time.time * targetSpeed) * 2f - 1f) * targetAmplitude;

        float tt = 1f - Mathf.Exp(-6f * Time.deltaTime);
        udTargetSmooth = Mathf.Lerp(udTargetSmooth, udTarget, tt);
        lrTargetSmooth = Mathf.Lerp(lrTargetSmooth, lrTarget, tt);

        bool inside =
            Mathf.Abs(ud - udTargetSmooth) <= tolerance &&
            Mathf.Abs(lr - lrTargetSmooth) <= tolerance;

        outsideTimer = inside ? 0f : outsideTimer + Time.deltaTime;
        if (outsideTimer >= failGraceTime)
        {
            Debug.Log("FAIL: spadles z krzesla");
            outsideTimer = 0f;
        }

        PlaceMarker(udCurrentMarker, sliderUD, To01(ud));
        PlaceMarker(lrCurrentMarker, sliderLR, To01(lr));
        PlaceWindow(udTargetWindow, sliderUD, To01(udTargetSmooth), tolerance);
        PlaceWindow(lrTargetWindow, sliderLR, To01(lrTargetSmooth), tolerance);
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

    static void PlaceWindow(RectTransform window, Slider slider, float target01, float tol)
    {
        if (!window || !slider) return;

        RectTransform r = slider.GetComponent<RectTransform>();
        float tol01Half = tol * 0.5f;

        float min01 = Mathf.Clamp01(target01 - tol01Half);
        float max01 = Mathf.Clamp01(target01 + tol01Half);

        Vector2 pos = window.anchoredPosition;
        Vector2 size = window.sizeDelta;

        bool vertical =
            slider.direction == Slider.Direction.BottomToTop ||
            slider.direction == Slider.Direction.TopToBottom;

        if (vertical)
        {
            float h = r.rect.height;
            float yMin = Mathf.Lerp(-h * 0.5f, h * 0.5f, min01);
            float yMax = Mathf.Lerp(-h * 0.5f, h * 0.5f, max01);
            pos.y = (yMin + yMax) * 0.5f;
            size.y = Mathf.Abs(yMax - yMin);
        }
        else
        {
            float w = r.rect.width;
            float xMin = Mathf.Lerp(-w * 0.5f, w * 0.5f, min01);
            float xMax = Mathf.Lerp(-w * 0.5f, w * 0.5f, max01);
            pos.x = (xMin + xMax) * 0.5f;
            size.x = Mathf.Abs(xMax - xMin);
        }

        window.anchoredPosition = pos;
        window.sizeDelta = size;
    }
}
