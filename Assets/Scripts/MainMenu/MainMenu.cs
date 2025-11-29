using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject optionspanel;
    public GameObject buttonsPanel;
    public TMP_Dropdown qualityDropdown;

    public GameObject mainPlayerSkin;
    public Sprite[] spritesSkins;
    public string[] skinNames = { "Blue", "Purple", "Red", "Yellow" };



    private void Start()
    {
        PlayerPrefs.SetString("MainPlayerSkin", skinNames[0]); //Skin por defecto azul
        
        int savedQuality = PlayerPrefs.GetInt("QualityLevel",-1);

        if(savedQuality == -1)
        {
            savedQuality = QualitySettings.GetQualityLevel();
        }

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        qualityDropdown.value = savedQuality;
        qualityDropdown.onValueChanged.AddListener(OnQualityChange);
    }

    public void Playgame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        buttonsPanel.SetActive(false);
        optionspanel.SetActive(true);
    }

    public void CloseOptions()
    {
        buttonsPanel.SetActive(true);
        optionspanel.SetActive(false);
    }

    public void OnQualityChange(int index)
    {
        QualitySettings.SetQualityLevel(index,true);
        PlayerPrefs.SetInt("QualityLevel",index);
    }

    public void selectSkin(int index){
        mainPlayerSkin.GetComponent<Image>().sprite = spritesSkins[index];
        PlayerPrefs.SetString("MainPlayerSkin", skinNames[index]);
    }
}
