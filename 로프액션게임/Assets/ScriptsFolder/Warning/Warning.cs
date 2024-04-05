using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{
    SpriteRenderer SpriteRenderer;
    float timer;
    void Start()
    {
        InvokeRepeating("Warning2",1f,1f);
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>=3)
        {
            Destroy(gameObject);
        }
        
    }
    void Warning2()
    {
        SpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        Invoke("Warning1", 0.5f);
    }
    void Warning1()
    {
        SpriteRenderer.color=Color.white;
    }
}
