using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase[] cameras;
    private int currentCamIndex = 0;
    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == 0) ? 10 : 0;
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

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == index) ? 10 : 0;
        }

        currentCamIndex = index;
    }
}
