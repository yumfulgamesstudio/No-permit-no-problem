using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI instance {  get; private set; }

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeValue;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI sfxVolumeValue;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        instance = this;
        
        backButton.onClick.AddListener(() =>
        {
            Hide();
        });

        sfxVolumeSlider.onValueChanged.AddListener((float value) =>
        {
            SoundManager.Instance.ChangeVolume(value);
            UpdateVisual();
        });
        musicVolumeSlider.onValueChanged.AddListener((float value) =>
        {
            MusicManager.instance.ChangeVolume(value);
            UpdateVisual();
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        sfxVolumeSlider.value = SoundManager.Instance.GetVolume();
        musicVolumeSlider.value = MusicManager.instance.GetVolume();

        UpdateVisual();
        
        Hide();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateVisual()
    {
        sfxVolumeValue.text = Mathf.Round(SoundManager.Instance.GetVolume() * 100f) + "%";

        musicVolumeValue.text = Mathf.Round(MusicManager.instance.GetVolume() * 100f) + "%";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}