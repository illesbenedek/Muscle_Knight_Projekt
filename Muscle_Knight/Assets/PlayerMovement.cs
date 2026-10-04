using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isFacingRight = true; //attól függ a karakter merre néz majd (de a muscle knight sprite jobbra néz szóval ez jó) -T.
    [Header("Movement")]
    public float moveSeed = 5f;
    float horizontalMovement;


    [Header("jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 1; //késõbb lehet 2, egyszerûbb lesz majd a double jumpot behozni ha ez itt van
                             //(kicsit nem akarja a unity átvenni a beírt értéket innen, nemtudom miért) -T.
    int jumpsRemaining;

    [Header("GrundCheck")]
    public Transform grundChekPos;
    public Vector2 grundChekSize = new Vector2(0.5f, 0.05f);
    public LayerMask grundLayer;
    bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 1.4f;
    public float maxFallSpeed = 8f;
    public float fallSpeedMultiplier = 2.2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;

    //Wall slide
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        groundCheck();
        Gravity();
        Flip();
        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSeed, rb.linearVelocity.y);
            Flip();
        }
    }

    public void Gravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //gyorsabb esés -T.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)); //a maximum zuhanási sebességet tartatja be -T.
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                // Teljes ugrás indítása az ugrás gomb megnyomásakor
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }
            else if (context.canceled)
            {
                //Változó magasságú ugrás: ha korábban elengedjük a gombot, csökkentjük a felfelé ívelõ sebességet
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;

            }
        }
        //Walljump
        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); //falról elugrás -T.
            wallJumpTimer = 0;
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //Walljump 0.5f-ig tart, és újra használhatod 0.6f után.

            //ugársokor forduljon meg a karakter

            if(transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }
        }

    }

    
    private void groundCheck()
    {
        if (Physics2D.OverlapBox(grundChekPos.position, grundChekSize, 0, grundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void WallSlide()
    {
        if(!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); //olyan mint a gravitációs max sebességes esés, csak a falon -T.
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = - transform.localScale.x; //ellentétes irányba ugrik azért van ott az a minusz jel "- transform" -T.
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime; // aha -T.
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void Flip() //mozgás iránytól függõen, megfordítja a sprite-ot meg a hitboxot -T.
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(grundChekPos.position, grundChekSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    /* private void OnValidate() //arra van hogy az inspector ben az legyen amit te akarsz és nem a legelsõ megkapott érték -T.
    {
        baseGravity = 1.4f;
        maxFallSpeed = 8f;
        fallSpeedMultiplier = 2.2f;
    }
    */
}
