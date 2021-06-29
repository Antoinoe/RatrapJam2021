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
    public Slider eSlider, mSlider;

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
        GetCorrectVolume();
    }
    private void Update()
    {
        print(isGamePaused);
    }
    public void Play()
    {
        AudioManager.instance.StopPlayAll();
        SceneManager.LoadScene("Game");
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
            ApplySoundChanges();
            isGamePaused = true;
            Time.timeScale = 1;
            menuList[0].SetActive(true);
        }

    }

    public void ApplySoundChanges()
    {
        PlayerPrefs.SetFloat("eVal", eSlider.value);
        PlayerPrefs.SetFloat("mVal", mSlider.value);
    }

    public void RestartLevel()
    {
        AudioManager.instance.StopPlayAll();
        SceneManager.LoadScene("Game");
    }

    public void GetCorrectVolume()
    {
        eSlider.value = PlayerPrefs.GetFloat("eVal", 1);
        mSlider.value = PlayerPrefs.GetFloat("mVal", 1);
    }

    public void EndScreen()
    {
        if (TMP_haswin)
        {
            menuList[1].transform.GetChild(0).GetComponent<Text>().text = "*ignore my voice crack* You suUUUk!";
        }
        else
        {
            menuList[1].transform.GetChild(0).GetComponent<Text>().text = "you got to the top of the mountain and beat the storm, cheh";
        }
        menuList[1].transform.GetChild(1).GetComponent<Text>().text = "Score : " + 0.ToString();/* + GameManager.instance.score.ToString();*/ 
    }

    public void MainMenu()
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
            AudioManager.instance.StopPlayAll();
        SceneManager.LoadScene("MainMenu");
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
}
