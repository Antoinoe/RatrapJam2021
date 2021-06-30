using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawn : MonoBehaviour
{
    private static EventSpawn inst;
    public static EventSpawn Get { get { return inst; } }

    Climber climber;

    [Header("CheckPoints")]
    public Vector3 dangerLevelUp;
    float nextDangerLimit;
    int dangerLevel = 0;
    float betweenSpawns = 120;
    float lastSpawn = 0;

    [Header("Wind Gust")]
    public float initialWindForce;
    public float windForce;
    public float windWarningTimer;
    public float windDuration;
    public GameObject windWarningPanel;

    // Events : Rock Slide, Wind Gust, Lightning Strike

    private void Awake()
    {
        if (inst == null) inst = this;
    }

    void Start()
    {
        nextDangerLimit = dangerLevelUp.x;
        climber = FindObjectOfType<Climber>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LevelUpDanger()
    {
        dangerLevel++;
        betweenSpawns /= 2;

        switch (dangerLevel)
        {
            case 0:
                nextDangerLimit = dangerLevelUp.y;
                break;
                
            case 1:
                nextDangerLimit = dangerLevelUp.z;
                break;
                
            case 2:
                nextDangerLimit = 2 * dangerLevelUp.z;
                break;

            default:
                break;
        }
    }

    public void CheckHeight(float highest)
    {
        if (dangerLevel < 3 && highest >= nextDangerLimit) LevelUpDanger();

        if (highest >= lastSpawn + betweenSpawns) Spawn();
    }

    void Spawn()
    {
        lastSpawn += betweenSpawns;

        switch (Random.Range(0,3))
        {
            case 0:
                RockSlide();
                break;

            case 1:
                StartCoroutine(WindGust());
                break;

            case 2:
                LightningStrike();
                break;

            default:
                break;
        }
    }

/// Events ------

    void RockSlide()
    {
        Debug.LogWarning("It's Raining Rocks !");
    }

    IEnumerator WindGust()
    {
        Debug.LogWarning("fwooo...");
        float direction = (Random.Range(0, 2) - .5f) * 2; // 1 OU -1
        windWarningPanel.transform.localScale = new Vector2(direction, 1);
        windWarningPanel.SetActive(true);

        float timer = 0;
        while(timer < windWarningTimer)
        {
            timer += Time.deltaTime;
            climber.AddVelocity(Vector2.right * direction * initialWindForce * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        windWarningPanel.SetActive(false);
        Debug.LogWarning("WojshoOOO !!!");
        while (timer < windWarningTimer + windDuration)
        {
            timer += Time.deltaTime;
            climber.AddVelocity(Vector2.right * direction * windForce * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    void LightningStrike()
    {
        Debug.LogWarning("Strike me down God ! You don't have the b-");
    }
}
