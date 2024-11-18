using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class button : MonoBehaviour
{

    Controller input;
    Animator anim;
    Vector2 movement;
    float horizontal; 
    //float speed = 1;
    void Awake()
    {
        input = new Controller();
        input.Control.Movement.performed += ctx => movement = ctx.ReadValue<Vector2>();
        input.Control.Movement.canceled += ctx => movement = Vector2.zero;
        input.Control.Scale.performed += ctx => grow();
        input.Control.Jump.performed += ctx => JumpPressed();
        
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (movement.x > 0 && horizontal <= 1)
            horizontal += Time.deltaTime * 2;
        else if (movement.x < 0 && horizontal >= -1)
            horizontal -= Time.deltaTime * 2;
        else
        {
            if (horizontal == 0)
                return;
            
            if (horizontal > 0)
                horizontal -= Time.deltaTime * 2;
            else
                horizontal += Time.deltaTime * 2;

        }

        horizontal = (float)Math.Round(horizontal, 2);

        anim.SetFloat("Side", horizontal);

        //Vector3 m = new Vector3(movement.x, 0, movement.y) * Time.deltaTime;
        //transform.Translate(m, Space.World);
    }

    void grow()
    {
        transform.localScale *= 1.1f;
    }

    void JumpPressed()
    {
        anim.SetTrigger("Jump");
    }

    void OnEnable()
    {
        input.Control.Enable();
    }

    void OnDisable()
    {
        input.Control.Movement.performed -= ctx => movement = ctx.ReadValue<Vector2>();
        input.Control.Movement.canceled -= ctx => movement = Vector2.zero;
        input.Control.Scale.performed -= ctx => grow();
        input.Control.Jump.performed -= ctx => JumpPressed();
        input.Control.Disable();
    }


}
