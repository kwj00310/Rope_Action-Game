using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHolo : MonoBehaviour
{
    float HP = 10;
    float HoloTimer = 0;

    public void OnDamaged()
    {
        HP -= 10;
    }
    void CheckDIE()
    {
        if (HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void CheckHoloTime()
    {
        if(HoloTimer >=10f ) 
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
        CheckHoloTime();
        HoloTimer += Time.deltaTime;
        CheckDIE();
    }
}
