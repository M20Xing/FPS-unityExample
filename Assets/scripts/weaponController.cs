using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class weaponController : MonoBehaviour
{
    public playerMovement PM;
    public int bulletsMag = 31;//弹夹数量
    public int range = 100;//射程
    public int bulletLeft = 300;//备弹
    public int currentBullects;//当前子弹数量
    public Transform shooterPoint;//射击的位置
    private bool GunShootInput;//是否按下开火键
    public float fireRate = 0.1f;//射速
    private float fireTimer=0;
    public ParticleSystem muzzleFlash;//枪口火焰特效
    public Light muzzleFlashLight;//枪口火焰灯光
    public GameObject hitparticle;//子弹命中粒子特效
    public GameObject bullecthole;//弹孔
    private AudioSource audioSource;

    public AudioClip Ak47SoundClip;//射击音效
    public AudioClip reloadAmmoLeftClip;//换弹1音效
    public AudioClip reloadOutOfAmmoLeftClip;//换弹2音效

    private bool isReload;//判断是否在装弹
    private bool isAiming;//判断是否瞄准

    public Transform casingSpawnPoint;//子弹壳抛出位置
    public Transform casingPrefab;//子弹壳预制体
    [Header("按键设置")]
    [SerializeField][Tooltip("填装子弹按键")] private KeyCode reloadInputName;
    [SerializeField][Tooltip("查看武器按键")] private KeyCode inspectInputName;
    [SerializeField][Tooltip("自动半自动切换按键")] private KeyCode GunShootModelInputName;


    [Header("UI设置")]
    public Image CrossHairUI;
    public Text AmmoTextUI;
    public Text ShootModelTextUI;

    private Animator anim;
    private Camera mainCamera;

    //使用枚举来区分全自动半自动
    public enum ShootMode{AutoRife,SemiGun};
    public ShootMode shootingMode;
    private int ModeNum = 1;
    private bool GunShootInputMode;
    private string shootName;
    private void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        reloadInputName = KeyCode.R;
        inspectInputName = KeyCode.F;
        GunShootModelInputName = KeyCode.X;
        shootingMode=ShootMode.AutoRife;//默认全自动
        shootName = "全自动";
        currentBullects = bulletsMag;
        UpdateAmmoUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(GunShootModelInputName) && ModeNum != 1)
        {
            ModeNum = 1;
            shootName = "全自动";
            shootingMode = ShootMode.AutoRife;
            ShootModelTextUI.text = shootName;
        }
        else if (Input.GetKeyDown(GunShootModelInputName) && ModeNum != 0)
        {
            ModeNum = 0;
            shootName = "半自动";
            shootingMode = ShootMode.SemiGun;
            ShootModelTextUI.text = shootName;
        }
        switch (shootingMode)
        {
            case ShootMode.AutoRife:
                GunShootInput = Input.GetMouseButton(0);
                fireRate = 0.1f;
                break;
                case ShootMode.SemiGun:
                GunShootInput = Input.GetMouseButtonDown(0);
                fireRate = 0.2f;

                break;
        }
        //GunShootInput = Input.GetMouseButton(0);
        if (GunShootInput && currentBullects>0)
        {
            GunFire();
        }
        else
        {
        muzzleFlashLight.enabled = false;

        }
        AnimatorStateInfo info=anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("reload_ammo_left") || info.IsName("reload_out_of_ammo"))
        {
            isReload = true;
        }
        else
        {
            isReload=false;
        }
        if (Input.GetKeyDown(reloadInputName) && currentBullects < bulletsMag && bulletLeft > 0)
        {
            Roload();
        }
        DoingAim();
        if (Input.GetKeyDown(inspectInputName))
        {
            anim.SetTrigger("inspect");//查看武器
        }
        anim.SetBool("Run", PM.isRun);
        anim.SetBool("Walk", PM.isWalk);
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }
    public void GunFire()
    {
        if (fireTimer < fireRate || currentBullects<=0 || isReload || PM.isRun) return;

        RaycastHit hit;
        Vector3 shootDirection = shooterPoint.forward;
        if (Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))
        {
            Debug.Log("打到了" + hit.transform.name);
            GameObject hitParticleEffect = Instantiate(hitparticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            GameObject bullectHoleEffect = Instantiate(bullecthole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            Destroy(hitParticleEffect,1f);
            Destroy(bullectHoleEffect,1f);

        }
        if (!isAiming)
        {
            anim.CrossFadeInFixedTime("fire", 0.1f);//不瞄准就腰射

        }
        else 
        {
            //瞄准开火
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);

        }
        PlayerShootSound();
        Instantiate(casingPrefab, casingSpawnPoint.transform.position, casingSpawnPoint.transform.rotation);//实例抛弹壳
        muzzleFlash.Play();//播放火光特效
        muzzleFlashLight.enabled = true;
        currentBullects--;
        UpdateAmmoUI();
        fireTimer = 0f;
    }
    public void Roload()
    {
        if (bulletLeft <= 0)
        {
            return;
        }
        DoReloadAnimation();
        int bullectToload = bulletsMag - currentBullects;
        int bullectToReduce = bulletLeft >= bullectToload ? bullectToload : bulletLeft;
        bulletLeft -= bullectToReduce;//减少备弹
        currentBullects +=bullectToReduce;//当前子弹数增加
        UpdateAmmoUI();
    }
    public void DoReloadAnimation() 
    {
        if (currentBullects > 0)
        {
            anim.Play("reload_ammo_left", 0,0);//播放动画一
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }
        if (currentBullects == 0)
        {
            anim.Play("reload_out_of_ammo", 0,0);//播放动画二
            audioSource.clip = reloadOutOfAmmoLeftClip;
            audioSource.Play();

        }
    }
    public void PlayerShootSound()
    {
        audioSource.clip = Ak47SoundClip;
        audioSource.Play();
    }
    public void DoingAim() //瞄准
    {

        if (Input.GetMouseButton(1) && !isReload && !PM.isRun)
        {
            isAiming = true;
            anim.SetBool("Aim", true);
            CrossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 25;
        }
        else
        {
            isAiming = false;
            anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 60;

        }
    }
    public void UpdateAmmoUI()
    {
        AmmoTextUI.text = currentBullects + "/" + bulletLeft;
        ShootModelTextUI.text = shootName;
    }
}
