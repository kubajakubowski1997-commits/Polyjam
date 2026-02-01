using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class RoomEntry
    {
        public string name;
        public int cameraIndex;
        public GameObject roomRoot;
        public MonoBehaviour[] resetBehaviours;
    }

    [Header("Refs")]
    public CameraSwitcher cameraSwitcher;

    [Header("Rooms")]
    public RoomEntry[] rooms;
    public float roomDuration = 10f;
    public bool avoidRepeat = true;

    [Header("Difficulty")]
    public float difficultyStart = 1f;
    public float difficultyIncreasePerSecond = 0.02f;
    public float difficultyMax = 3f;
    public bool useTimeScale = false;
    public float timeScaleBase = 1f;
    public float timeScalePerDifficulty = 0.3f;
    public float timeScaleMax = 2.5f;

    [Header("Game Over")]
    public UnityEvent onGameOver;
    [Tooltip("Opóźnienie przed Game Over (sekundy).")]
    public float gameOverDelay = 0.5f;

    public static float Difficulty { get; private set; } = 1f;

    int currentRoom = -1;
    float roomTimer;
    bool gameOver;
    bool gameOverPending;

    void Awake()
    {
        Difficulty = Mathf.Max(0.1f, difficultyStart);
    }

    void Start()
    {
        // Pick first room on start
        if (cameraSwitcher == null) cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        if (rooms != null && rooms.Length > 0)
        {
            SwitchToRandomRoom();
        }
    }

    void Update()
    {
        if (gameOver) return;

        // Global difficulty ramp
        Difficulty = Mathf.Min(difficultyMax, Difficulty + difficultyIncreasePerSecond * Time.deltaTime);
        if (useTimeScale)
        {
            float ts = timeScaleBase + Difficulty * timeScalePerDifficulty;
            Time.timeScale = Mathf.Clamp(ts, timeScaleBase, timeScaleMax);
        }

        // Room timer
        roomTimer += Time.deltaTime;
        if (roomTimer >= roomDuration)
        {
            roomTimer = 0f;
            ResetRoom(currentRoom);
            SwitchToRandomRoom();
        }
    }

    public void TriggerGameOver()
    {
        // Stop room switching and notify
        if (gameOver) return;
        gameOver = true;
        if (gameOverPending) return;
        gameOverPending = true;
        StartCoroutine(DelayedGameOver());
    }

    private System.Collections.IEnumerator DelayedGameOver()
    {
        float delay = Mathf.Max(0f, gameOverDelay);
        if (delay > 0f) yield return new WaitForSeconds(delay);
        DisableAllRooms();
        onGameOver?.Invoke();
    }

    void SwitchToRandomRoom()
    {
        // Random room, optional no-repeat
        if (rooms == null || rooms.Length == 0) return;

        int next = UnityEngine.Random.Range(0, rooms.Length);
        if (avoidRepeat && rooms.Length > 1)
        {
            int guard = 0;
            while (next == currentRoom && guard < 20)
            {
                next = UnityEngine.Random.Range(0, rooms.Length);
                guard++;
            }
        }

        currentRoom = next;
        roomTimer = 0f;

        // Reset the next room before it becomes active/visible.
        ResetRoom(currentRoom);
        SetActiveRoom(currentRoom);

        if (cameraSwitcher != null)
        {
            cameraSwitcher.SwitchTo(rooms[currentRoom].cameraIndex);
        }

    }

    void ResetRoom(int index)
    {
        // Call ResetRoom() on all behaviours that implement IRoomResettable
        if (rooms == null || index < 0 || index >= rooms.Length) return;
        RoomEntry room = rooms[index];
        if (room.resetBehaviours == null) return;

        for (int i = 0; i < room.resetBehaviours.Length; i++)
        {
            MonoBehaviour mb = room.resetBehaviours[i];
            if (mb is IRoomResettable r) r.ResetRoom();
        }
    }

    void SetActiveRoom(int index)
    {
        // Disable all rooms except active one
        if (rooms == null) return;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].roomRoot != null)
            {
                rooms[i].roomRoot.SetActive(i == index);
            }
        }
    }

    public void DisableAllRooms()
    {
        // Disable all room roots on game over.
        if (rooms == null) return;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].roomRoot != null)
            {
                rooms[i].roomRoot.SetActive(false);
            }
        }
    }

    public void RestartCurrentScene()
    {
        // Reload active scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        // Load first scene in build settings.
        SceneManager.LoadScene(0);
    }
}
