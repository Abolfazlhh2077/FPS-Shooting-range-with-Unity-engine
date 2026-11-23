using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsPanel;
    public GameObject playButton;
    public GameObject controlsButton;
    public GameObject exitButton;
    

    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowControls()
    {
        playButton.SetActive(false);
        controlsButton.SetActive(false);
        exitButton.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        playButton.SetActive(true);
        controlsButton.SetActive(true);
        exitButton.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}