using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject settingsMenuPanel;

    [Header("Sliders")]
    [SerializeField] private Slider scrollSensitivitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    [Header("Slider Text")]
    [SerializeField] private TMP_Text scrollSensitivityText;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;

    [Header("Keybind Text")]
    [SerializeField] private TMP_Text leftKeyText;
    [SerializeField] private TMP_Text rightKeyText;
    [SerializeField] private TMP_Text jumpKeyText;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [Header("Camera")]
    [SerializeField] private GameObject SecondaryCamera;

    public static bool SettingsOpen { get; private set; }

    public static float ScrollSensitivity { get; private set; } = 1f;

    public static KeyCode LeftKey { get; private set; } = KeyCode.A;
    public static KeyCode RightKey { get; private set; } = KeyCode.D;
    public static KeyCode JumpKey { get; private set; } = KeyCode.Space;

    private string keyToRebind = "";

    private void Start()
    {
        LoadSettings();

        if (settingsMenuPanel != null)
            settingsMenuPanel.SetActive(false);

        if (scrollSensitivitySlider != null)
        {
            scrollSensitivitySlider.minValue = 0.2f;
            scrollSensitivitySlider.maxValue = 3f;
            scrollSensitivitySlider.value = ScrollSensitivity;
            scrollSensitivitySlider.onValueChanged.AddListener(SetScrollSensitivity);
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = AudioListener.volume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = GetMusicVolume();
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        UpdateTexts();
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(keyToRebind))
        {
            ListenForNewKey();
            return;
        }

        if (SettingsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    public void OpenSettings()
    {
        if (SettingsOpen)
        {
            CloseSettings(); return;
        }
        SettingsOpen = true;
        if (SecondaryCamera != null)
        {
            SecondaryCamera.SetActive(false);
        }
        
        if (settingsMenuPanel != null)
            settingsMenuPanel.SetActive(true);


    }

    public void CloseSettings()
    {
        SettingsOpen = false;
        keyToRebind = "";
        if (SecondaryCamera != null)
        {
            SecondaryCamera.SetActive(true);
        }
        if (settingsMenuPanel != null)
            settingsMenuPanel.SetActive(false);



        SaveSettings();
        UpdateTexts();
    }

    public void SetScrollSensitivity(float value)
    {
        ScrollSensitivity = value;
        SaveSettings();
        UpdateTexts();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        SaveSettings();
        UpdateTexts();
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource == null)
        {
            GameObject musicObject = GameObject.FindGameObjectWithTag("GameMusic");

            if (musicObject != null)
                musicSource = musicObject.GetComponent<AudioSource>();
        }

        if (musicSource != null)
            musicSource.volume = value;

        SaveSettings();
        UpdateTexts();
    }

    public void RebindLeft()
    {
        keyToRebind = "Left";

        if (leftKeyText != null)
            leftKeyText.text = "Press key...";
    }

    public void RebindRight()
    {
        keyToRebind = "Right";

        if (rightKeyText != null)
            rightKeyText.text = "Press key...";
    }

    public void RebindJump()
    {
        keyToRebind = "Jump";

        if (jumpKeyText != null)
            jumpKeyText.text = "Press key...";
    }

    private void ListenForNewKey()
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (keyCode == KeyCode.Escape)
                {
                    keyToRebind = "";
                    UpdateTexts();
                    return;
                }

                AssignKey(keyCode);
                return;
            }
        }
    }

    private void AssignKey(KeyCode newKey)
    {
        if (keyToRebind == "Left")
            LeftKey = newKey;

        if (keyToRebind == "Right")
            RightKey = newKey;

        if (keyToRebind == "Jump")
            JumpKey = newKey;

        keyToRebind = "";

        SaveSettings();
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (scrollSensitivityText != null)
            scrollSensitivityText.text = "Scroll Sensitivity: " + ScrollSensitivity.ToString("0.0");

        if (masterVolumeText != null)
            masterVolumeText.text = "Volume: " + Mathf.RoundToInt(AudioListener.volume * 100f) + "%";

        if (musicVolumeText != null)
            musicVolumeText.text = "Music: " + Mathf.RoundToInt(GetMusicVolume() * 100f) + "%";

        if (leftKeyText != null)
            leftKeyText.text = LeftKey.ToString();

        if (rightKeyText != null)
            rightKeyText.text = RightKey.ToString();

        if (jumpKeyText != null)
            jumpKeyText.text = JumpKey.ToString();
    }

    private float GetMusicVolume()
    {
        if (musicSource != null)
            return musicSource.volume;

        return PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("ScrollSensitivity", ScrollSensitivity);
        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        PlayerPrefs.SetFloat("MusicVolume", GetMusicVolume());

        PlayerPrefs.SetString("LeftKey", LeftKey.ToString());
        PlayerPrefs.SetString("RightKey", RightKey.ToString());
        PlayerPrefs.SetString("JumpKey", JumpKey.ToString());

        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        ScrollSensitivity = PlayerPrefs.GetFloat("ScrollSensitivity", 1f);
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        LeftKey = LoadKey("LeftKey", KeyCode.A);
        RightKey = LoadKey("RightKey", KeyCode.D);
        JumpKey = LoadKey("JumpKey", KeyCode.Space);

        if (musicSource != null)
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    private KeyCode LoadKey(string keyName, KeyCode defaultKey)
    {
        string savedKey = PlayerPrefs.GetString(keyName, defaultKey.ToString());

        if (Enum.TryParse(savedKey, out KeyCode loadedKey))
            return loadedKey;

        return defaultKey;
    }
}