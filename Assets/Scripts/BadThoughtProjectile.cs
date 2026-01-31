using UnityEngine;

public class BadThoughtProjectile : MonoBehaviour
{
    private GameObject targetObject; // teraz możesz przypisać pusty GameObject
    [SerializeField] private float speed = 10f;
    [SerializeField] private float arcHeight = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float journeyLength;
    private float startTime;

    private void OnEnable()
    {
        targetObject = GameObject.FindWithTag("PlayerHead");
        if (targetObject == null) return;

        startPos = transform.position;
        targetPos = targetObject.transform.position; // pobieramy Transform z GameObject
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPos, targetPos);
    }

    void Update()
    {
        if (targetObject == null) return;

        float distCovered = (Time.time - startTime) * speed;
        float fraction = distCovered / journeyLength;

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, fraction);
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * fraction);

        transform.position = currentPos;

        if (fraction >= 1f)
            Destroy(gameObject);
    }

    public void OnHit()
    {
        Debug.Log("GoodThought trafiony: " + name);

        // Tutaj możesz dodać animację, efekt, dźwięk itp.

        // Na razie zniszczenie obiektu:
        Destroy(gameObject);
    }
}
