using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SpiderController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sensor sensor;

    [SerializeField] private GameObject explodeDamage;

    private new Rigidbody rigidbody;

    [SerializeField] private int health;
    [SerializeField] private Slider healSlider;

    private float nextActionTime;

    private bool canAttack;
    public bool getAttacked;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        sensor = GetComponent<Sensor>();
        rigidbody = GetComponent<Rigidbody>();

        explodeDamage.SetActive(false);

        healSlider.maxValue = health;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        healSlider.value = health;

        if (animator.GetBool("isInteract"))
        {
            return;
        }

        if (health > 0)
        {
            if (getAttacked || sensor.sawPlayer)
            {
                FaceTarget();

                if (sensor.distance > navMeshAgent.stoppingDistance + .5f)
                {
                    if (!canAttack)
                    {
                        return;
                    }

                    if (sensor.distance < 2.8f)
                    {
                        HandleAttack();
                    }
                    else
                    {
                        HandlePursue();
                    }
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
        HandleMove(8f, true);
        navMeshAgent.destination = sensor.player.transform.position;
    }

    private void HandleWander()
    {
        HandleMove(2f, false);
        Vector3 wanderPoint = new Vector3(Random.Range(transform.position.x + 5, transform.position.x - 5), 0, Random.Range(transform.position.z + 5, transform.position.z - 5));
        navMeshAgent.destination = wanderPoint;

        //Debug.Log(wanderPoint);
    }

    private void HandleAttack()
    {
        HandleStop();
        PlayTargetAnimation("ExplodeAttack", true);

        explodeDamage.SetActive(true);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.applyRootMotion = false;
        }

        Invoke("Explode", nextActionTime);
    }

    private void Explode()
    {
        canAttack = false;

        animator.SetBool("isDead", true);

        Destroy(gameObject, 3f);
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
        animator.CrossFadeInFixedTime(targetAnimation, 0.2f);
    }
}