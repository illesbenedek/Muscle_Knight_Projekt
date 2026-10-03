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

    [Header("Jumping")]
    public float jumpPower = 10f;

    [Header("Ground Check")]
    public Transform grundChekPos;
    public Vector2 grundChekSize = new Vector2(0.5f, 0.5f);
    public LayerMask grundLayer;

    void Start()
    {
        // Ha nem húztad be manuálisan az rb-t, automatikusan megpróbálja megkeresni a karakteren
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSeed, rb.linearVelocity.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Gomb megnyomásakor ugrás (csak ha a talajon áll)
        if (context.performed && isGrunded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }

        // Gomb elengedésekor az ugrási magasság csökkentése (akkor is mûködik, ha már levegõben van)
        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private bool isGrunded()
    {
        if (grundChekPos == null) return false;

        return Physics2D.OverlapBox(grundChekPos.position, grundChekSize, 0, grundLayer);
    }

    public void OnDrawGizmosSelected()
    {
        if (grundChekPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(grundChekPos.position, grundChekSize);
        }
    }
}