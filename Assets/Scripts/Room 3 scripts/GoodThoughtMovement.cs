using UnityEngine;
using DG.Tweening;

public class GoodThoughtMovement : MonoBehaviour
{
    [Header("Target")]
    private Transform focalPoint;

    [Header("Orbit")]
    [SerializeField] private float orbitSpeed = 30f; // stopnie na sekundę

    [Header("Vertical Sine")]
    [SerializeField] private float verticalAmplitude = 0.25f;
    [SerializeField] private float verticalDuration = 2f;
    
    [Header("Tween Settings")]
    public float flyDuration = 0.5f; // czas lotu do głowy
    public float scaleDuration = 0.3f; // czas znikania
    public float endScale = 0.1f;

    private Vector3 offset;
    private bool hasBeenHit;
    private bool isFlyingToHead;

    private void Awake()
    {
        GameObject fp = GameObject.FindWithTag("PlayerHead");
        if (fp != null)
            focalPoint = fp.transform;
    }

    private void OnEnable()
    {
        hasBeenHit = false;
        isFlyingToHead = false;
        if (focalPoint == null) return;

        offset = transform.position - focalPoint.position;

        StartVerticalSine();
    }

    private void Update()
    {
        if (focalPoint == null || isFlyingToHead) return; // zatrzymanie orbity podczas lotu

        // ORBITA — obrót offsetu
        offset = Quaternion.AngleAxis(
            orbitSpeed * Time.deltaTime,
            Vector3.up
        ) * offset;

        transform.position = focalPoint.position + offset;
    }

    private void StartVerticalSine()
    {
        transform.DOKill(); // usuń wcześniejsze tweeny
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
        transform.DOKill();
    }
    
    public void OnHit()
    {
        if (hasBeenHit) return;
        hasBeenHit = true;
        isFlyingToHead = true; // zatrzymuje orbitę

        Debug.Log("GoodThought trafiony: " + name);

        if (focalPoint == null)
        {
            Destroy(gameObject);
            return;
        }

        // zatrzymaj sinusoidę
        transform.DOKill();

        // --- Animacja lotu do głowy ---
        transform.DOMove(focalPoint.position, flyDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                // --- Animacja znikania / skalowania ---
                transform.DOScale(endScale, scaleDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
            });
    }
}
