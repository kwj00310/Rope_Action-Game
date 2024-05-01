using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BotA : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    //�׽�Ʈ��
    
    //�׽�Ʈ�� 
    bool WalkFlag;
    public bool flipFlag=false;
    int NextMove;
    bool isAttacking = false;
    Vector3 PlayerPos;
    Vector3 WhereToAtk;
    public GameObject warning;
    public GameObject Atk1;
    GameObject bullet;

    void Start()
    {
        Think();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {

        flipFlag = false;
        WalkFlag= false;
        rb.velocity = new Vector2(NextMove, rb.velocity.y);
        if (NextMove < 0) flipFlag = true;
        GetComponent<SpriteRenderer>().flipX = flipFlag;
        if (Mathf.Abs(NextMove)>0)
        {
            WalkFlag = true;
        }
        animator.SetBool("Is Walk", WalkFlag);
        if(Hp<=0)
        { 
            Destroy(this.gameObject);
        } 
        LeftSideAttack();
    }

    void FixedUpdate()
    {
        Vector2 frontVec = new Vector2(rb.position.x + NextMove*0.2f,rb.position.y);
        Debug.DrawRay(frontVec, Vector2.down,new Color(0,1,0));
        RaycastHit2D hit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (hit.collider==null)
        {
            NextMove=NextMove*-1;
            CancelInvoke();
            Invoke("Think", 5);
        }
    }
    void LeftSideAttack()
    {
        if (flipFlag==true)
        {
            Debug.DrawRay(rb.transform.position,new Vector2(-4,0), new Color(1, 0, 0));
            RaycastHit2D leftHit = Physics2D.Raycast(rb.transform.position, new Vector2(-1, 0), 4, LayerMask.GetMask("Player"));
            if(leftHit.collider!=null) 
            {
                PlayerPos = leftHit.transform.position;
                StartCoroutine("BeforeAttack");
            }
            
        }
        else
        {
            Debug.DrawRay(rb.transform.position, new Vector2(4, 0), new Color(1, 0, 0));
            RaycastHit2D RightHit = Physics2D.Raycast(rb.transform.position, new Vector2(1, 0), 4, LayerMask.GetMask("Player"));
            if (RightHit.collider != null)
            {
                PlayerPos = RightHit.transform.position;
                StartCoroutine("BeforeAttack");
            }
            
        }
        
    }

    //�����ϱ��� ������ ��ġ�� ǥ��
    IEnumerator BeforeAttack()
    {
        if (isAttacking == false)
        {
            WhereToAtk = PlayerPos;
            isAttacking = true;
            Debug.Log("������ ��ġ:" + WhereToAtk);
            Instantiate(warning, WhereToAtk, Quaternion.identity);
            yield return new WaitForSeconds(2f);
            StartCoroutine("Attack");
        }
    }
    IEnumerator Attack()
    {
        Debug.Log("��������");

        if (flipFlag == false)
        {
            bullet = Instantiate(Atk1, this.transform.position, Quaternion.Euler(0,0,180f));
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
        }
        else if (flipFlag == true)
        {
            bullet = Instantiate(Atk1, this.transform.position, Quaternion.Euler(0, 0,0f));
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
        }
        
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }



    void Think()
    {
        NextMove = Random.Range(-2, 2);
        Invoke("Think", 5);
    }
    public static float Hp = 3;

    public void TakeDamage(float damage)
    {
        if (animator==null)
        {
            Debug.LogError("�ִϸ����� ����!");
        }
        animator.Play("Hit");
        Hp -= damage;
    }
    void ReturnIdle()
    {
        animator.SetTrigger("Is not Hit");
    }
}
