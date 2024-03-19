using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Vector3 offset;
    Vector3 Max_Position;
    Transform ViewPoint;
    public string View_Taget;
    public float Smooth_Value=0.0625f;
    void Camera_offset()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) offset = Vector3.left * 2;
        else if (Input.GetKey(KeyCode.RightArrow)) offset = Vector3.right * 2;
        else if (Input.GetKey(KeyCode.UpArrow)) offset = Vector3.up*2;
        else if (Input.GetKey(KeyCode.DownArrow)) offset = Vector3.down *2;
        else offset = Vector3.zero;
    }
    void Start()
    {
        View_Taget = "Player";
        ViewPoint = GameObject.Find(View_Taget).transform;
    }
  
    void Update()
    {
        Camera_offset();   
    }
    private void FixedUpdate()
    {
        Max_Position = ViewPoint.position + offset;
        Vector3 Camera_positon = Vector3.Lerp(this.transform.position, Max_Position, Smooth_Value);
        this.transform.position = new Vector3(Camera_positon.x, Camera_positon.y, this.transform.position.z);
    }
}
