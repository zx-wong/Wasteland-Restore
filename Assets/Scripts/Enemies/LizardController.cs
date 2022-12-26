using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class LizardController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sensor sensor;

    [SerializeField] private int health;

    [SerializeField] private float remainingDistance;
    [SerializeField] private float nextAttackTime;

    [SerializeField] private bool canAttack = false;
    //[SerializeField] private bool attacked;
    [SerializeField] private bool nextAttack;

    [SerializeField] private bool wander = false;

    [SerializeField] private bool combo = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        sensor = GetComponent<Sensor>();

        //attacked = false;
        nextAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        remainingDistance = navMeshAgent.remainingDistance;

        if (animator.GetBool("isInteract"))
        {
            return;
        }

        if (health > 0)
        {
            if (sensor.sawPlayer)
            {
                FaceTarget();
                wander = false;

                if (sensor.distance > navMeshAgent.stoppingDistance + .5f)
                {
                    canAttack = false;
                    HandlePursue();
                }
                else
                {
                    canAttack = true;

                    if (canAttack && nextAttack)
                    {
                        HandleAttack();
                    }
                }
            }
            else
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    animator.SetBool("isWalk", false);
                }

                if (!wander)
                {
                    HandleWander();
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
        HandleMove(6f, true);
        navMeshAgent.destination = sensor.player.transform.position;
    }

    private void HandleWander()
    {
        HandleMove(2f, false);
        Vector3 wanderPoint = new Vector3(Random.Range(transform.position.x + 5, transform.position.x - 5), 0, Random.Range(transform.position.z + 5, transform.position.z - 5));
        navMeshAgent.destination = wanderPoint;

        wander = true;
        //Debug.Log(wanderPoint);
    }

    private void HandleAttack()
    {
        if (!combo)
        {
            PlayTargetAnimation("MutantPunch", true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("MutantPunch"))
            {
                //attacked = true;
            }
        }
        else
        {
            PlayTargetAnimation("MutantSwiping", true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("MutantSwiping"))
            {
                //attacked = false;
            }
        }
    }

    private void ResetAttack()
    {
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
}