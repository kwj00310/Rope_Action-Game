using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotA_AtkPat : MonoBehaviour
{
    [SerializeField]
    bool isAttacking = false;
    Vector3 PlayerPos;
    Vector3 WhereToAtk;
    public GameObject warning;
    public GameObject Atk1;
    GameObject bullet;


    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerPos = collision.transform.position;
            StartCoroutine("BeforeAttack");
        }
    }
    //공격하기전 공격할 위치를 표시
    IEnumerator BeforeAttack()
    {
        if (isAttacking == false)
        {
            WhereToAtk = PlayerPos;
            isAttacking = true;
            Debug.Log("감지된 위치:" + WhereToAtk);
            Instantiate(warning, WhereToAtk, Quaternion.identity);
            yield return new WaitForSeconds(2f);
            StartCoroutine("Attack");
        }
    }
    IEnumerator Attack()
    {
        Debug.Log("공격중임");
        bullet=Instantiate(Atk1, this.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

}
