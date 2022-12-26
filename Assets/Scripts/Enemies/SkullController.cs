using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SkullController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sensor sensor;

    [SerializeField] private int health;
    [SerializeField] private Slider healSlider;

    [Header("Skull Orb")]
    [SerializeField] private float orbSpeed;
    [SerializeField] private float timeBetweenShot;
    [SerializeField] private GameObject orb;
    [SerializeField] private Transform orbPoint;

    [SerializeField] private bool readyToShoot;
    [SerializeField] private bool withinRange;
    [SerializeField] private bool canAttack;

    public bool getAttacked;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        sensor = GetComponent<Sensor>();

        healSlider.maxValue = health;
        readyToShoot = true;
    }

    void Update()
    {
        healSlider.value = health;


        if (health > 0)
        {
            if (getAttacked || sensor.sawPlayer)
            {
                FaceTarget();

                if (sensor.distance >= 25)
                {
                    withinRange = false;
                    canAttack = false;
                    HandlePursue();
                }
                else if (sensor.distance <= 25)
                {
                    withinRange = true;
                    canAttack = true;
                    HandleStop();
                }

                if (withinRange && canAttack && readyToShoot)
                {
                    HandleAttack();
                }
            }
        }
        else
        {
            healSlider.gameObject.SetActive(false);

            navMeshAgent.isStopped = true;
            navMeshAgent.speed = 0;

            animator.applyRootMotion = true;
            animator.SetBool("isDead", true);
            Destroy(gameObject, 3f);
        }
    }

    public void HandleHealth(int damage)
    {
        health -= damage;
        getAttacked = true;

        if (health <= 0)
        {
            return;
        }

        animator.applyRootMotion = true;
        PlayTargetAnimation("GetHit", true);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.applyRootMotion = false;
        }
    }

    private void HandleMove(float speed, bool seeTarget)
    {
        navMeshAgent.isStopped = false;

        if (!seeTarget)
        {
            navMeshAgent.speed = speed;
            animator.SetBool("isRun", false);
        }
        else
        {
            navMeshAgent.speed = speed;
            animator.SetBool("isRun", true);
        }
    }

    private void HandleStop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;

        animator.SetBool("isRun", false);
    }

    private void HandlePursue()
    {
        HandleMove(7f, true);
        navMeshAgent.destination = sensor.player.transform.position;
    }

    private void HandleWander()
    {
        HandleMove(2f, false);
        Vector3 wanderPoint = new Vector3(Random.Range(transform.position.x + 5, transform.position.x - 5), 0, Random.Range(transform.position.z + 5, transform.position.z - 5));
        navMeshAgent.destination = wanderPoint;
    }

    private void HandleAttack()
    {
        HandleStop();
        PlayTargetAnimation("SpitAttack", true);

        var orbProjectile = Instantiate(orb, orbPoint.transform.position, orbPoint.transform.rotation);
        orbProjectile.GetComponent<Rigidbody>().velocity = orbPoint.transform.forward * orbSpeed;

        readyToShoot = false;

        Invoke("ResetShot", timeBetweenShot);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void FaceTarget()
    {
        //Debug.Log("Facing " + sensor.player);
        Vector3 direction = (sensor.player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteract)
    {
        animator.applyRootMotion = isInteract;
        animator.SetBool("isInteract", isInteract);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}