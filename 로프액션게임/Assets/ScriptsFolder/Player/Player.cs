using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.SearchService;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static float HP=100;
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
    bool WallFlag;
    bool jumpRequest = false;
    bool standFlag;
    float curTime;
    float CoolTime=0.1f;
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
        WallCheck();
        CheckAnimator();
        Attack();
        CheckHealth();
        BackSpawn();
    }
    void WallCheck()
    {
        if(turnFlag==false)
        {
            Debug.DrawRay(rb.position,Vector3.right, new Color(1, 0, 0));
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.right, 0.3f,LayerMask.GetMask("Platform"));
            if (hit.collider != null)
            {
                WallFlag = true;
                RunFlag=false;
                WalkFlag=false;
                Debug.Log("오른쪽 부분 벽에 붙어있음");
            }
            else
            {
                WallFlag = false;
            }
        }
        else if(turnFlag==true)
        {
            Debug.DrawRay(rb.position,Vector3.left,new Color(1, 0,0));
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.left, 0.3f, LayerMask.GetMask("Platform"));
            if (hit.collider != null)
            {
                RunFlag = false;
                WalkFlag = false;
                WallFlag = true;
                Debug.Log("왼쪽 부분 벽에 붙어있음");
            }
            else
            {
                WallFlag = false;
            }
        }
        
    }
    void Jump()
    {
        if(!standFlag)
        {
            JumpCounter++;
            rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            JumpFlag = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string layerName= LayerMask.LayerToName(collision.gameObject.layer);
        if (layerName=="Platform")
        {
            JumpFlag = false;
            DownFlag = false;
            JumpCounter = 0;
            if(!WallFlag)
            {
                if (collision.relativeVelocity.y >= 6f && collision.relativeVelocity.y <= 15f)
                {
                    animator.Play("Down-Stand");
                    standFlag = true;
                }
                if (collision.relativeVelocity.y > 15)
                {
                    animator.Play("HardDown-Stand");
                    standFlag = true;
                }
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
    void ResetStandFlag()
    {
        standFlag = false;
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
        animator.SetBool("Is Wall", WallFlag);
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
    void CheckHealth()
    {
        if(HP<=0)
        {
            gameObject.SetActive(false);
        }
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
    void BackSpawn()
    {
        if (this.transform.position.y<=-20)
        {
            this.transform.position = new Vector2(-5, 1);
            HP -= 30;
        }
    }
}
