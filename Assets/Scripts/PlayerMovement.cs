using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2;

    Rigidbody2D rbd2d;
    Vector2 movementInput;

    private Animator animator;

    void Start()
    {
        rbd2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput = movementInput.normalized ;

        animator.SetFloat("Horizontal",Math.Abs(movementInput.x));
        animator.SetFloat("Vertical",Math.Abs(movementInput.y));

        CheckFlip();
    }

    private void FixedUpdate()
    {
        rbd2d.linearVelocity = movementInput * speed;
    }

    //Flip del personaje hacia izquierda o derecha
    void CheckFlip()
    {
        if(movementInput.x > 0 && transform.localScale.x < 0 || movementInput.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}
