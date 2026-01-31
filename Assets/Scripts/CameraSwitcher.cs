using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Canvas room1Canvas;
    [SerializeField] private Canvas room2Canvas;
    [SerializeField] private Canvas room3Canvas;
    [SerializeField] private Canvas room4Canvas;
    [SerializeField] private Canvas room5Canvas;
    
    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCameraBase[] cameras;

    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Action Maps per Camera")]
    [SerializeField] private string[] actionMapNames; // np. ["Player", "Player Room 3", "Player Room 4"]

    private int currentCamIndex = 0;

    void Start()
    {
        if (cameras.Length != actionMapNames.Length)
            Debug.LogWarning("Liczba kamer i actionMapNames powinna być taka sama!");

        // ustawienie priorytetu początkowej kamery i włączonej Action Map
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == 0) ? 10 : 0;
        }

        if (playerInput != null && actionMapNames.Length > 0)
        {
            playerInput.SwitchCurrentActionMap(actionMapNames[0]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CamSwap(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CamSwap(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CamSwap(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) CamSwap(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) CamSwap(4);
    }

    private void CamSwap(int index)
    {
        if (index < 0 || index >= cameras.Length) return;

        // zmiana priorytetów kamer
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = (i == index) ? 10 : 0;

        // wyłącz poprzednią mapę
        if (playerInput != null && currentCamIndex < actionMapNames.Length)
        {
            var prevMap = playerInput.actions.FindActionMap(actionMapNames[currentCamIndex]);
            if (prevMap != null)
                prevMap.Disable();
        }

        // włącz nową mapę
        if (playerInput != null && index < actionMapNames.Length)
        {
            var nextMap = playerInput.actions.FindActionMap(actionMapNames[index]);
            if (nextMap != null)
                nextMap.Enable();
        }

        currentCamIndex = index;

        if (index == 0)
        {
            deactivateAll();
            room1Canvas.gameObject.SetActive(true);
        }

        if (index == 1)
        {
            deactivateAll();
            room2Canvas.gameObject.SetActive(true);
        }

        if (index == 2)
        {
            deactivateAll();
            room3Canvas.gameObject.SetActive(true);
        }

        if (index == 3)
        {
            deactivateAll();
            room4Canvas.gameObject.SetActive(true);
        }

        if (index == 4)
        {
            deactivateAll();
            room5Canvas.gameObject.SetActive(true);
        }
            
    }

    private void deactivateAll()
    {
        new Canvas[] { room1Canvas, room2Canvas, room3Canvas, room4Canvas, room5Canvas }
            .Where(c => c != null)
            .ToList()
            .ForEach(c => c.gameObject.SetActive(false));
    }
}