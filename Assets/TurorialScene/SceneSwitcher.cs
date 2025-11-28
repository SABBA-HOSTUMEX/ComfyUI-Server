using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneSwitcher : MonoBehaviour
{
    public Button ChangeScene;
    private void Start()
    {
        ChangeScene.onClick.AddListener(SwitchScene);
    }
    public void SwitchScene()
    {
        SceneManager.LoadScene("TurorialScene");
    }
}
