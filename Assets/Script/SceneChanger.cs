using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public Button button;
    public string SceneName;

    private void Start() {
        button.onClick.AddListener(ChangeScene);
    }

    private void ChangeScene() {
        SceneManager.LoadScene(SceneName);
    }
}
