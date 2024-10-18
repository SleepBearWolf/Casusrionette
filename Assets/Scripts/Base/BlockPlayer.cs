using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayer : MonoBehaviour
{
    [SerializeField] private float pushBackForce = 5f; 
    private Rigidbody2D rb2d;
    private bool isPushedBack = false;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Vector2 pushBackDirection = transform.position - collision.transform.position;
            PushBack(pushBackDirection.normalized); 
        }
    }

    private void PushBack(Vector2 direction)
    {
        if (!isPushedBack)
        {
            isPushedBack = true;
            rb2d.velocity = direction * pushBackForce; 

            StartCoroutine(ResetPushBack());
        }
    }

    private IEnumerator ResetPushBack()
    {
        yield return new WaitForSeconds(0.5f); 
        isPushedBack = false; 
    }
}
