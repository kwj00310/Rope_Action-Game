using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SearchService;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static float HP = 100;
    //이동관련 변수
    float Speed;            //걷기&달리기 스피드
    float JumpPower = 4;    //점프 강도 설정
    public int JumpCounter = 0;    //점프 횟수 체크
    float curTime;          //현재 쿨타임 진행시간 척도
    float CoolTime = 0.1f;  //쿨타임 주기
    //참조컴포넌트 관련
    Rigidbody2D rb;         //RigidBody 컴포넌트 변수화
    Animator animator;      //Animator 컴포넌트 변수화
    SpriteRenderer spriteRenderer;
    public Transform pos;   //Transform 컴포넌트 변수화(히트박스를 가져옴)
    //플래그 변수
    bool IsDamaged = false;
    bool WalkFlag;          //애니메이션 걷기 플래그
    bool turnFlag;          //애니메이션 돌기 플래그(Renderer.Filp.X)
    bool RunFlag;           //애니메이션 달리기 플래그
    bool JumpFlag;          //애니메이션 점프 플래그
    bool DownFlag;          //애니메이션 떨어지기 플래그
    bool WallFlag;          //애니메이션 벽에 매달리기 플래그
    bool jumpRequest = false;//점프 유무 플래그
    public bool standFlag;         //애니메이션 다시 서기 애니메이션 출력상태 확인 플래그
    bool IsPause = false;

    bool SandeFlag=false;

    //벡터 변수
    public Vector2 RightAttackBoxSize;  //어택공간 벡터값
    public Vector2 AttackBoxSize;       //왼,오른쪽 움직임 상황 벡터
    public GameObject Holo;
    public GameObject SandePrefab;
    Vector2 movement;
    // 유니티 지원 함수들
    void Start()
    {
        spriteRenderer= GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
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
        if(Input.GetKeyDown(KeyCode.C))
        {
            MakeHolo();
        }
        Pause();
        sande();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Layer가 Playform일때 실행되는 로직
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);
        if (layerName == "Platform")
        {
            JumpFlag = false;
            DownFlag = false;
            JumpCounter = 0;
            if (!WallFlag)
            {
                // 부딪치는 상대가속도의 차이로 애니메이션의 유무가 달라짐
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
        if (collision.gameObject.tag=="Enemy")
        {
            OnDamaged(collision.transform.position);
        }
    }
    void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            IsPause = !IsPause;
        }
        if (IsPause)
        {
            Time.timeScale = 0f;
        }
        else if (!IsPause)
        {
            Time.timeScale = 1f;
        }

    }
    public void OnDamaged(Vector2 targetPos)
    {
        animator.Play("Pain");
        HP -= 10;
        IsDamaged = true;
        gameObject.layer = 3;
        JumpCounter = 0;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rb.AddForce(new Vector2(dirc,1)*2,ForceMode2D.Impulse);

        StartCoroutine(ResetDamageState());
        Invoke("Returnlayer", 3f);
    }
    public void MakeHolo()
    {
        if (turnFlag && Input.GetKey(KeyCode.A))
        {
            GameObject M_Holo = Instantiate(Holo, rb.transform.position, Quaternion.Euler(0, 180f, 0f));
            transform.Translate(-1.5f, 0, 0);
        }
        else if (!turnFlag && Input.GetKey(KeyCode.D))
        {
            GameObject M_Holo = Instantiate(Holo, rb.transform.position, Quaternion.Euler(0, 0, 0));
            transform.Translate(1.5f, 0, 0);
        }
        else if (turnFlag) 
        {
            GameObject M_Holo = Instantiate(Holo, rb.transform.position, Quaternion.Euler(0, 180f, 0f));
            transform.Translate(1.5f, 0, 0);
        }
        else if (!turnFlag)
        {
            GameObject M_Holo = Instantiate(Holo, rb.transform.position, Quaternion.Euler(0, 0, 0));
            transform.Translate(-1.5f, 0, 0);
        }
    }
    //테스트 함수
    void sande()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SandeFlag = !SandeFlag;
        }
        if(SandeFlag)
        {
            GameObject gameObject = Instantiate(SandePrefab, rb.transform.position, Quaternion.Euler(0, 0, 0));
        }
        
    }


    //테스트 중 
    void Returnlayer()
    {
        spriteRenderer.color = Color.white;
        gameObject.layer = 8;
    }
    IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(1f);
        IsDamaged = false;
    }
    private void FixedUpdate()
    {
        if(!IsDamaged)
        {
            rb.velocity = new Vector2(movement.x * Speed, rb.velocity.y);
        }
        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }
    //해당 함수는 벽면에 레이캐스트를 쏴서 Platform이 있는지 확인 후 작동하는 함수
    void WallCheck()
    {
        if (turnFlag == false)
        {
            Debug.DrawRay(rb.position, Vector3.right, new Color(1, 0, 0));
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.right, 0.3f, LayerMask.GetMask("Platform"));
            if (hit.collider != null)
            {
                WallFlag = true;
                RunFlag = false;
                WalkFlag = false;
            }
            else
            {
                WallFlag = false;
            }
        }
        else if (turnFlag == true)
        {
            Debug.DrawRay(rb.position, Vector3.left, new Color(1, 0, 0));
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.left, 0.3f, LayerMask.GetMask("Platform"));
            if (hit.collider != null)
            {
                RunFlag = false;
                WalkFlag = false;
                WallFlag = true;
            }
            else
            {
                WallFlag = false;
            }
        }
    }
    //점프하는 함수(점프카운터,점프 실행 및 점프 애니메이션 출력유무)
    void Jump()
    {
        if (!standFlag)
        {
            JumpCounter++;
            rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            JumpFlag = true;
        }
    }
    //떨어지고 다시 서는 애니메이션에서 x좌표 고정을 풀어주는 함수
    public void SetLanded()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("Is Landed");
    }
    //좌표 고정 함수
    void StopPosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    //해당 오브젝트의 y축 가속도가 일정 이하일시 떨어지는 애니메이션 호출
    void IsDown()
    {
        if (rb.velocity.y < -7f)
        {
            JumpFlag = false;
            DownFlag = true;
        }
    }
    //이동관련 왼,오른쪽 유무와 달리기 속도를 관리하는 함수
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
    //공격모션 출력 함수 
    void Attack()
    {
          if (Input.GetMouseButtonDown(0) && curTime <= 0&&!standFlag)
          {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                animator.Play("OnePunch");
                curTime = CoolTime;
                Vector2 Attack_pos = pos.transform.position;
                // 만약 오른쪽으로 돌고있을시 해당 벡터 값만큼 공격포인트가 이동되게한다.
                if (turnFlag == true)
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
    //다시 서기 점프 가능 유무 함수(다시 서기 애니메이션 후 호출)
    void ResetStandFlag()
    {
        standFlag = false;
    }
    // 공격모션에서 풀어주는 함수(공격 애니메이션 후 호출)
    void ResetFreeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("End Motion");
    }
    //애니메이션 출력 관련 함수
    void CheckAnimator()
    {
        if (Input.GetKey(KeyCode.LeftShift)) RunFlag = true;
        if (Mathf.Abs(movement.x) == 1) WalkFlag = true;
        if (Input.GetKeyDown(KeyCode.F) && JumpCounter < 2) jumpRequest = true;
        animator.SetBool("Is Run", RunFlag);
        animator.SetBool("Is walk", WalkFlag);
        animator.SetBool("Is Jump", JumpFlag);
        animator.SetBool("Is Down", DownFlag);
        animator.SetBool("Is Wall", WallFlag);
    }
    //히트박스 크기 및 위치 확인용 함수
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
    //(임시) 체력 0이하 비활성화 함수
    void CheckHealth()
    {
        if (HP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    //(임시) 일정 y좌표값 떨어질 시 호출되는 함수
    void BackSpawn()
    {
        if (this.transform.position.y <= -20)
        {
            this.transform.position = new Vector2(-5, 1);
            HP -= 30;
        }
    }
}