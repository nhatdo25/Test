using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start Game
    public GameObject newGamePanel;
    public void PlayGame()
    {

        if (GameManager.Instance.HasSaveFile())
        {
            // Nếu đã có save -> bật panel hỏi
            newGamePanel.SetActive(true);
        }
        else
        {
            // Nếu chưa có -> đi chọn class
            SceneManager.LoadScene("SelectCharacter");
        }
    }
    public void ContinueGame()
    {
        // Load thẳng game (ví dụ Village scene)
        SceneManager.LoadScene("Village");
    }
    public void NewGame()
    {
        GameManager.Instance.DeleteSave();
        SceneManager.LoadScene("SelectCharacter");
    }
    public void CancelNewGame()
    {
        newGamePanel.SetActive(false);
    }
    // Settings
    public void OpenSettings()
    {
        // Có thể load sang scene Settings riêng
        // Hoặc bật panel trong MainMenu
        Debug.Log("Mở Settings...");
    }

    // Tutorial
    public void OpenTutorial()
    {
        // Có thể load scene tutorial riêng
        //SceneManager.LoadScene("Tutorial");
        Debug.Log("tutorial...");
    }

    // Quit
    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}
