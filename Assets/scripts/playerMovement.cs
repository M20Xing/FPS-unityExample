using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public float walkSpeed = 10f;
    public float runSpeed = 15f;
    public float speed;
    public Vector3 moveDirection;
    public bool isRun;//判断是否奔跑
    public bool isWalk;//判断是否行走
    private bool isJump;
    public bool isGround;
    public float jumpForce = 3f;//跳跃的力
    public Vector3 velocity;//y轴的冲量变化
    public Transform groundCheck;//地面检测
    private float groundDistance = 0.1f;//与地面的距离
    public LayerMask groundMash;
    public float gravity = -9f;
    [Header("声音设置")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runingSound;

    [Header("键位设置")]
    [SerializeField][Tooltip("奔跑按键")] public KeyCode runInputName;
    [SerializeField][Tooltip("跳跃按键")] public string jumpInputName = "Jump";
    // Start is called before the first frame update
    private void Start()
    {
        characterController=GetComponent<CharacterController>();
        audioSource=GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;
       groundCheck= GameObject.Find("Player/CheckGround").GetComponent<Transform>();
    }

    // Update is called once per frame
   private  void Update()
    {
        CheckGround();
        Move();
        Debug.Log(isGround);

    }
    public void Move()
    {
        //if (isGround && velocity.y <= 0)
        //{
        //    velocity.y = -2f;
        //}
        float h = Input.GetAxis("Horizontal");
        float v=Input.GetAxis("Vertical");
        isRun=Input.GetKey(runInputName);
        isWalk=(Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)?true:false;
        speed=isRun ? runSpeed : walkSpeed;
        moveDirection = (transform.right * h + transform.forward * v).normalized;
         characterController.Move(moveDirection*speed*Time.deltaTime);
        if (isGround == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        Debug.Log(velocity.y);
        characterController.Move(velocity * Time.deltaTime);
        Jump();
        PlayFootStepSound();


    }
    public void PlayFootStepSound()
    {
        if (isGround && moveDirection.sqrMagnitude > 0.9f)
        {
            audioSource.clip = isRun ? runingSound : walkingSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else 
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
    public void Jump() 
    {
        isJump = Input.GetButtonDown(jumpInputName);
        //Debug.Log(isJump);
        if (isJump && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity) ;
        }

    }
    public void CheckGround()
    {
       isGround= Physics.CheckSphere(groundCheck.position, groundDistance, groundMash);
        if (isGround && velocity.y<=0)
        {
            velocity.y = -2f;
        }
    }
}
