using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private int sceneIdx;

    // Start is called before the first frame update
    void Start()
    {
        sceneIdx = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(sceneIdx);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SceneManager.LoadScene(nextSceneId(1));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SceneManager.LoadScene(nextSceneId(-1));
        }
    }

    int nextSceneId(int changeValue)
    {
        int nextScene = (sceneIdx + changeValue) % SceneManager.sceneCountInBuildSettings;
        if (nextScene < 0)
            nextScene = SceneManager.sceneCountInBuildSettings - 1 ;
        return nextScene;
    }
}