using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerScript : MonoBehaviour
{
    protected Joystick joystick;

    protected bool jump;
    Rigidbody rb;
    BoxCollider bc;

    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {        
        rb.MovePosition(transform.position + (transform.forward *
            joystick.Vertical * 0.05f));
        rb.MoveRotation(rb.rotation * Quaternion.Euler(
            new Vector3(0,joystick.Horizontal * 2f,0)));

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Pickups"){
            other.gameObject.transform.parent = GetComponent<Transform>();
        }
    }
}
