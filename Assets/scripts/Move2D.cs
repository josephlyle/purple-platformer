using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    [SerializeField] private LayerMask groundLayerMask;
    public float moveSpeed = 5f;
    private float defaultMS = 5f;
    public float crouchSpeed = 1f;
    // jumping controls
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 4f;
    // running controls
    private string dir; // direction player is holding, (left or right)
    public float friction = 0.5f; // friction for when a player isn't holding a direction
    public float airFriction = 0.99f;
    // animator
    public Animator animator;
    public bool grounded;
    private bool facingRight = true;
    private bool faceingRightCheck; // facing right current frame check
    private bool running = false;
    private bool jumped = false;
    private bool crouching = false;
    public bool landing = false;
    public float landFrame = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update(){
        // movement state checks
        if(grounded && Input.GetKey(KeyCode.S)){ // check for crouching
            crouching = true;
            if(moveSpeed > (crouchSpeed + 2f)){ // keep some momentum when you start crouching
                moveSpeed *= .99f;
            }else{ moveSpeed = crouchSpeed; } 
        }else{
            crouching = false;
            moveSpeed = defaultMS; 
        }

        // basic movement
        if(Input.GetKey(KeyCode.D)){
            dir = "right";
            faceingRightCheck = true;
        }else if(Input.GetKey(KeyCode.A)){
            dir = "left";
            faceingRightCheck = false;
        }else{
            dir = "null";
        }

        if(Input.GetButtonDown("Jump") && grounded){
            Jump();
        }

        // references to animator
        animator.SetFloat("yVelocity", rb.velocity.y); 
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Jumped", jumped);
        animator.SetBool("Running", running); 
        animator.SetBool("Crouching", crouching);
        animator.SetBool("MinCrouchSpeed", (moveSpeed <= crouchSpeed));  
        animator.SetBool("Landing", landing);


//TODO: make this better / maybe put it somewhere else?  
        // if velocity y falls below 5f set jumpsquat to false 
        if(Input.GetButtonDown("Jump")){
            jumped = true;
            //landed = false;
        }
        if (rb.velocity.y < 5f) {
            jumped = false;            
        }
    }
    
    private void FixedUpdate(){ // physics
        grounded = IsGrounded(); // check if you are grounded
        //running physics
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        if(dir == "right" || dir == "left"){ // running
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
            running = true;
        }else if(dir == "null" && rb.velocity.x != 0 && grounded){ // deceleration
            rb.velocity = new Vector2(rb.velocity.x * friction, rb.velocity.y);
            running = false;
        }else if(dir == "null" && !grounded){ // air momentum/friction
            rb.velocity = new Vector2(rb.velocity.x * airFriction, rb.velocity.y);
        }

        // flip spritesheet if not facing direction of movement
        if(facingRight != faceingRightCheck){ 
            Flip();
        }

        //jumping physics
        if(rb.velocity.y < 0){ // if you're falling
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; //-1 to negate unity's default gravity
        }else if(rb.velocity.y > 0 && !Input.GetButton("Jump")){ //shorthop
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Jump(){
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(0f, 0f, 0f); // jumping starts from neutral velocity 
        rb.AddForce(new Vector2(0f, 6f), ForceMode2D.Impulse);
    }

    bool IsGrounded(){
        Vector3 raySize = new Vector3(bc.bounds.size.x*.45f, bc.bounds.size.y, bc.bounds.size.z);
        Vector3 raySource = new Vector3(bc.bounds.center.x, bc.bounds.center.y - 0.1f, bc.bounds.center.z);

        RaycastHit2D raycastHit = Physics2D.BoxCast(raySource, raySize, 0f, Vector2.down, .2f, groundLayerMask);

//TODO: gross
        if(!grounded && raycastHit.collider){ // landing check with a 3f delay so animation plays all the landing animation
            landing = true;
        } 
        if(landing && landFrame <= 4f){
            landFrame++;
        }else{
            landing = false;
            landFrame = 0;
        }
        
        /* code to cast rays of the ground check raycast on the debug scene */
        // var pointA = new Vector3(raySource.x - (raySize.x), raySource.y, 0);
        // var pointB = new Vector3(raySource.x - (raySize.x), raySource.y-.35f, 0);
        // var pointC = new Vector3(raySource.x + (raySize.x), raySource.y, 0);
        // var pointD = new Vector3(raySource.x + (raySize.x), raySource.y-.35f, 0);
        // Color color = new Color(1f, 0f, 0f);
        // Color color2 = new Color(0f, 0f, 1f);
        // Debug.DrawLine(pointA, pointB, color, .02f);
        // Debug.DrawLine(pointC, pointD, color2, .02f);
        return raycastHit.collider != null;
    }

    void Flip(){
        facingRight = faceingRightCheck;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}