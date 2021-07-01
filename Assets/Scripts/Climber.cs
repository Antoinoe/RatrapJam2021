using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    // Direction
    Vector2 dir;

    [Header("Jump")]
    // Jumping
    Rigidbody2D rb;
    public float jumpForce = 10;
    public int fallSpeed = 1;
    [HideInInspector] private float dirAngle;

    [Header("Holds")]
    // Hold
    public LayerMask targetLayer;
    [SerializeField] Hold inUseHold;
    GameObject inUseGO;

    [Header("Dash")]
    // Dash
    [SerializeField] bool canDash = false;
    public float dashCooldown;
    float dashTimer;
    [Range(0, 1)] public float dashCooldownVisu; ///
    public float castDistance;

    [Header("Ascension")]
    //Climbing
    public float height;
    float startHeight;
    public float highestPoint = 0;

    [Header("Death")]
    public GameObject deathParticles;
    public GameObject deathCorpse;
    public Color strikedColor;

    [Header("")]
    SpriteRenderer sr;
    Camera mainCam;
    public float deathDistance;
    public State playerState;

    // Anim
    Animator anim;
    private string currentAnimState;
    // Animation States
    const string Player_Idle = "Idle";
    const string Player_Jump = "Jump";
    const string Player_SideJump = "JumpSide";
    const string Player_Fall = "Falling";
    const string Player_SideFall = "FallingSide";


    public enum State
    {
        Grounded,
        Jumping,
        Holding,
        Falling,
        Dead
    }

    bool CanJump { get { return playerState == State.Holding || playerState == State.Grounded; } }
    bool CanGrab { get { return playerState == State.Jumping || playerState == State.Falling; } }
    public bool Alive { get { return playerState != State.Dead; } }


    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        startHeight = transform.position.y;
    }


    void Update()
    {
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = new Vector2(cursorPos.x - transform.position.x, cursorPos.y - transform.position.y).normalized;

        if (Input.GetMouseButtonUp(0) && CanJump && Alive)
            Jump();

        if(rb.velocity.y < .01f && playerState == State.Jumping && CanGrab)
        {
            if (Mathf.Abs(dirAngle) > 5)
                ChangeAnimationState(Player_SideFall);
            else
                ChangeAnimationState(Player_Fall);
        }

        /// Dash Cast
        RaycastHit2D[] hit = new RaycastHit2D[0];

        if (canDash)
        {
            hit = Physics2D.BoxCastAll(transform.position, Vector2.one * 5, 0, dir, castDistance, targetLayer);

            if (hit.Length > 0)
            {
                GameObject closest = hit[0].transform.gameObject;
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider != null && hit[i].transform.gameObject != inUseGO)
                    {
                        Vector2 _I = new Vector2(hit[i].transform.position.x - transform.position.x, hit[i].transform.position.y - transform.position.y);
                        Vector2 proj = Vector3.Project(_I, dir);
                        //Debug.DrawRay(transform.position, _I, Color.magenta, Time.deltaTime);
                        //Debug.DrawLine(new Vector2(transform.position.x + _I.x, transform.position.y + _I.y), new Vector2(transform.position.x + proj.x, transform.position.y + proj.y), Color.cyan, Time.deltaTime);

                        Vector2 ItoProj = new Vector2(proj.x - _I.x, proj.y - _I.y);
                        if (ItoProj.magnitude < Vector2.Distance(closest.transform.position, Vector3.Project(closest.transform.position, dir)))
                        {
                            closest = hit[i].transform.gameObject;
                        }
                    }
                }

                //Debug.DrawLine(transform.position, closest.transform.position, Color.red, Time.deltaTime;

                if (Input.GetMouseButtonUp(1) && Alive)
                    Dash(closest);
            }
        }

        /// Ascension
        height = transform.position.y - startHeight;
        if (height > highestPoint) highestPoint = height;
        TileSpawn.Get.ReachNextSpawn(highestPoint);
        EventSpawn.Get.CheckHeight(highestPoint);

        if (TooFar && Alive) DeathFall();


            ////////////// ------------------------ Debug ------------------------

            if (Input.GetKeyUp(KeyCode.Space))
                HoldPosition(transform.position);

        if (Input.GetKeyUp(KeyCode.Return))
            Release();

        Debug.DrawRay(transform.position, dir * castDistance, Color.yellow, Time.deltaTime);
    }

    void Jump()
    {
        dirAngle = Vector2.SignedAngle(Vector2.up, dir);

        Release();

        rb.velocity = dir * jumpForce;

        playerState = State.Jumping;

        if (Mathf.Abs(dirAngle) > 5)
        {
            ChangeAnimationState(Player_SideJump);
            sr.flipX = Mathf.Sign(dirAngle) < 0;
        }
        else
            ChangeAnimationState(Player_Jump);
    }

    void HoldPosition(Vector3 position, State newState = State.Holding, GameObject holdingTo = null)
    {
        sr.flipX = false;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        transform.position = position;

        playerState = newState;

        if (holdingTo != null)
        {
            inUseHold = holdingTo.GetComponent<Hold>();
            inUseGO = holdingTo;
            inUseHold.Holding();
        }

        ChangeAnimationState(Player_Idle);
    }

    public void Release()
    {
        rb.gravityScale = fallSpeed;
        playerState = State.Falling;

        if (inUseHold != null)
        {
            inUseHold.Release();
            ChangeAnimationState(Player_Fall);
        }
    }

    void Dash(GameObject target)
    {
        Debug.DrawLine(transform.position, target.transform.position, Color.white, .1f); ///////////////////////////

        //Release();
        HoldPosition(target.transform.position, State.Holding, target);

        canDash = false;
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        dashTimer = 0;

        while (!canDash)
        {
            dashTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);

            dashCooldownVisu = Mathf.Lerp(0, 1, dashTimer / dashCooldown);
            if (dashTimer >= dashCooldown) canDash = true;
        }
    }

    public void AddVelocity(Vector2 add)
    {
        if (CanGrab) rb.velocity += add;
    }


    bool TooFar { get { return (transform.position - mainCam.transform.position).magnitude >= deathDistance; } }

    public void DeathFall()
    {
        HoldPosition(transform.position, State.Dead);
        sr.enabled = false;

        StartCoroutine(DeathTimer(1));
    }

    public void DeathHit(bool recolor = false)
    {
        HoldPosition(transform.position, State.Dead);
        sr.enabled = false;

        // Spawn Body particle
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        ParticleSystem corpse = Instantiate(deathCorpse, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        if (recolor) corpse.startColor = strikedColor;

        StartCoroutine(DeathTimer(2));
    }

    IEnumerator DeathTimer(float t)
    {
        yield return new WaitForSeconds(t);

        // Show DeathScreen
    }


    void ChangeAnimationState(string newState)
    {
        if (currentAnimState == newState) return;

        anim.Play(newState);
        currentAnimState = newState;
    }

// ------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Hold") && CanGrab)
            HoldPosition(collision.transform.position, State.Holding, collision.gameObject);

        if (collision.gameObject.CompareTag("Ground"))
            HoldPosition(collision.ClosestPoint(transform.position), State.Grounded);

        if (collision.gameObject.CompareTag("Danger") && Alive)
            DeathHit(); // Collide with Rock
    }
}
