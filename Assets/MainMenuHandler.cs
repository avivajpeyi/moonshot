using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public  GameObject InstructionsMenu;
    
    public Toggle debuggingToggle;


    private OptionsContainer optionsContainer;
    
    void Start()
    {
        optionsContainer = FindObjectOfType<OptionsContainer>();
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        InstructionsMenu.SetActive(false);
        
        
        debuggingToggle.onValueChanged.AddListener(delegate {
            DebuggingToggleValueChanged(debuggingToggle);
        });
        optionsContainer.DebuggingMode = debuggingToggle.isOn;
    }


    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        InstructionsMenu.SetActive(false);
    }

    public void ShowOptions()
    {
        OptionsMenu.SetActive(true);
        InstructionsMenu.SetActive(false);
    }
    
    
    
    void DebuggingToggleValueChanged(Toggle change)
    {
        Debug.Log("New Value : " + debuggingToggle.isOn);
        optionsContainer.DebuggingMode = debuggingToggle.isOn;
    }

    public void ShowInstructions()
    {
        OptionsMenu.SetActive(false);
        InstructionsMenu.SetActive(true);
    }
}
