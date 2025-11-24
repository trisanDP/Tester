using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public Button button;
    public Button quit;

    public string SceneName;

    private void Start() {
        button.onClick.AddListener(ChangeScene);
        if(quit != null)
            quit.onClick.AddListener(CloseGame);
    }

    private void ChangeScene() {
        SceneManager.LoadScene(SceneName);
    }

    void CloseGame() {
        Application.Quit();
    }
}
