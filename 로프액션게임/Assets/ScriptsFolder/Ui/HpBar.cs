using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
   public Slider HPBar;

    private float maxHp = 100;
    void Start()
    {
        HPBar.value = Player.HP / maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Player.HP -= 10;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Player.HP = maxHp;
        }
        HandleHp();
    }
    private void HandleHp()
    {
        HPBar.value = Mathf.Lerp(HPBar.value, Player.HP / maxHp,Time.deltaTime);
    }
}
