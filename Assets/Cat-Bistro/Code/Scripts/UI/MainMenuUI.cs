using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private OptionsUI optionUIPanel;


    private void Awake()
    {
        optionUIPanel.enabled = false;

        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.Demo);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }
}
