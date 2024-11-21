using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMovement : MonoBehaviour
{
    private Rigidbody2D rb; // Rigidbody2D component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(2, 0); // เคลื่อนที่ไปทางขวา
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);
    }
}
