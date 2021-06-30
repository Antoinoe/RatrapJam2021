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
    [SerializeField] bool isJumping = false;

    [Header("Holds")]
    // Hold
    public bool onFallOnly = false;
    [SerializeField] bool canGrab = true;
    Collider2D col2D;
    public LayerMask targetLayer;
    [SerializeField] Hold inUseHold;
    GameObject inUseGO;

    [Header("Dash")]
    // Dash
    [SerializeField] bool canDash = false;
    public float dashCooldown;
    float dashTimer;
    [Range(0,1)] public float dashCooldownVisu; ///
    public float castDistance;

    [Header("Ascension")]
    //Climbing
    public float height;
    float startHeight;
    public float highestPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();

        startHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = new Vector2(cursorPos.x - transform.position.x, cursorPos.y - transform.position.y).normalized;

        if (Input.GetMouseButtonUp(0) && !isJumping)
            Jump();

        if (onFallOnly)
        {
            if (!canGrab && isJumping && rb.velocity.y <= .5f)
            {
                canGrab = true;
                col2D.enabled = false;
                col2D.enabled = true;
            }
        }

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

                if (Input.GetMouseButtonUp(1))
                    Dash(closest);
            }
        }

        height = transform.position.y - startHeight;
        if (height > highestPoint) highestPoint = height;
        TileSpawn.Get.ReachNextSpawn(highestPoint);


        ////////////// ------------------------ Debug ------------------------

        if (Input.GetKeyUp(KeyCode.Space))
            GrabHold(transform.position);
        
        if (Input.GetKeyUp(KeyCode.Return))
            Release();

        //Debug.DrawRay(transform.position, dir * 5, Color.red, Time.deltaTime);
        Debug.DrawRay(transform.position, dir * castDistance, Color.yellow, Time.deltaTime);

        
    }

    void Jump()
    {
        rb.velocity = dir * jumpForce;
        //rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);

        Release();

        if (onFallOnly && dir.y > 0) canGrab = false;
    }

    void GrabHold(Vector3 position, GameObject holdingTo = null)
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        transform.position = position;

        isJumping = false;

        if (holdingTo != null)
        {
            inUseHold = holdingTo.GetComponent<Hold>();
            inUseGO = holdingTo;
            inUseHold.Holding();
        }
    }

    public void Release()
    {
        rb.gravityScale = fallSpeed;
        isJumping = true;

        if (inUseHold != null)
        {
            inUseHold.Release();
        }
    }

    void Dash(GameObject target)
    {
        Debug.DrawLine(transform.position, target.transform.position, Color.white, .1f);

        Release();
        GrabHold(target.transform.position, target);

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Hold"))
        {
            if(canGrab)
                GrabHold(collision.transform.position, collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Ground")) GrabHold(collision.ClosestPoint(transform.position));
    }
}
