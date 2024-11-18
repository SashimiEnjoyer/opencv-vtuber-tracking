using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knight_controller : MonoBehaviour {

    float speed = 1;
    float rotspeed = 100;
    float rot = 0f;
    float gravity = 8;

    Vector3 movement = Vector3.zero;

    CharacterController controller;
    Animator anim;
	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if(controller.isGrounded)
        {
            if(Input.GetKey(KeyCode.W))
            {
                anim.SetFloat("speed", 0.5f, 0.1f, Time.deltaTime);
                movement = new Vector3(0, 0, 1);
                movement *= speed;
                movement = transform.TransformDirection(movement);
            }
            if(Input.GetKeyUp(KeyCode.W))
            {
                anim.SetFloat("speed", 0);
                movement = new Vector3(0, 0, 0);
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetFloat("speed", 1, 0.1f, Time.deltaTime);
                movement = new Vector3(0, 0, 3);
                movement *= speed;
                movement = transform.TransformDirection(movement);
            }
            if (Input.GetKeyUp(KeyCode.W) && Input.GetKeyUp(KeyCode.LeftShift))
            {
                anim.SetFloat("speed", 0);
                movement = new Vector3(0, 0, 0);
            }
            if (Input.GetMouseButton(0))
            {
                anim.SetBool("attack", true);
                movement = new Vector3(0, 0, 0);
            }
            if (Input.GetMouseButtonUp(0))
            {
                anim.SetBool("attack", false);
            }

        }

        rot += Input.GetAxis("Horizontal") * rotspeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rot, 0);
        movement.y -= gravity * Time.deltaTime;
        controller.Move(movement * Time.deltaTime);
	}
}
