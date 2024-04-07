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
    //�̵����� ����
    float Speed;            //�ȱ�&�޸��� ���ǵ�
    float JumpPower = 4;    //���� ���� ����
    public int JumpCounter = 0;    //���� Ƚ�� üũ
    float curTime;          //���� ��Ÿ�� ����ð� ô��
    float CoolTime = 0.1f;  //��Ÿ�� �ֱ�
    //����������Ʈ ����
    Rigidbody2D rb;         //RigidBody ������Ʈ ����ȭ
    Animator animator;      //Animator ������Ʈ ����ȭ
    SpriteRenderer spriteRenderer;
    public Transform pos;   //Transform ������Ʈ ����ȭ(��Ʈ�ڽ��� ������)
    //�÷��� ����
    bool IsDamaged = false;
    bool WalkFlag;          //�ִϸ��̼� �ȱ� �÷���
    bool turnFlag;          //�ִϸ��̼� ���� �÷���(Renderer.Filp.X)
    bool RunFlag;           //�ִϸ��̼� �޸��� �÷���
    bool JumpFlag;          //�ִϸ��̼� ���� �÷���
    bool DownFlag;          //�ִϸ��̼� �������� �÷���
    bool WallFlag;          //�ִϸ��̼� ���� �Ŵ޸��� �÷���
    bool jumpRequest = false;//���� ���� �÷���
    public bool standFlag;         //�ִϸ��̼� �ٽ� ���� �ִϸ��̼� ��»��� Ȯ�� �÷���
    bool IsPause = false;

    bool SandeFlag=false;

    //���� ����
    public Vector2 RightAttackBoxSize;  //���ð��� ���Ͱ�
    public Vector2 AttackBoxSize;       //��,������ ������ ��Ȳ ����
    public GameObject Holo;
    public GameObject SandePrefab;
    Vector2 movement;
    // ����Ƽ ���� �Լ���
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
        //Layer�� Playform�϶� ����Ǵ� ����
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);
        if (layerName == "Platform")
        {
            JumpFlag = false;
            DownFlag = false;
            JumpCounter = 0;
            if (!WallFlag)
            {
                // �ε�ġ�� ��밡�ӵ��� ���̷� �ִϸ��̼��� ������ �޶���
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
    //�׽�Ʈ �Լ�
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


    //�׽�Ʈ �� 
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
    //�ش� �Լ��� ���鿡 ����ĳ��Ʈ�� ���� Platform�� �ִ��� Ȯ�� �� �۵��ϴ� �Լ�
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
    //�����ϴ� �Լ�(����ī����,���� ���� �� ���� �ִϸ��̼� �������)
    void Jump()
    {
        if (!standFlag)
        {
            JumpCounter++;
            rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            JumpFlag = true;
        }
    }
    //�������� �ٽ� ���� �ִϸ��̼ǿ��� x��ǥ ������ Ǯ���ִ� �Լ�
    public void SetLanded()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("Is Landed");
    }
    //��ǥ ���� �Լ�
    void StopPosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    //�ش� ������Ʈ�� y�� ���ӵ��� ���� �����Ͻ� �������� �ִϸ��̼� ȣ��
    void IsDown()
    {
        if (rb.velocity.y < -7f)
        {
            JumpFlag = false;
            DownFlag = true;
        }
    }
    //�̵����� ��,������ ������ �޸��� �ӵ��� �����ϴ� �Լ�
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
    //���ݸ�� ��� �Լ� 
    void Attack()
    {
          if (Input.GetMouseButtonDown(0) && curTime <= 0&&!standFlag)
          {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                animator.Play("OnePunch");
                curTime = CoolTime;
                Vector2 Attack_pos = pos.transform.position;
                // ���� ���������� ���������� �ش� ���� ����ŭ ��������Ʈ�� �̵��ǰ��Ѵ�.
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
    //�ٽ� ���� ���� ���� ���� �Լ�(�ٽ� ���� �ִϸ��̼� �� ȣ��)
    void ResetStandFlag()
    {
        standFlag = false;
    }
    // ���ݸ�ǿ��� Ǯ���ִ� �Լ�(���� �ִϸ��̼� �� ȣ��)
    void ResetFreeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("End Motion");
    }
    //�ִϸ��̼� ��� ���� �Լ�
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
    //��Ʈ�ڽ� ũ�� �� ��ġ Ȯ�ο� �Լ�
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
    //(�ӽ�) ü�� 0���� ��Ȱ��ȭ �Լ�
    void CheckHealth()
    {
        if (HP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    //(�ӽ�) ���� y��ǥ�� ������ �� ȣ��Ǵ� �Լ�
    void BackSpawn()
    {
        if (this.transform.position.y <= -20)
        {
            this.transform.position = new Vector2(-5, 1);
            HP -= 30;
        }
    }
}