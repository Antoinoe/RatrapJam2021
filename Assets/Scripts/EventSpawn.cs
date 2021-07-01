using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSpawn : MonoBehaviour
{
    private static EventSpawn inst;
    public static EventSpawn Get { get { return inst; } }

    Climber climber;
    GameObject mainCanvas;

    [Header("CheckPoints")]
    public Vector3 dangerLevelUp;
    float nextDangerLimit;
    int dangerLevel = 0;
    float betweenSpawns = 120;
    float lastSpawn = 0;

    [Header("Rock Slide")]
    public GameObject rockPrefab;
    public float rockWarningTimer;
    public float rockFallSpeed;
    public Vector2 rockSpinSpeed;
    public GameObject rockWarningPanel;

    [Header("Wind Gust")]
    public float initialWindForce;
    public float windForce;
    public float windWarningTimer;
    public float windDuration;
    public GameObject windWarningPanel;
    public RainEffect rainEffect;

    [Header("Lightning Strike")]
    public GameObject strikeTargetPrefab;
    public float lightningTargetDuration;
    public float lightningFollowSpeed = 7;
    public float strikeHitRadius = 5;

    // Events : Rock Slide, Wind Gust, Lightning Strike

    private void Awake()
    {
        if (inst == null) inst = this;
    }

    void Start()
    {
        nextDangerLimit = dangerLevelUp.x;
        climber = FindObjectOfType<Climber>();
        mainCanvas = FindObjectOfType<Canvas>().gameObject;
        if (rainEffect == null) rainEffect = FindObjectOfType<RainEffect>();
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

        switch (Random.Range(0,3)) /////
        {
            case 0:
                StartCoroutine(RockSlide());
                break;

            case 1:
                StartCoroutine(WindGust());
                break;

            case 2:
                StartCoroutine(LightningStrike());
                break;

            default:
                break;
        }
    }

/// Events ------

    IEnumerator RockSlide()
    {
        float targetX = climber.transform.position.x;
        RectTransform panel = Instantiate(rockWarningPanel, mainCanvas.transform).GetComponent<RectTransform>();
        RectTransform icon = panel.GetChild(0).GetComponent<RectTransform>();
        Vector2 panelPos = new Vector2(climber.transform.position.x, panel.position.y); // Rock World position
        Vector2 outCanvasPos;

        float timer = 0;
        while(timer < rockWarningTimer)
        {
            timer += Time.deltaTime;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, Camera.main.WorldToScreenPoint(panelPos), Camera.main, out outCanvasPos); // World -> to Screen -> to Canvas Rect Position !!!
            icon.anchoredPosition = new Vector2(outCanvasPos.x, icon.anchoredPosition.y);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(panel.gameObject);

        GameObject rockSlide = Instantiate(rockPrefab, new Vector2(targetX, climber.transform.position.y + 40), Quaternion.identity);
        Rigidbody2D rbR = rockSlide.GetComponent<Rigidbody2D>();
        rbR.gravityScale = rockFallSpeed;
        rbR.angularVelocity = Random.Range(rockSpinSpeed.x, rockSpinSpeed.y);
        rbR.velocity += Vector2.right * Random.Range(-3.5f, 3.5f);
    }

    IEnumerator WindGust()
    {
        float direction = (Random.Range(0, 2) - .5f) * 2; // 1 OU -1
        windWarningPanel.transform.localScale = new Vector2(direction, 1);
        windWarningPanel.SetActive(true);

        rainEffect.ApplyWind(direction);
        float timer = 0;
        while(timer < windWarningTimer)
        {
            timer += Time.deltaTime;
            climber.AddVelocity(Vector2.right * direction * initialWindForce * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        rainEffect.ApplyWind(direction, 2);
        windWarningPanel.SetActive(false);
        while (timer < windWarningTimer + windDuration)
        {
            timer += Time.deltaTime;
            climber.AddVelocity(Vector2.right * direction * windForce * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        rainEffect.ApplyWind(direction, 0);
    }

    IEnumerator LightningStrike()
    {
        FollowTarget targetPoint = Instantiate(strikeTargetPrefab, climber.transform.position, Quaternion.identity).GetComponent<FollowTarget>();
        targetPoint.target = climber.transform;
        targetPoint.speed = lightningFollowSpeed;
        
        float timer = 0;
        while(timer < lightningTargetDuration)
        {
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        targetPoint.speed = 0;

        yield return new WaitForSeconds(.8f);

        Debug.DrawRay(targetPoint.transform.position, (targetPoint.transform.position - climber.transform.position).normalized * strikeHitRadius, Color.white, Time.deltaTime);
        if ((targetPoint.transform.position - climber.transform.position).magnitude <= strikeHitRadius)
            climber.DeathHit();

        Destroy(targetPoint.gameObject);
    }
}
