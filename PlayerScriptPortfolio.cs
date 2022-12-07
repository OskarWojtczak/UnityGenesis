using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] is a way of displaying a variable in Unity via the Inspector tab without making it public and accessible to other scripts
    [SerializeField] private Transform groundCheckTransform = null;
    [SerializeField] private LayerMask playerMask;

    //All private variables 
    //jumpKeyWasPressed will need to be turned on and off so that we know when to execute specific bits of code
    private bool jumpKeyWasPressed;
    //horizontalInput is float because it is dynamically set using keyboard inputs every frame
    private float horizontalInput;
    //Tidying code by replacing every instance of GetComponent with our declared variable
    private Rigidbody rigidbodyComponent;
    //Keep track of coin collection and 'spend' via superJump
    private int superJumpsLeft;
    

    // Start is called before the first frame update
    void Start()
    {
        //We want this variable to be set only once but definitely before any other code is ran since it is used in Update()
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the space is key is pressed
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            //sets jump variable to true
            jumpKeyWasPressed = true;
        }

        //Assigns a value to the input variable eacch frame based on Unitys definition of what "Horizontal" input keys are. By default these are 'a,d' and <- and -> arrow keys
        horizontalInput = Input.GetAxis("Horizontal");
    }

    // FixedUpdate is called once every physics update(By default Unity physics updates 100 times/second, 100Hz)
    private void FixedUpdate() 
    {
        //The horizontalInput is used to increase or decrease the velocity in the x-plane
        //The 'new' y-value must be set to the 'old' y-value so that the object falls at the rate dictated by the Unity engine game physics and is not reset every frame when we input horizontal commands
        rigidbodyComponent.velocity = new Vector3(horizontalInput*2, rigidbodyComponent.velocity.y, 0);


        //This checks if the player object is grounded
        //We are checking if a smallsphere placed at the playerobjects feet is in contact with any other collider like the ground. 
        if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f, playerMask).Length == 0) 
        {
            //if the player is not grounded we do not want to execute the next block of code so we return
            return;
        }

        //if the player is grounded this code block executes
        if (jumpKeyWasPressed) 
        {
            //This code applies any coin power ups, executes a jump and resets out jump input to false(off).
            float jumpPower = 6f;
            if(superJumpsLeft > 0) 
            {
                jumpPower *= 2;
                superJumpsLeft --;
            }
            rigidbodyComponent.AddForce(Vector3.up *jumpPower, ForceMode.VelocityChange);
            jumpKeyWasPressed = false;
        }

        
    }

    //Called on Trigger events.(isTrigger was ticked(on) for 'coin' prefabs in Unity)
    private void OnTriggerEnter(Collider other) 
    {
        //Checks to see if the triggering object is also the desired layer
        if (other.gameObject.layer == 7)
        {
            //Destroys 'coin' since it is collected upon contact. Increases coin counter by 1(superJumpsLeft)
            Destroy(other.gameObject);
            superJumpsLeft ++;
        }        
    }

    
}
