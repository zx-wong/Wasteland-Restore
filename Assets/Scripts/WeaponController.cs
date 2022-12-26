using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Model;

public class WeaponController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Weapon Settings")]
    public WeaponSettings weaponSettings;
    public bool isInitialised;

    private Vector3 weaponRotation;
    private Vector3 weaponRotationVelocity;

    private Vector3 targetWeaponRotation;
    private Vector3 targetWeaponRotationVelocity;    
    
    private Vector3 weaponMovementRotation;
    private Vector3 weaponMovementRotationVelocity;

    private Vector3 targetWeaponMovementRotation;
    private Vector3 targetWeaponMovementRotationVelocity;

    private bool isGroundTrigger;
    private bool isFallTrigger;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        weaponRotation = transform.localRotation.eulerAngles;
    }

    public void Initialise (PlayerController PlayerController)
    {
        playerController = PlayerController;
        isInitialised = true;
    }

    private void Update()
    {
        if (!isInitialised)
        {
            return;
        }

        animator.speed = playerController.weaponAnimationSpeed;

        CalculateWeaponSway();
        SetWeaponAnimation();

        if (!playerController.isGround && !isGroundTrigger)
        {
            isGroundTrigger = true;
        }
        else if (!playerController.isGround && isGroundTrigger)
        {
            isGroundTrigger = false;
        }
    }

    public void TriggerJump()
    {
        isGroundTrigger = false;
        animator.CrossFade("Jump", .1f);
    }

    private void CalculateWeaponSway()
    {
        // Sway
        targetWeaponRotation.y += weaponSettings.swayAmount * (weaponSettings.invertXSway ? -playerController.viewInput.x : playerController.viewInput.x) * Time.deltaTime;
        targetWeaponRotation.x += weaponSettings.swayAmount * (weaponSettings.invertYSway ? playerController.viewInput.y : -playerController.viewInput.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -weaponSettings.swayXClamp, weaponSettings.swayXClamp);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -weaponSettings.swayYClamp, weaponSettings.swayYClamp);
        targetWeaponRotation.z = targetWeaponRotation.y;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, weaponSettings.resetSmooth);
        weaponRotation = Vector3.SmoothDamp(weaponRotation, targetWeaponRotation, ref weaponRotationVelocity, weaponSettings.swaySmooth);

        // Movement Sway
        targetWeaponMovementRotation.z = weaponSettings.swayMovementX * (weaponSettings.invertXMovementSway ? -playerController.moveInput.x : playerController.moveInput.x) * Time.deltaTime;
        targetWeaponMovementRotation.x = weaponSettings.swayMovementY * (weaponSettings.invertYMovementSway ? playerController.moveInput.y : -playerController.moveInput.y) * Time.deltaTime;

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, weaponSettings.resetSmooth);
        weaponMovementRotation = Vector3.SmoothDamp(weaponMovementRotation, targetWeaponMovementRotation, ref weaponMovementRotationVelocity, weaponSettings.swayMovementSmooth);

        transform.localRotation = Quaternion.Euler(weaponRotation + weaponMovementRotation);
    }

    private void SetWeaponAnimation()
    {
        animator.SetBool("isSprint", playerController.isSprint);
    }
}
