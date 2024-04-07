using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField]
    float bulletTimer;
    BotA TestBot;
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0f;
        TestBot = GetComponent<BotA>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().flipX = true;
        bulletTimer += Time.deltaTime;
        Timeout();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerComponent = collision.gameObject.GetComponent<Player>();
            playerComponent.OnDamaged(collision.transform.position);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("PlayerHolo"))
        {
            PlayerHolo HoloComponent = collision.gameObject.GetComponent<PlayerHolo>();
            HoloComponent.OnDamaged();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
    void Timeout()
    {
        if(bulletTimer>=5)
        {
            Destroy(gameObject);
        }
    }
}
