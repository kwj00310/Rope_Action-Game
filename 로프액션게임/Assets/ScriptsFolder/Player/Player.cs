using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
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
    public Transform pos;
    //플래그 변수
    bool WalkFlag;
    bool turnFlag;
    bool RunFlag;
    bool JumpFlag;
    bool DownFlag;
    bool jumpRequest = false;
    float curTime;
    float CoolTime=0.5f;
    //벡터 변수
    public Vector2 RightAttackBoxSize;
    public Vector2 AttackBoxSize;
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
        if (curTime > 0) curTime -= Time.deltaTime; 
        movement.x = 0;
        movement.y = rb.velocity.y;
        WalkFlag = false;
        RunFlag = false;
        GetComponent<SpriteRenderer>().flipX = turnFlag;
        IsDown();
        CheckMoving();
        CheckAnimator();
        Attack();
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
        if(rb.velocity.y<-7f) 
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
    //공격모션 출력 메서드
    void Attack()
    {
        if (Input.GetMouseButtonDown(0)&&curTime<=0) 
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            animator.Play("OnePunch");
            curTime=CoolTime;
            Vector2 Attack_pos= pos.transform.position;
            if (turnFlag==true)
            {
                Attack_pos += new Vector2(-0.432f, 0);
            }
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(Attack_pos, AttackBoxSize, 0);
            foreach (Collider2D obj in collider2Ds)
            {
                if (obj.tag == "Enemy")
                {
                    obj.GetComponent<BotA>().TakeDamage(1);
                }
            }
        }
   
    }
    // 어택에서 푸는 메서드
    void ResetFreeze()
    {
        rb.constraints =RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("End Motion");
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
    private void OnDrawGizmos()
    {
        Vector2 Attack_pos = pos.transform.position;
        if (turnFlag == true)
        {
            Attack_pos += new Vector2(-0.432f, 0);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Attack_pos, AttackBoxSize);
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
