using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    // Switches Cinemachine virtual cameras and room canvases.
    [SerializeField] private Canvas room1Canvas;
    [SerializeField] private Canvas room2Canvas;
    [SerializeField] private Canvas room3Canvas;
    [SerializeField] private Canvas room4Canvas;
    [SerializeField] private Canvas room5Canvas;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCameraBase[] cameras;

    [Header("Start")]
    [SerializeField] private bool autoInitialize = true;

    private int currentCamIndex = 0;

    void Start()
    {
        if (!autoInitialize) return;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == 0) ? 10 : 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchTo(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchTo(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchTo(4);
    }

    public void SwitchTo(int index)
    {
        if (index < 0 || index >= cameras.Length) return;

        // Only one camera has higher priority
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = (i == index) ? 10 : 0;

        currentCamIndex = index;

        if (index == 0)
        {
            DeactivateAll();
            room1Canvas.gameObject.SetActive(true);
        }

        if (index == 1)
        {
            DeactivateAll();
            room2Canvas.gameObject.SetActive(true);
        }

        if (index == 2)
        {
            DeactivateAll();
            room3Canvas.gameObject.SetActive(true);
        }

        if (index == 3)
        {
            DeactivateAll();
            room4Canvas.gameObject.SetActive(true);
        }

        if (index == 4)
        {
            DeactivateAll();
            room5Canvas.gameObject.SetActive(true);
        }
    }

    private void DeactivateAll()
    {
        // Disable all room canvases, then enable only active one
        new Canvas[] { room1Canvas, room2Canvas, room3Canvas, room4Canvas, room5Canvas }
            .Where(c => c != null)
            .ToList()
            .ForEach(c => c.gameObject.SetActive(false));
    }
}
