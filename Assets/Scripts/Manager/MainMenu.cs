using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayCurrentLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenLevelList()
    {
        SceneManager.LoadScene(6);
    }
}
