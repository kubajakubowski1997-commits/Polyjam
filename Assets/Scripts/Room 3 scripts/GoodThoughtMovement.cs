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

    private Vector3 offset;

    private void Awake()
    {
        GameObject fp = GameObject.FindWithTag("PlayerHead");
        if (fp != null)
            focalPoint = fp.transform;
    }

    private void OnEnable()
    {
        if (focalPoint == null) return;

        // 🔑 KLUCZ: zachowujemy losowy spawn
        offset = transform.position - focalPoint.position;

        StartVerticalSine();
    }

    private void Update()
    {
        if (focalPoint == null) return;

        // ORBITA — tylko obrót offsetu
        offset = Quaternion.AngleAxis(
            orbitSpeed * Time.deltaTime,
            Vector3.up
        ) * offset;

        transform.position = focalPoint.position + offset;
    }

    private void StartVerticalSine()
    {
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
        Debug.Log("GoodThought trafiony: " + name);

        // Tutaj możesz dodać animację, efekt, dźwięk itp.

        // Na razie zniszczenie obiektu:
        Destroy(gameObject);
    }
}