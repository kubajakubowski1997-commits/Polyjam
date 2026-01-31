using DG.Tweening;
using UnityEngine;

public class BadThoughtProjectile : MonoBehaviour
{
    private GameObject targetObject; // cel lotu
    [SerializeField] private float speed = 3f;
    [SerializeField] private float arcHeight = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float journeyLength;
    private float startTime;

    [Header("Tween Settings")]
    public float scaleDuration = 0.5f; // czas animacji zmniejszania
    public float endScale = 0.1f;

    private bool hasBeenHit = false; // flaga zabezpieczająca

    private void OnEnable()
    {
        targetObject = GameObject.FindWithTag("PlayerHead");
        if (targetObject == null) return;

        startPos = transform.position;
        targetPos = targetObject.transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPos, targetPos);

        hasBeenHit = false;
        transform.localScale = Vector3.one; // reset skali
    }

    void Update()
    {
        if (targetObject == null || hasBeenHit) return; // zatrzymanie ruchu po trafieniu

        float distCovered = (Time.time - startTime) * speed;
        float fraction = distCovered / journeyLength;

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, fraction);
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * fraction);

        transform.position = currentPos;

        if (fraction >= 1f)
            Destroy(gameObject); // trafienie w head, niszcz obiekt
    }

    public void OnHit()
    {
        if (hasBeenHit) return; // zabezpieczenie, żeby wykonało się tylko raz
        hasBeenHit = true;

        // zatrzymujemy ruch w Update()
        // DOKill dla bezpieczeństwa, jeśli były jakieś tweens
        transform.DOKill();

        // --- Animacja zmniejszania ---
        transform.DOScale(endScale, scaleDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Destroy(gameObject); // niszcz po animacji
            });
    }
}
