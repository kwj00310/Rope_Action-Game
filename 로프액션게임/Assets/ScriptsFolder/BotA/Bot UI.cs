using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BotUI : MonoBehaviour
{
    [SerializeField]
    public Slider HpBar;
    public float MaxHP=3;
    void Start()
    {
        HpBar.value = BotA.Hp/MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHp();
    }
    
    private void HandleHp()
    {
        HpBar.value = BotA.Hp/MaxHP;
    }
}
