using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("test");
    }

    public void QuitGame()
    {
        Application.Quit();
        
        Debug.Log("Gra została zamknięta");
    }
}