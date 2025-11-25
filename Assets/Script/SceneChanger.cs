using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public Button button;
    public Button quit;

    public string SceneName;

    private void Start() {
        if(button != null)
            button.onClick.AddListener(ChangeScene);
        if(quit != null)
            quit.onClick.AddListener(CloseGame);
    }

    public void ChangeScene() {
        SceneManager.LoadScene(SceneName);
    }

    void CloseGame() {
        Application.Quit();
    }
}
