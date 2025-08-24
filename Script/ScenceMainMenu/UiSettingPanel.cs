using UnityEngine;
using UnityEngine.UI;

public class UISettingsPanel : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void OnEnable()
    {
        // Khi panel bật -> load giá trị hiện tại
        musicSlider.value = GameManager.Instance.musicVolume;
        sfxSlider.value = GameManager.Instance.sfxVolume;
    }

    public void OnMusicChanged(float value)
    {
        GameManager.Instance.musicVolume = value;
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicSource.volume = value;
    }

    public void OnSFXChanged(float value)
    {
        GameManager.Instance.sfxVolume = value;
    }

    public void SaveSettings()
    {
        GameManager.Instance.SaveSettings();
        gameObject.SetActive(false); // ẩn panel
    }

    public void Back()
    {
        gameObject.SetActive(false); // ẩn panel không lưu
    }
}
