using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEffect : MonoBehaviour
{
    ParticleSystem pSyst;
    ParticleSystem.Particle[] rainDrops;
    Transform pSysTransform;

    [Header("Angle")]
    [Range(-90, 90)] public float mainAngle = 0;
    float targetAngle;
    float baseAngle = 0;
    public float angleDifference;

    public float angularVelocity;
    public float maxRandom;

    [Header("Particles")]
    public float strongWindSpeed = 350;
    float baseSpeed;
    public float strongDropCount = 350;
    float baseCount;

    // Start is called before the first frame update
    void Start()
    {
        pSyst = GetComponentInChildren<ParticleSystem>();
        pSysTransform = pSyst.transform.parent;

        baseSpeed = pSyst.startSpeed;
        baseCount = pSyst.emissionRate;
    }

    // Update is called once per frame
    void Update()
    {
        float addAngle = Random.Range(-maxRandom, maxRandom);

        Mathf.Clamp(TargetAngle, 1, 179);
        if (Mathf.Abs(pSysTransform.rotation.eulerAngles.z - TargetAngle) > angleDifference)
        {
            addAngle += TargetAngle - pSysTransform.rotation.eulerAngles.z;
        }

        pSysTransform.rotation = Quaternion.Euler(new Vector3(0, 0, pSysTransform.rotation.eulerAngles.z + addAngle * angularVelocity * Time.deltaTime));
    }

    float TargetAngle { get => mainAngle + 90; }

    public void ApplyWind(float direction, int windLevel = 1) // direction 1 OU -1
    {
        if(windLevel > 1)
        {
            mainAngle = direction * 80;
            pSyst.startSpeed = strongWindSpeed;
            pSyst.emissionRate = strongDropCount;
        }
        else if (windLevel < 1)
            Reset(direction);
        else
            mainAngle = direction * Random.Range(50, 65);
    }

    public void Reset(float direction = 0)
    {
        mainAngle = baseAngle + Random.Range(-10, 10) + direction * Random.Range(-5, 25);
        pSyst.startSpeed = baseSpeed;
        pSyst.emissionRate = baseCount;
    }
}
