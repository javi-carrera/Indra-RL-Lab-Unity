using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_menu : MonoBehaviour
{
    public void playUC1 ()
    {
        SceneManager.LoadScene("UC1Scene");
    }

    public void playUC2()
    {
        SceneManager.LoadScene("UC2Scene");
    }

    //public void playUC3()
    //{
      //  SceneManager.LoadScene("UC1Scene");
    //} 

    public void QuitApp()
    {
        Application.Quit();
    }
}
