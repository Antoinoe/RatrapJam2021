using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Ref Points / Position
    Vector2 startPos;
    public Transform bottom;
    float bottomY;
    public Transform summitPoint;
    Vector2 summitPos;
    public Transform leftBound;
    float Xmax;
    public Transform rightBound;
    float Xmin;
    Transform player;
    Climber climber;
    Camera cam;

    // Following Speed
    public Vector2 speed;
    [Range(0,2)] public float speedFactor = 1;
    [Min(1)] public float FallSpeedClamp;


    // Start is called before the first frame update
    void Start()
    {
        climber = FindObjectOfType<Climber>();
        player = climber.transform;

        startPos = new Vector2(0, player.position.y);
        bottomY = bottom.position.y;
        summitPos = new Vector2(0, summitPoint.position.y);
        Xmin = leftBound.position.x;
        Xmax = rightBound.position.x;
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float Xmov = ((transform.position.x - CamWidth >= Xmin && transform.position.x + CamWidth <= Xmax) && Mathf.Abs(player.position.x - transform.position.x) > .05f) ? (player.position.x - transform.position.x) * speed.x : 0;
        float Ymov = (Mathf.Abs(player.position.y - transform.position.y) > .05f) ? (player.position.y - transform.position.y) * speed.y : 0;
        
        Ymov = Mathf.Max(Ymov, -FallSpeedClamp);

        transform.Translate(new Vector2(Xmov, Ymov) * Time.deltaTime * speedFactor);
        ClampCamera();
    }

    float CamHeight { get { return cam.orthographicSize; } }
    float CamWidth { get { return cam.aspect * CamHeight; } }
    float CamOffset { get { return cam.transform.localPosition.y; } }

    void ClampCamera()
    {
        transform.position = new Vector2(Mathf.Max(transform.position.x, Xmin + CamWidth), Mathf.Max(transform.position.y, climber.highestPoint - CamHeight - CamOffset));
        transform.position = new Vector2(Mathf.Min(transform.position.x, Xmax - CamWidth), Mathf.Min(transform.position.y, summitPos.y + CamOffset));
        transform.position = new Vector2(transform.position.x, Mathf.Max(transform.position.y, bottomY + CamHeight - CamOffset));
    }
}
