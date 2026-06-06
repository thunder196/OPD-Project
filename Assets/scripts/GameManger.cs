using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;
    public GameObject ShopPanel;
    public GameObject ModeMenuPanel;
    private string match3SceneName = "Match3Game";
    private string bubleBlastName = "BubbleBlast";

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    { 
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        ShopPanel.SetActive(false);
        ModeMenuPanel.SetActive(false);
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
        ModeMenuPanel.SetActive(false);
    }

    public void OpenModeMenu()
    {
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        ShopPanel.SetActive(false);
        ModeMenuPanel.SetActive(true);
    }

    public void LaunchMathc3()
    {
        SceneManager.LoadScene(match3SceneName);
    }

    public void LaunchBubbleBlast()
    {
        SceneManager.LoadScene(bubleBlastName);
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
