using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SlugController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sensor sensor;

    [SerializeField] private int health;
    [SerializeField] private Slider healSlider;

    private bool readyToShoot;
    public bool getAttacked;

    [Header("Slug Orb")]
    [SerializeField] private int orbPerShot;
    [SerializeField] private float orbSpeed;
    [SerializeField] private float timeBetweenShot;
    [SerializeField] private GameObject orb;
    [SerializeField] private Transform orbPoint;

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

        if (animator.GetBool("isInteract"))
        {
            return;
        }

        if (health > 0)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                animator.SetBool("isWalk", false);
            }

            if (sensor.sawPlayer || getAttacked)
            {
                if (sensor.withinFleeRange)
                {
                    HandleFlee();
                }
                else
                {
                    HandleStop();
                    transform.LookAt(sensor.player.transform.position);

                    //FaceTarget();
                }

                if (sensor.distance > 30)
                {
                    HandlePursue();
                }
                else if (sensor.distance <= 30 && readyToShoot)
                {
                    FaceTarget();
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
        animator.SetBool("isWalk", true);

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
    }

    private void HandlePursue()
    {
        navMeshAgent.destination = sensor.player.transform.position;
    }

    private void HandleFlee()
    {
        HandleMove(7f, true);
        Vector3 fleeDirection = transform.position - sensor.player.transform.position;
        Vector3 targetFlee = transform.position + fleeDirection;

        navMeshAgent.destination = targetFlee;
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
        PlayTargetAnimation("RangeAttack", true);

        StartCoroutine(Shoot());

        readyToShoot = false;

        Invoke("ResetShot", timeBetweenShot);
    }

    IEnumerator Shoot()
    {
        while (orbPerShot < 3)
        {
            var orbProjectile = Instantiate(orb, orbPoint.transform.position, orbPoint.rotation);
            orbProjectile.GetComponent<Rigidbody>().velocity = orbPoint.transform.forward * orbSpeed;

            yield return new WaitForSeconds(.15f);
            orbPerShot += 1;
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;

        orbPerShot = 0;
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
