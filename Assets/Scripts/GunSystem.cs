using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunSystem : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform barrel;
    private RaycastHit hitInfo;

    [Header("Gun Stats")]
    [SerializeField] private int damage;
    [SerializeField] private int magazineSize;
    [SerializeField] private int bulletPerTap;
    [SerializeField] private int totalBullet;

    private int bulletShot, bulletLeft, bulletRequire, totalGunBullet;

    [SerializeField] private float spread, range, reloadTime, timeBetweenShot, timeBetweenShooting;
    [SerializeField] private bool allowButtonHold;
    [SerializeField] private bool allowReload;
    [SerializeField] private bool isLauncher;

    private bool shooting, reloading, readyToShoot;

    [SerializeField] private GameObject muzzleFlash, bulletDecay, hitEnemyParticle, grenadePrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Text bulletText;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;

    private void Awake()
    {
        bulletShot = 0;
        bulletLeft = magazineSize;
        readyToShoot = true;

        totalGunBullet = totalBullet;
    }

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerCamera = GetComponentInParent<Camera>();
        animator = GetComponentInChildren<Animator>();
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerController.moveInput.x > 0 || playerController.moveInput.y > 0)
        {
            if (playerController.isSprint)
            {
                animator.SetBool("isRun", true);
            }
            else
            {
                animator.SetBool("isRun", false);
            }
                
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }

        if (!PauseMenu.isPaused)
        {
            HandleInput();
        }

        bulletText.text = bulletLeft + " / " + totalBullet;
    }

    private void HandleInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (readyToShoot && shooting && !reloading && bulletLeft > 0)
        {
            HandleShoot();
        }

        if (allowReload)
        {
            if (Input.GetKeyDown(KeyCode.R) && bulletLeft < magazineSize)
            {
                if (totalBullet != 0)
                {
                    HandleReload();
                }
            }
        }

        if (totalBullet <= 0)
        {
            allowReload = false;
        }
    }

    private void HandleShoot()
    {
        readyToShoot = false;

        source.PlayOneShot(shootClip);

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = playerCamera.transform.forward + new Vector3(x, y, 0);

        PlayTargetAnimation("Shot", true);
        Instantiate(muzzleFlash, barrel.position, Quaternion.identity, barrel.transform);

        if (isLauncher)
        {
            GameObject grenadeBullet = Instantiate(grenadePrefab, barrel.transform.position, barrel.transform.rotation, barrel.transform);
            grenadeBullet.GetComponent<Rigidbody>().AddForce(barrel.transform.forward  * range, ForceMode.Impulse);
        }
        else
        {
            for (int i = 0; i < bulletPerTap; i++)
            {
                if (Physics.Raycast(playerCamera.transform.position, direction, out hitInfo, range, layerMask))
                {
                    Debug.DrawRay(playerCamera.transform.position, direction * range, Color.red, 3f);
                    //Debug.Log(hitInfo.collider.tag);

                    if (hitInfo.collider.isTrigger)
                    {
                        Debug.Log("Raycast Collide IsTrigger");
                        if (hitInfo.collider.name == "GargoyleHoming(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<GargoyleHoming>().HandleHealth(damage);
                        }
                    }

                    if (hitInfo.collider.tag == "Enemy")
                    {
                        if (hitInfo.collider.name == "Mutant" || hitInfo.collider.name == "Mutant(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<MutantController>().HandleHealth(damage);
                            hitInfo.transform.gameObject.GetComponent<MutantController>().getAttacked = true;
                        }

                        if (hitInfo.collider.name == "Creepy" || hitInfo.collider.name == "Creepy(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<CreepyController>().HandleHealth(damage);
                            hitInfo.transform.gameObject.GetComponent<CreepyController>().getAttacked = true;
                        }

                        if (hitInfo.collider.name == "Skull" || hitInfo.collider.name == "Skull(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<SkullController>().HandleHealth(damage);
                            hitInfo.transform.gameObject.GetComponent<SkullController>().getAttacked = true;
                        }

                        if (hitInfo.collider.name == "Spider" || hitInfo.collider.name == "Spider(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<SpiderController>().HandleHealth(damage);
                            hitInfo.transform.gameObject.GetComponent<SpiderController>().getAttacked = true;
                        }

                        if (hitInfo.collider.name == "Slug" || hitInfo.collider.name == "Slug(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<SlugController>().HandleHealth(damage);
                            hitInfo.transform.gameObject.GetComponent<SlugController>().getAttacked = true;
                        }

                        if (hitInfo.collider.name == "Boss" || hitInfo.collider.name == "Boss(Clone)")
                        {
                            hitInfo.transform.gameObject.GetComponent<GargoyleController>().HandleHealth(damage);
                            //hitInfo.transform.gameObject.GetComponent<GargoyleController>().getAttacked = true;
                        }

                        Instantiate(hitEnemyParticle, hitInfo.point, Quaternion.Euler(hitInfo.normal), barrel.transform);
                    }
                    else
                    {
                        Instantiate(bulletDecay, hitInfo.point, Quaternion.Euler(hitInfo.normal), barrel.transform);
                    }
                }
            }
        }

        bulletShot++;
        bulletLeft--;

        foreach (Transform child in barrel.transform)
        {
            GameObject.Destroy(child.gameObject, 5f);
        }

        //Debug.Log(hitInfo.point);

        Invoke("ResetShoot", timeBetweenShooting);

        //if (bulletShot > 0 && bulletLeft > 0)
        //{
        //    Invoke("HandleShoot", timeBetweenShot);
        //}
    }

    private void HandleReload()
    {
        reloading = true;
        PlayTargetAnimation("Reload", true);
        source.PlayOneShot(reloadClip);

        Invoke("ReloadFinished", reloadTime);
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }

    private void ReloadFinished()
    {
        reloading = false;

        bulletRequire = bulletShot;

        if (bulletRequire > totalBullet)
        {
            bulletLeft += totalBullet;
            totalBullet = 0;
        }
        else
        {
            totalBullet = totalBullet - bulletShot;
            bulletLeft = magazineSize;
        }

        bulletShot = 0;
    }

    public void RestoreAmmo()
    {
        totalBullet = totalGunBullet;
    }

    public void IncreaseAmmo(int ammoNum)
    {
        totalBullet += ammoNum;
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteract)
    {
        animator.applyRootMotion = isInteract;
        animator.SetBool("isInteract", isInteract);
        animator.CrossFadeInFixedTime(targetAnimation, 0.2f);
    }
}
