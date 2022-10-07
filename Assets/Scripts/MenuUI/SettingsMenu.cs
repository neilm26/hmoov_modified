using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    //REMEMBER TO ADD THE INPUT FIELD NUMBERS BESIDE THE SLIDER FUNCTIONALITY LATER 
    //ALSO REMEMBER TO SWAP OUT ALL OF THE TEXT WITH TEXTMESHPRO
    //replace Text with TextMeshProUGUI 

    public AudioMixer audioMixer;

    public Dropdown resolutionDropdown;

    Resolution[] resolutions;
    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetLobbyVolume (float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen (int FullscreenMode)
    {
        if (FullscreenMode == 0)
            Debug.Log("someone make a windowed fullscreen lol");

        if (FullscreenMode == 1)
            Screen.fullScreen = true;

        if (FullscreenMode == 2)
            Screen.fullScreen = false;
    }
}
