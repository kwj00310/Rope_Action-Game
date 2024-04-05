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
        bullet=Instantiate(Atk1, this.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

}
