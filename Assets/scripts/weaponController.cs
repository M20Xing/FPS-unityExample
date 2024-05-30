using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class weaponController : MonoBehaviour
{
    public playerMovement PM;
    public int bulletsMag = 31;//��������
    public int range = 100;//���
    public int bulletLeft = 300;//����
    public int currentBullects;//��ǰ�ӵ�����
    public Transform shooterPoint;//�����λ��
    private bool GunShootInput;//�Ƿ��¿����
    public float fireRate = 0.1f;//����
    private float fireTimer=0;
    public ParticleSystem muzzleFlash;//ǹ�ڻ�����Ч
    public Light muzzleFlashLight;//ǹ�ڻ���ƹ�
    public GameObject hitparticle;//�ӵ�����������Ч
    public GameObject bullecthole;//����
    private AudioSource audioSource;

    public AudioClip Ak47SoundClip;//�����Ч
    public AudioClip reloadAmmoLeftClip;//����1��Ч
    public AudioClip reloadOutOfAmmoLeftClip;//����2��Ч

    private bool isReload;//�ж��Ƿ���װ��
    private bool isAiming;//�ж��Ƿ���׼

    public Transform casingSpawnPoint;//�ӵ����׳�λ��
    public Transform casingPrefab;//�ӵ���Ԥ����
    [Header("��������")]
    [SerializeField][Tooltip("��װ�ӵ�����")] private KeyCode reloadInputName;
    [SerializeField][Tooltip("�鿴��������")] private KeyCode inspectInputName;
    [SerializeField][Tooltip("�Զ����Զ��л�����")] private KeyCode GunShootModelInputName;


    [Header("UI����")]
    public Image CrossHairUI;
    public Text AmmoTextUI;
    public Text ShootModelTextUI;

    private Animator anim;
    private Camera mainCamera;

    //ʹ��ö��������ȫ�Զ����Զ�
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
        shootingMode=ShootMode.AutoRife;//Ĭ��ȫ�Զ�
        shootName = "ȫ�Զ�";
        currentBullects = bulletsMag;
        UpdateAmmoUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(GunShootModelInputName) && ModeNum != 1)
        {
            ModeNum = 1;
            shootName = "ȫ�Զ�";
            shootingMode = ShootMode.AutoRife;
            ShootModelTextUI.text = shootName;
        }
        else if (Input.GetKeyDown(GunShootModelInputName) && ModeNum != 0)
        {
            ModeNum = 0;
            shootName = "���Զ�";
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
            anim.SetTrigger("inspect");//�鿴����
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
            Debug.Log("����" + hit.transform.name);
            GameObject hitParticleEffect = Instantiate(hitparticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            GameObject bullectHoleEffect = Instantiate(bullecthole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            Destroy(hitParticleEffect,1f);
            Destroy(bullectHoleEffect,1f);

        }
        if (!isAiming)
        {
            anim.CrossFadeInFixedTime("fire", 0.1f);//����׼������

        }
        else 
        {
            //��׼����
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);

        }
        PlayerShootSound();
        Instantiate(casingPrefab, casingSpawnPoint.transform.position, casingSpawnPoint.transform.rotation);//ʵ���׵���
        muzzleFlash.Play();//���Ż����Ч
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
        bulletLeft -= bullectToReduce;//���ٱ���
        currentBullects +=bullectToReduce;//��ǰ�ӵ�������
        UpdateAmmoUI();
    }
    public void DoReloadAnimation() 
    {
        if (currentBullects > 0)
        {
            anim.Play("reload_ammo_left", 0,0);//���Ŷ���һ
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }
        if (currentBullects == 0)
        {
            anim.Play("reload_out_of_ammo", 0,0);//���Ŷ�����
            audioSource.clip = reloadOutOfAmmoLeftClip;
            audioSource.Play();

        }
    }
    public void PlayerShootSound()
    {
        audioSource.clip = Ak47SoundClip;
        audioSource.Play();
    }
    public void DoingAim() //��׼
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
