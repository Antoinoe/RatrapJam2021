using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject[] menuList;
    public bool isGamePaused;
    public bool TMP_haswin = false;
    public string title;


    public static MenuManager instance;

    void Awake()
    {
        //singleton
        transform.parent = null;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        AudioManager.instance.GetCorrectVolume();
        //Time.timeScale = 0;

    }
    public void Play()
    {
        //AudioManager.instance.StopPlayAll();
        SceneManager.LoadScene("PlayLevel");
    }

    public void Pause()
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            Time.timeScale = 0;
            menuList[0].SetActive(false);
        }
        else
        {
            isGamePaused = true;
            Time.timeScale = 1;
            menuList[0].SetActive(true);
        }

    }

    public void RestartLevel()
    {
        //AudioManager.instance.StopPlayAll();
        SceneManager.LoadScene("PlayLevel");
    }



    public void EndScreen()
    {
        menuList[1].SetActive(true);
        if (TMP_haswin)
        {
            menuList[1].transform.GetChild(0).GetComponent<Text>().text = title;
        }
        else
        {
            menuList[1].transform.GetChild(0).GetComponent<Text>().text = title;
        }
        //menuList[1].transform.GetChild(1).GetComponent<Text>().text = "Score : " + 0.ToString();/* + GameManager.instance.score.ToString();*/ 
    }

    public void MainMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            AudioManager.instance.StopPlayAll();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            for (int i = 0; i < menuList.Length; i++)
            {
                if(i == 0)
                {
                    menuList[i].SetActive(true);
                }
                else
                {
                    menuList[i].SetActive(false);
                }
            }
        }
    }
            

    public void Credits()
    {
        for (int i = 0; i <menuList.Length; i++)
        {
            if (menuList[i].name == "Credits")
            {
                menuList[i].SetActive(true);
            }
            else
                menuList[i].SetActive(false);
        }
    }

    public void OptionMenu()
    {
        for (int i = 0; i < menuList.Length; i++)
        {
            if (i == 1)
                menuList[i].SetActive(true);
            else
                menuList[i].SetActive(false);
        }
    }

    public void ClosePopUp()
    {
        Time.timeScale = 1;
        menuList[2].SetActive(false);
    }
}
