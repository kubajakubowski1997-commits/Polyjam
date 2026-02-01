using UnityEngine;
using UnityEngine.Events;

public class Room3FailTracker : MonoBehaviour, IRoomResettable
{
    [Header("Fail Rules")]
    [Tooltip("Ile złych myśli musi dolecieć do głowy, aby rozważać porażkę.")]
    public int badThoughtsToFail = 3;

    [Tooltip("Jak długo trwa room, zanim sprawdzimy warunek porażki.")]
    public float roomDuration = 10f;

    [Header("Refs")]
    [Tooltip("Opcjonalnie: wywołuje globalne Game Over przy porażce.")]
    public GameManager gameManager;

    [Header("Fail")]
    public UnityEvent onFail;

    // Stan runtime.
    private int badArrived;
    private bool goodCaught;
    private float timer;
    private bool failTriggered;
    private bool evaluationDone;

    private void Awake()
    {
        // Upewnij się, że event nie jest null.
        if (onFail == null) onFail = new UnityEvent();
    }

    private void Update()
    {
        // Odliczanie czasu pokoju.
        timer += Time.deltaTime;
        if (timer >= roomDuration && !evaluationDone)
        {
            evaluationDone = true;
            TryFail();
        }
    }

    public void RegisterBadArrived()
    {
        // Wołane przez złe myśli, które dotarły do głowy.
        badArrived++;
    }

    public void RegisterGoodCaught()
    {
        // Wołane, gdy gracz złapie zieloną myśl.
        goodCaught = true;
    }

    private void TryFail()
    {
        if (failTriggered) return;
        if (badArrived < badThoughtsToFail) return;
        if (goodCaught) return;

        failTriggered = true;
        onFail?.Invoke();
        if (gameManager != null)
        {
            gameManager.TriggerGameOver();
        }
    }

    public void ResetRoom()
    {
        // Reset stanu po aktywacji pokoju.
        badArrived = 0;
        goodCaught = false;
        timer = 0f;
        failTriggered = false;
        evaluationDone = false;
    }
}

