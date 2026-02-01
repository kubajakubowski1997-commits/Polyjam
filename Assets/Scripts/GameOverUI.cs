using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [Header("Enable Targets")]
    [Tooltip("Komponenty do w³¹czenia na Game Over (np. Canvas, CanvasGroup, MonoBehaviour).")]
    public Behaviour[] enableBehaviours;

    [Tooltip("Obiekty do aktywacji na Game Over.")]
    public GameObject[] enableObjects;

    public void Show()
    {
        if (enableBehaviours != null)
        {
            for (int i = 0; i < enableBehaviours.Length; i++)
            {
                if (enableBehaviours[i] != null)
                    enableBehaviours[i].enabled = true;
            }
        }

        if (enableObjects != null)
        {
            for (int i = 0; i < enableObjects.Length; i++)
            {
                if (enableObjects[i] != null)
                    enableObjects[i].SetActive(true);
            }
        }
    }
}
