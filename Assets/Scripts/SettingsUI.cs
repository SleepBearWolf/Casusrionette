using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public void AdjustVolume(float volume)
    {
        
       
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        
    }

    public void ToggleFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        
    }
}
