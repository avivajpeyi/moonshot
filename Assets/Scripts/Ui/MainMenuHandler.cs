using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public  GameObject InstructionsMenu;
    
    public Toggle debuggingToggle;


    public Toggle linearGravityToggle;
    public Slider gravityFactorSlider;
    public TextMeshProUGUI gravityEquation;
    public TextMeshProUGUI gravityFactorText;
    
    private OptionsContainer optionsContainer;
    
    void Start()
    {
        optionsContainer = FindObjectOfType<OptionsContainer>();
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        InstructionsMenu.SetActive(false);
        
        // SET DEFAULTS 
        gravityFactorSlider.value = 1.5f;
        debuggingToggle.isOn = false;
        linearGravityToggle.isOn = true;

        SetGravityEquation();
        SetGravityFactorText();
        
        // SET LISTENERS 
        debuggingToggle.onValueChanged.AddListener(delegate {
            DebuggingToggleValueChanged(debuggingToggle);
        });
        linearGravityToggle.onValueChanged.AddListener(delegate {
            LinearGravityToggleValueChanged(debuggingToggle);
        });
        
        gravityFactorSlider.onValueChanged.AddListener(delegate {GravityFactorSliderValueChangeCheck(); });
        
        optionsContainer.gravityFactor = gravityFactorSlider.value;
        optionsContainer.linearGravity = linearGravityToggle.isOn;
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

    // Invoked when the value of the slider changes.
    public void GravityFactorSliderValueChangeCheck()
    {
        Debug.Log("New gravity factor = " + gravityFactorSlider.value);
        SetGravityFactorText();
        optionsContainer.gravityFactor = gravityFactorSlider.value;
    }

    void SetGravityEquation()
    {
        if (linearGravityToggle.isOn)
            gravityEquation.text = "F = -G mM / r";
        else
            gravityEquation.text = "F = -G mM / r\u00B2";
    }


    void SetGravityFactorText()
    {
        gravityFactorText.text = string.Format("G={0:0.0}", gravityFactorSlider.value);
    }

    void DebuggingToggleValueChanged(Toggle change)
    {
        Debug.Log("New Value : " + debuggingToggle.isOn);
        optionsContainer.DebuggingMode = debuggingToggle.isOn;
    }

    void LinearGravityToggleValueChanged(Toggle change)
    {
        Debug.Log("New Value : " + linearGravityToggle.isOn);
        optionsContainer.linearGravity = linearGravityToggle.isOn;
        SetGravityEquation();
    }

    
    public void ShowInstructions()
    {
        OptionsMenu.SetActive(false);
        InstructionsMenu.SetActive(true);
    }
}
