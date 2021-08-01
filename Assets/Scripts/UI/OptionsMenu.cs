using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace RyanGQ.RunOrDie.UI
{
    /// <summary>
    /// Options menu controls.
    /// </summary>
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] Text DisplayModeState;
        [SerializeField] Text ResolutionState;
        [SerializeField] Text SensitivityState;
        [SerializeField] Text VolumeState;
        [SerializeField] AudioMixer MainMixer;

        public static int Sensitivity = 50;
        public const float MouseDivisorX = 15f;
        public const float MouseDivisorY = 17.5f;

        private const string _displayModeDefault = "Window";
        private const int _resolutionDefault = 2;
        private const int _sensitivityDefault = 50;
        private const int _volumeDefault = 100;

        private List<int[]> _allowedResolutions = new List<int[]>();

        private void Start()
        {
            ApplyOptions();
        }

        private void ApplyOptions()
        {
            // Set up and apply options.
            switch (PlayerPrefs.GetString("DisplayMode", _displayModeDefault))
            {
                case "Window":
                    Screen.fullScreen = false;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    DisplayModeState.text = "Window";
                    break;
                case "Borderless":
                    Screen.fullScreen = true;
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    DisplayModeState.text = "Borderless";
                    break;
                case "Fullscreen":
                    Screen.fullScreen = true;
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    DisplayModeState.text = "Fullscreen";
                    break;
            }

            int resolution = PlayerPrefs.GetInt("Resolution", _resolutionDefault);
            foreach (Resolution r in Screen.resolutions)
                _allowedResolutions.Add(new int[] { r.width, r.height });
            ResolutionState.text = _allowedResolutions[resolution][0] + "x" + _allowedResolutions[resolution][1];
            Screen.SetResolution(_allowedResolutions[resolution][0], _allowedResolutions[resolution][1], Screen.fullScreen);

            int sensitivity = PlayerPrefs.GetInt("Sensitivity", _sensitivityDefault);
            SensitivityState.text = sensitivity + "%";
            Sensitivity = sensitivity;

            int volume = PlayerPrefs.GetInt("Volume", _volumeDefault);
            VolumeState.text = volume + "%";
            MainMixer.SetFloat("MasterVolume", volume);
        }

        public void OnDisplayModeChange()
        {
            switch (PlayerPrefs.GetString("DisplayMode", _displayModeDefault))
            {
                case "Window":
                    PlayerPrefs.SetString("DisplayMode", "Borderless");
                    break;
                case "Borderless":
                    PlayerPrefs.SetString("DisplayMode", "Fullscreen");
                    break;
                case "Fullscreen":
                    PlayerPrefs.SetString("DisplayMode", "Window");
                    break;
            }
            ApplyOptions();
        }

        public void OnResolutionChange()
        {
            int resolution = PlayerPrefs.GetInt("Resolution", _resolutionDefault);
            if (resolution == _allowedResolutions.Count - 1)
                resolution = 0;
            else
                resolution++;
            PlayerPrefs.SetInt("Resolution", resolution);
            ApplyOptions();
        }

        public void OnSensitivityChange()
        {
            int sensitivity = PlayerPrefs.GetInt("Sensitivity", _sensitivityDefault);
            if (sensitivity == 100)
                sensitivity = 0;
            else
                sensitivity += 10;
            PlayerPrefs.SetInt("Sensitivity", sensitivity);
            ApplyOptions();
        }

        public void OnVolumeChange()
        {
            int volume = PlayerPrefs.GetInt("Volume", _volumeDefault);
            if (volume == 100)
                volume = 0;
            else
                volume += 10;
            PlayerPrefs.SetInt("Volume", volume);
            ApplyOptions();
        }

    }
}