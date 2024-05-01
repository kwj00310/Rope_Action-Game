using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeSlow : MonoBehaviour
{
    float DestroyTime = 0;
    float Green = 0;
    private void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
    }
    public void ResetGreen()
    {
        Green += 0.0085f;
        if (Green >= 255)
        {
            Green = 0;
        }
    }
    void Update()
    {
        ResetGreen();
        DestroyTime += Time.deltaTime;
        GetComponent<SpriteRenderer>().color = new Color(0, 255, Green);
         
        Destroy();

    }
    private void Destroy()
    {
        if(DestroyTime>=0.5f)
        {
            Destroy(this.gameObject);
        }
    }
}
