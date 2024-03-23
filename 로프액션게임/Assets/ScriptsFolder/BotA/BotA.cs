using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class BotA : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    bool WalkFlag;
    bool flipFlag=false;
    public int NextMove;
    
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
    void Think()
    {
        NextMove = Random.Range(-2, 2);
        Invoke("Think", 5);
    }
    public int Hp = 3;

    public void TakeDamage(int damage)
    {
        if (animator==null)
        {
            Debug.LogError("애니메이터 오류!");
        }
        animator.SetTrigger("Is Hit");
        Hp -= damage;
    }
    void ReturnIdle()
    {
        animator.SetTrigger("Is not Hit");
    }
}
