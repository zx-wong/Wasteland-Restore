using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CreepyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sensor sensor;
    [SerializeField] private GameObject creepyCollider;

    [SerializeField] private int health;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private float nextAttackTime;
    private bool nextAttack = false;

    [SerializeField] private float nextShotTime;
    private bool nextShot = false;

    public bool getAttacked;

    [Header("Spit")]
    [SerializeField] private GameObject spitObj;
    [SerializeField] private Transform spitPoint;
    [SerializeField] private float spitSpeed;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        sensor = GetComponent<Sensor>();

        creepyCollider.SetActive(false);
        healthSlider.maxValue = health;

        nextAttack = true;
        nextShot = true;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health;

        if (animator.GetBool("isInteract"))
        {
            return;
        }

        if (health > 0)
        {
            if (sensor.sawPlayer || getAttacked)
            {
                FaceTarget();

                if (sensor.distance > 35f)
                {
                    HandlePursue();
                }
                
                if (sensor.distance < 35f && sensor.distance > 10f && nextShot)
                {
                    HandleRangeAttack();
                }

                if (sensor.distance < 5f && nextAttack)
                {
                    HandleMeleeAttack();
                }
            }
            else
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {                
                    animator.SetBool("isWalk", false);
                }
            }
        }
        else
        {
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
            animator.SetBool("isWalk", true);
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

        animator.SetBool("isWalk", false);
    }

    private void HandlePursue()
    {
        HandleMove(4f, true);
        navMeshAgent.destination = sensor.player.transform.position;
    }

    private void HandleWander()
    {
        HandleMove(2f, false);
        Vector3 wanderPoint = new Vector3(Random.Range(transform.position.x + 5, transform.position.x - 5), 0, Random.Range(transform.position.z + 5, transform.position.z - 5));
        navMeshAgent.destination = wanderPoint;
        //Debug.Log(wanderPoint);
    }

    private void HandleRangeAttack()
    {
        HandleStop();
        PlayTargetAnimation("Range", true);

        var spitProjectile = Instantiate(spitObj, spitPoint.transform.position, spitPoint.rotation);
        spitProjectile.GetComponent<Rigidbody>().velocity = spitPoint.transform.forward * spitSpeed;

        nextShot = false;

        Invoke("ResetShot", nextAttackTime);
    }    
    
    private void HandleMeleeAttack()
    {
        HandleStop();
        PlayTargetAnimation("Melee", true);
        creepyCollider.SetActive(true);
        nextAttack = false;

        Invoke("ResetAttack", nextAttackTime);
    }

    private void ResetShot()
    {
        nextShot = true;
    }

    private void ResetAttack()
    {
        creepyCollider.SetActive(false);
        nextAttack = true;
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

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
    }
}