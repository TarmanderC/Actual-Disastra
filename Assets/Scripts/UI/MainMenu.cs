using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame() {
        Application.Quit();
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            QuitGame();
        }
    }
}
