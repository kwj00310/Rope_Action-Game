using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //이동관련 변수
    float Speed;
    float JumpPower = 4;
    int JumpCounter=0;
    //참조컴포넌트 관련
    Rigidbody2D rb;
    Animator animator;
    //플래그 변수
    bool WalkFlag;
    bool turnFlag;
    bool RunFlag;
    bool JumpFlag;
    bool DownFlag;
    bool jumpRequest = false;
    float lastSpeed;
    //벡터 변수
    Vector2 movement;
    Vector2 PlayerSize;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerSize=GetComponent<SpriteRenderer>().bounds.size;
    }
    // Update is called once per frame
    void Update()
    {
        movement.x = 0;
        movement.y = rb.velocity.y;
        WalkFlag = false;
        RunFlag = false;

        float currentSpeed = Mathf.Abs(rb.velocity.x);
        
 
        GetComponent<SpriteRenderer>().flipX = turnFlag;
        IsDown();
        CheckMoving();
        CheckAnimator();
    }
    void Jump()
    {
        JumpCounter++;
        rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
        JumpFlag = true;
            
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string layerName= LayerMask.LayerToName(collision.gameObject.layer);
        if (layerName=="Platform")
        {
            JumpFlag = false;
            DownFlag = false;
            JumpCounter = 0;
            if(collision.relativeVelocity.y>=6f&& collision.relativeVelocity.y <= 15f)
            {
                animator.Play("Down-Stand");
            }
            if (collision.relativeVelocity.y > 15)
            {
                animator.Play("HardDown-Stand");
            }
        }
    }
    public void SetLanded()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("Is Landed");
    }
    void StopPosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    void IsDown()
    {
        if(rb.velocity.y<-4f) 
        {
            JumpFlag = false;
            DownFlag = true;
        }
    }
    void CheckMoving()
    {
        if (Input.GetKey(KeyCode.D))
        {
            turnFlag = false;
            movement.x = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            turnFlag = true;
            movement.x = -1;
        }
        Speed = Input.GetKey(KeyCode.LeftShift) ? 5 : 2.5f;
        
    }
    void CheckAnimator()
    {
        if (Input.GetKey(KeyCode.LeftShift)) RunFlag = true;
        if (Mathf.Abs(movement.x) == 1) WalkFlag = true;
        if (Input.GetKeyDown(KeyCode.F)&&JumpCounter < 2) jumpRequest = true;
        animator.SetBool("Is Run", RunFlag);
        animator.SetBool("Is walk", WalkFlag);
        animator.SetBool("Is Jump", JumpFlag);
        animator.SetBool("Is Down", DownFlag);
    }
    private void FixedUpdate()

    {
        rb.velocity = new Vector2(movement.x * Speed, movement.y);
        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }
}
