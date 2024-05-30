using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController charactercontroller;
    public float speed = 10f;
    public Vector3 movedirection;

    // start is called before the first frame update
    private void Start()
    {
        charactercontroller = GetComponent<CharacterController>();

    }

    // update is called once per frame
    private void Update()
    {
        //debug.log("1");
        moveit();
    }
    public void moveit()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        movedirection = (transform.right * h + transform.forward * v).normalized;
        charactercontroller.Move(movedirection * speed * Time.deltaTime);
        //charactercontroller.Move(movedirection);

    }
    //public float speed = 5.0f; // �ƶ��ٶ�

    //void Update()
    //{
    //    float horizontalInput = Input.GetAxis("Horizontal");
    //    float verticalInput = Input.GetAxis("Vertical");

    //    Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

    //    if (moveDirection.magnitude >= 0.1f)
    //    {
    //        MovePlayer(moveDirection);
    //    }
    //    else
    //    {
    //        StopPlayer();
    //    }
    //}

    //void MovePlayer(Vector3 moveDirection)
    //{
    //    transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    //}

    //void StopPlayer()
    //{
    //    // ������������ֹͣ�ƶ����߼������������ٶ�Ϊ0�򲥷�ֹͣ����
    //}
}
