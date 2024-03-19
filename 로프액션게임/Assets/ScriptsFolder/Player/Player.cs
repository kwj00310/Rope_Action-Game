using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //이동관련 변수
    float Speed;
    float JumpPower = 4;
    //참조컴포넌트 관련
    Rigidbody2D rb;
    Animator animator;
    //플래그 변수
    bool WalkFlag;
    bool turnFlag;
    bool RunFlag;
    bool jumpRequest = false;
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
        Speed =Input.GetKey(KeyCode.LeftShift) ? 5 : 2.5f;
        if (Input.GetKey(KeyCode.LeftShift)) RunFlag = true;
        if(Input.GetKey(KeyCode.D))
        {
            turnFlag = false;
            movement.x = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            turnFlag=true;
            movement.x = -1;
        }
        if (Mathf.Abs(movement.x)==1) WalkFlag = true;
        GetComponent<SpriteRenderer>().flipX = turnFlag;
        if (Input.GetKeyDown(KeyCode.F))jumpRequest = true;
        //애니메이션 설정
        animator.SetBool("Is Run",RunFlag);
        animator.SetBool("Is walk", WalkFlag);
        if (rb.velocity.y < -2.9f)
        {
            Debug.DrawRay(this.transform.position, Vector2.down * PlayerSize.y, new Color(0, 1, 0));
            RaycastHit2D ray = Physics2D.Raycast(this.transform.position, Vector2.down, PlayerSize.y / 2, LayerMask.GetMask("Platform"));
            animator.SetBool("Is Jump", false);
            animator.SetBool("Is Down", true);
            if (ray.collider != null)
            {
                if (ray.distance < (PlayerSize.y / 2) - 0.01f)
                {
                    animator.SetBool("Is Down", false);
                }
            }
        }
    }
    void Jump()
    {
        rb.AddForce(Vector2.up *JumpPower, ForceMode2D.Impulse);
        animator.SetBool("Is Jump", true);
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
