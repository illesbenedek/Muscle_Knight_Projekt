using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSeed = 5f;
    float horizontalMovement;


    [Header("jumping")]
    public float jumpPower = 10f;

    [Header("GrundCheck")]
    public Transform grundChekPos;
    public Vector2 grundChekSize = new Vector2(0.5f,0.5f);
    public LayerMask grundLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSeed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if (isGrunded())
        {


            if (context.performed)
            {
                // Teljes ugrás indítása az ugrás gomb megnyomásakor
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            }
            else if (context.performed)
            {
                //Változó magasságú ugrás: ha korábban elengedjük a gombot, csökkentjük a felfelé ívelõ sebességet
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

            }
        }

    }

    private bool isGrunded()
    {
        if (Physics2D.OverlapBox(grundChekPos.position, grundChekSize, 0, grundLayer))
        {
            return true;
        }
        return false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(grundChekPos.position, grundChekSize);
    }
}
