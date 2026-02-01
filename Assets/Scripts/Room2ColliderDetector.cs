using UnityEngine;
using UnityEngine.Events;

public class Room2ColliderDetector : MonoBehaviour
{
    // Tag obiektów, które mają powodować porażkę.
    [Tooltip("Tag obiektów przeszkód w Room2.")]
    public string obstacleTag = "Obstacle";

    // Referencja do GameManagera w scenie (ustaw w Inspectorze).
    [Tooltip("Referencja do GameManager w scenie.")]
    public GameManager gameManager;

    [Header("Fail")]
    public UnityEvent onFail;

    // Zabezpieczenie przed wielokrotnym triggerem.
    private bool triggered;

    private void Awake()
    {
        // Upewnij się, że event nie jest null.
        if (onFail == null) onFail = new UnityEvent();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Obsługa kolizji nie-trigger.
        TryGameOver(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Obsługa kolizji trigger.
        TryGameOver(other);
    }

    private void TryGameOver(Collider other)
    {
        // Guardy na duplikaty lub brak collidera.
        if (triggered) return;
        if (other == null) return;

        // Reagujemy tylko na przeszkody z odpowiednim tagiem.
        if (!other.CompareTag(obstacleTag)) return;

        triggered = true;

        // Powiadomienie listenerów (np. UI).
        onFail?.Invoke();

        if (gameManager == null)
        {
            Debug.LogWarning("Room2ColliderDetector: GameManager not assigned in inspector.");
            return;
        }

        // Globalny game over.
        gameManager.TriggerGameOver();
    }
}

