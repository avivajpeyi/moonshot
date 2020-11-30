using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class AnimateUI : MonoBehaviour
{
    [Header("Orbit settings")] [SerializeField]
    public TextMeshProUGUI MoonText;

    public TextMeshProUGUI ShotText;
    public AudioClip MoonShotAudio;
    private AudioSource audioSource;
    public GameObject TitlePanel;

    public GameObject startPositionUI;
    public GameObject endPositionUI;
    private TextMeshProUGUI startUI;
    private TextMeshProUGUI endUI;

    public GameObject LevelCompleteUI;
    public TextMeshProUGUI levelCompleteText;
    
    [SerializeField] Vector2 startPos = new Vector2();
    [SerializeField] Vector2 endPos = new Vector2();

    private Rocket rocket;
    private Planetoid startPlanet;
    private Planetoid endPlanet;
    private Transform startPlanetTransform;
    private Transform endPlanetTransform;

    
    private StarSystemSpawner starSystemSpawner;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        starSystemSpawner = FindObjectOfType<StarSystemSpawner>();
        MoonText.text = "";
        ShotText.text = "";
        StartCoroutine(AnimateTitle());
        rocket = FindObjectOfType<Rocket>();

    }

    IEnumerator AnimateTitle()
    {
        yield return new WaitForSeconds(.1f);
        audioSource.PlayOneShot(MoonShotAudio);
        MoonText.text = "MOON";
        yield return new WaitForSeconds(.4f);
        ShotText.text = "SHOT";
        yield return new WaitForSeconds(.5f);
        MoonText.text = "";
        ShotText.text = "";
        TitlePanel.SetActive(false);
        
        
        // init controls
        if (rocket != null)
            rocket.controlEnabled = true;

        // init instructions 
        startPlanet = starSystemSpawner.start.GetComponent<Planetoid>();
        endPlanet = starSystemSpawner.end.GetComponent<Planetoid>();
        startPlanetTransform = startPlanet.transform;
        endPlanetTransform = endPlanet.transform;
        startUI = startPositionUI.GetComponent<TextMeshProUGUI>();
        endUI = endPositionUI.GetComponent<TextMeshProUGUI>();
        float dist = Camera.main.orthographicSize;
        startUI.text = "START";
        endUI.text = "GOAL";
        startUI.fontSize = dist * 0.5f;
        endUI.fontSize = dist * 0.5f;
        StartCoroutine(AnimateInstructions());
    }

    IEnumerator AnimateInstructions()
    {
        float timeBeforeFade = 15.0f;
        float fadeSpeed = 5.0f;
        float timeElapsed = 0;
        while (startUI.alpha > 0 && endUI.alpha > 0)
        {
            startPos =   startPlanetTransform.position;
            endPos = endPlanetTransform.position;
            startPos.y += startPlanet.size;
            endPos.y += endPlanet.size;
            
            startPositionUI.transform.position = startPos;
            endPositionUI.transform.position = endPos;

            timeElapsed += Time.deltaTime;

            if (timeElapsed >= timeBeforeFade)
            {
                startUI.alpha -= fadeSpeed * Time.deltaTime;
                endUI.alpha -= fadeSpeed * Time.deltaTime;
            }

            yield return null;
        }
        
    }

    public void EnableLevelCompleteUI()
    {
        LevelCompleteUI.SetActive(true);
    }

    public void ResetScene()
    {
        Debug.Log("Reset rocket");
        LevelCompleteUI.SetActive(false);
        rocket.ResetRocket();
    }
    

}