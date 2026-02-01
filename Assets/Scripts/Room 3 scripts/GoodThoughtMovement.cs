using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class GoodThoughtMovement : MonoBehaviour
{
    // Cel orbitowania (głowa gracza).
    [Header("Target")]
    private Transform focalPoint;

    [Header("Orbit")]
    [SerializeField] private float orbitSpeed = 30f; // stopnie na sekundę

    [Header("Vertical Sine")]
    [SerializeField] private float verticalAmplitude = 0.25f;
    [SerializeField] private float verticalDuration = 2f;

    [Header("Tween Settings")]
    public float flyDuration = 0.5f; // czas lotu do głowy
    public float scaleDuration = 0.3f; // czas zmniejszania
    public float endScale = 0.1f;

    [Header("Events")]
    public UnityEvent onCaught;

    // Offset i flagi stanu.
    private Vector3 offset;
    private bool hasBeenHit;
    private bool isFlyingToHead;

    private void OnEnable()
    {
        // Ustal cel przy każdym włączeniu obiektu.
        if (focalPoint == null)
        {
            GameObject fp = GameObject.FindWithTag("PlayerHead");
            if (fp != null) focalPoint = fp.transform;
        }

        // Reset stanu.
        hasBeenHit = false;
        isFlyingToHead = false;
        if (onCaught == null) onCaught = new UnityEvent();
        if (focalPoint == null) return;

        // Zachowujemy offset do orbitowania.
        offset = transform.position - focalPoint.position;

        StartVerticalSine();
    }

    private void Update()
    {
        // Zatrzymaj orbitę podczas lotu do głowy.
        if (focalPoint == null || isFlyingToHead) return;

        // Obrót offsetu wokół osi Y.
        offset = Quaternion.AngleAxis(
            orbitSpeed * Time.deltaTime,
            Vector3.up
        ) * offset;

        transform.position = focalPoint.position + offset;
    }

    private void StartVerticalSine()
    {
        // Delikatna oscylacja w pionie.
        transform
            .DOLocalMoveY(
                transform.localPosition.y + verticalAmplitude,
                verticalDuration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        // Sprzątanie tweenów po wyłączeniu obiektu.
        transform.DOKill();
    }

    public void OnHit()
    {
        // Zabezpieczenie przed wielokrotnym trafieniem.
        if (hasBeenHit) return;
        hasBeenHit = true;
        isFlyingToHead = true;

        // Powiadomienie o złapaniu zielonej myśli.
        onCaught?.Invoke();

        if (focalPoint == null)
        {
            Destroy(gameObject);
            return;
        }

        // Zatrzymaj oscylację i inne tweens.
        transform.DOKill();

        // Lot do głowy, potem zmniejszenie i zniszczenie.
        transform.DOMove(focalPoint.position, flyDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                transform.DOScale(endScale, scaleDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => Destroy(gameObject));
            });
    }
}

