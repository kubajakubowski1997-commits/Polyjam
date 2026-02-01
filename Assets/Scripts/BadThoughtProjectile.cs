using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class BadThoughtProjectile : MonoBehaviour
{
    // Cel (głowa gracza), do którego leci obiekt.
    private GameObject targetObject;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float arcHeight = 2f;

    // Dane ruchu w locie.
    private Vector3 startPos;
    private Vector3 targetPos;
    private float journeyLength;
    private float startTime;

    [Header("Tween Settings")]
    public float scaleDuration = 0.5f;
    public float endScale = 0.1f;

    [Header("Events")]
    public UnityEvent onReachedHead;

    // Blokada wielokrotnego wywołania hit / dotarcia do celu.
    private bool hasBeenHit;
    private bool reachTriggered;

    private void OnEnable()
    {
        // Szukamy głowy gracza przy każdym włączeniu obiektu.
        targetObject = GameObject.FindWithTag("PlayerHead");
        if (targetObject == null) return;

        // Inicjalizacja ruchu w stronę celu.
        startPos = transform.position;
        targetPos = targetObject.transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPos, targetPos);

        // Reset stanu na reuse.
        hasBeenHit = false;
        reachTriggered = false;
        if (onReachedHead == null) onReachedHead = new UnityEvent();
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        // Zatrzymaj ruch jeśli nie ma celu lub został trafiony.
        if (targetObject == null || hasBeenHit) return;

        float distCovered = (Time.time - startTime) * speed;
        float fraction = distCovered / journeyLength;

        // Ruch po łuku w stronę głowy.
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, fraction);
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * fraction);
        transform.position = currentPos;

        // Dotarcie do głowy.
        if (fraction >= 1f)
        {
            if (!reachTriggered)
            {
                reachTriggered = true;
                onReachedHead?.Invoke();
            }
            Destroy(gameObject);
        }
    }

    public void OnHit()
    {
        // Zabezpieczenie przed wielokrotnym trafieniem.
        if (hasBeenHit) return;
        hasBeenHit = true;

        // Zatrzymaj tweens i zmniejsz obiekt.
        transform.DOKill();
        transform.DOScale(endScale, scaleDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}

