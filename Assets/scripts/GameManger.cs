using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;
    public GameObject ShopPanel;
    private string match3SceneName = "Match3Game";

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    { 
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        ShopPanel.SetActive(false);
    }

    public void OpenSetting()
    {
        SettingsPanel.SetActive(true);
    }

    public void OpenShop()
    {
        MainMenuPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ShopPanel.SetActive(true);
    }

    public void LaunchMathc3()
    {
        SceneManager.LoadScene(match3SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }
}
