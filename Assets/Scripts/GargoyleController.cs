using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class GargoyleController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sensor sensor;
    [SerializeField] private GameObject meleeCollider;

    [SerializeField] public int maxHealth;
    [SerializeField] public int health;

    [SerializeField] private float nextAttackTime;

    public bool bossDead;
    private bool gotPattern;

    private bool getAttacked;
   
    private bool nextAttack;
    private bool readyToShoot;

    private bool canSpawn;

    private bool attackAllow;

    [Header("Pattern 2")]
    public Transform ballPoint;
    public Transform beamPoint;

    public GameObject ballPrefab;
    public GameObject beamPrefab;
    public GameObject ballSpell;
    public GameObject beamSpell;
    public GameObject flightSpell;

    private Vector3 flightSpellOffset;

    [Header("Pattern 3")]
    public Transform bulletHellTransform;

    public GameObject bulletHellPrefab;

    private int bulletHellCount = 0;
    private int count = 0;

    [Header("Pattern 3")]
    public Transform homingTransform;

    public GameObject homingPrefab;

    private bool goForAttack = false;
    private bool startCast = false;
    private bool casting = false;

    private bool meleeAttack;
    private bool medianAttack;
    private bool longAttack;
    private bool attackComplete;

    [Header("Pattern 1")]
    [SerializeField] private int radialNumber;
    [SerializeField] private float radialSpeed;
    private const float radialRadius = 1F;
    private Vector3 radialTransform;
    [SerializeField] private GameObject radialPrefab;
    [SerializeField] private GameObject fallPrefab;

    [SerializeField] private bool canRadial;
    [SerializeField] private bool canFall;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        sensor = GetComponent<Sensor>();

        health = maxHealth;

        meleeCollider.SetActive(false);

        nextAttack = true;
        readyToShoot = true;

        canSpawn = true;

        attackComplete = true;

        canRadial = true;
        canFall = true;
    }

    void Update()
    {
        if (animator.GetBool("isInteract"))
        {
            return;
        }

        if (health > 0)
        {
            if (canRadial)
            {
                RadialBullet(Random.Range(0, 17));
                canRadial = false;
            }

            if (canFall)
            {
                FallBullet();
                canFall = false;
            }

            if (sensor.sawPlayer || getAttacked)
            {
                if (sensor.distance >= 10f)
                {
                    FaceTarget();
                    HandlePursue();
                }
                else if (sensor.distance <= 4f)
                {
                    FaceTarget();
                    HandleAttack();
                }
            }

                //    if (sensor.distance >= 4f)
                //    {
                //        if (sensor.distance <= 15f)
                //        {
                //            medianAttack = true;
                //        }
                //        else if (sensor.distance >= 15f)
                //        {
                //            longAttack = true;
                //        }
                //    }
                //    else
                //    {
                //        meleeAttack = true;
                //    }

                //    attackComplete = false;
                //}

                //if (meleeAttack)
                //{
                //    HandleAttack();
                //}
                //else if (medianAttack)
                //{
                //    HandleBulletHell();
                //}
                //else if (longAttack)
                //{
                //    HomingBullet();
                //}
            }
        else
        {
            //Debug.Log("Boss Dead");
            navMeshAgent.isStopped = true;
            navMeshAgent.speed = 0;

            animator.applyRootMotion = true;
            animator.SetBool("isDead", true);

            SceneManager.LoadScene("WinScene");

            //Destroy(gameObject, 3f);
        }
    }

    private void RadialBullet(int radialNum)
    {
        radialTransform = transform.position;
        radialTransform = new Vector3(radialTransform.x, radialTransform.y + 0.5f, radialTransform.z);

        float angleStep = 360f / radialNum;
        float angle = 0;

        for (int i = 0; i <= radialNum - 1; i++)
        {
            float prefabXDir = radialTransform.x + Mathf.Sin((angle * Mathf.PI) / 180) * radialRadius;
            float prefabYDir = radialTransform.y + Mathf.Cos((angle * Mathf.PI) / 180) * radialRadius;

            Vector3 projectileVector = new Vector3(prefabXDir, prefabYDir, 0);
            Vector3 projectileMoveDirection = (projectileVector - radialTransform).normalized * radialSpeed;

            GameObject tempObj = Instantiate(radialPrefab, radialTransform, Quaternion.identity);
            tempObj.GetComponent<Rigidbody>().velocity = new Vector3(projectileMoveDirection.x, 0, projectileMoveDirection.y);

            Destroy(tempObj, 10f);

            angle += angleStep;
        }

        StartCoroutine(ResetBullet());
    }

    IEnumerator ResetBullet()
    {
        yield return new WaitForSeconds(1f);
        canRadial = true;
    }

    private void FallBullet()
    {
        float x = Random.Range(-0.05f, 0.05f);
        float z = Random.Range(-0.05f, 0.05f);

        Vector3 position = sensor.player.transform.position + new Vector3(x, 10, z);

        GameObject tempObj = Instantiate(fallPrefab, position, Quaternion.identity);
        tempObj.GetComponent<Rigidbody>().transform.position = position;
        tempObj.GetComponent<Rigidbody>().useGravity = true;
        
        StartCoroutine(ResetFallBullet());
    }

    IEnumerator ResetFallBullet()
    {
        yield return new WaitForSeconds(5f);
        canFall = true;
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(5f);
        attackComplete = true;
    }

    //private void RandomPattern()
    //{
    //    var patternNum = Random.Range(1, 2);
    //    Debug.Log(patternNum);

    //    switch (patternNum)
    //    {
    //        case 0:
    //            patternOne = true;
    //            patternTwo = false;
    //            patternThree = false;
    //            break;
    //        case 1:
    //            patternOne = false;
    //            patternTwo = true;
    //            patternThree = false;
    //            break;
    //        case 2:
    //            patternOne = false;
    //            patternTwo = false;
    //            patternThree = true;
    //            break;
    //        default:
    //            break;
    //    }

    //    gotPattern = true;
    //}

    public void HandleHealth(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            return;
        }

        animator.applyRootMotion = true;
        PlayTargetAnimation("Hit", true);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.applyRootMotion = false;
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
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = 7f;
        animator.SetBool("isWalk", true);
        navMeshAgent.destination = sensor.player.transform.position;
    }

    private void HandleAttack()
    {
        HandleStop();
        meleeCollider.SetActive(true);
        PlayTargetAnimation("Attack01", true);
        nextAttack = false;

        Invoke("ResetAttack", nextAttackTime);
    }

    private void HandleBulletHell()
    {
        var center = bulletHellTransform.transform.position;

        while (bulletHellCount <= 5)
        {
            for (int i = 0; i < 16; i++)
            {
                var radians = i * 2 * Mathf.PI / 16;

                var vertical = Mathf.Sin(radians);
                var horizontal = Mathf.Cos(radians);

                var spawnDir = new Vector3(horizontal, 0, vertical);

                var spawnPos = center + spawnDir * 1.5f;

                GameObject bulletHellBullet = Instantiate(bulletHellPrefab, new Vector3(spawnPos.x, spawnPos.y + 0.5f, spawnPos.z), bulletHellTransform.rotation, bulletHellTransform);
                bulletHellBullet.GetComponent<Rigidbody>().AddForce(spawnPos * 3f, ForceMode.Impulse);
            }

            bulletHellCount++;

            if (bulletHellCount >= 5)
            {
                bulletHellCount = 0;
                medianAttack = false;

                StartCoroutine(AttackDelay());
            }
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;

        gotPattern = false;
    }

    private void ResetAttack()
    {
        meleeCollider.SetActive(false);
    }

    private void PrecastStart()
    {
        HandleStop();
        PlayTargetAnimation("Cast01", true);
        ballSpell = Instantiate(ballPrefab, ballPoint.position, ballPoint.rotation);
        ballSpell.transform.parent = ballPoint;
        Destroy(ballSpell, 3.0f);

        Invoke("CastStart", 2.5f);
    }

    private void CastStart()
    {
        HandleStop();
        startCast = false;
        animator.SetBool("isCast", true);
        beamSpell = Instantiate(beamPrefab, beamPoint.position, Quaternion.Euler(beamPoint.rotation.x + 170, beamPoint.rotation.y + 180, 0));
        beamSpell.transform.parent = beamPoint;

        Invoke("CastEnd", 10f);
    }

    private void CastEnd()
    {
        HandleStop();
        animator.SetBool("isCastEnd", true);
        
        if (beamSpell)
        {
            ParticleSystem ps = beamSpell.GetComponent<ParticleSystem>();
            var em = ps.emission;
            em.enabled = false;

            foreach (Transform child in beamSpell.transform)
            {
                if (child.gameObject.GetComponent<ParticleSystem>())
                {
                    ParticleSystem cps = beamSpell.GetComponent<ParticleSystem>();
                    var cem = cps.emission;
                    cem.enabled = false;
                    //break;
                }

                if (child.gameObject.GetComponent<Light>())
                {
                    child.gameObject.GetComponent<Light>().enabled = false;
                }

                Destroy(child.gameObject);
            }

            Destroy(beamSpell, 3.0f);
        }

        casting = false;
    }

    //private void FlightCast()
    //{
    //    GameObject newSpell = Instantiate(flightSpell, transform.position, Quaternion.identity);
    //    newSpell.transform.eulerAngles = new Vector3(-90, 0, 0);
    //    newSpell.transform.position = newSpell.transform.position + flightSpellOffset;
    //    Destroy(newSpell, 9.0f);
    //}

    private void HomingBullet()
    {
        if (canSpawn && count < 1)
        {
            StartCoroutine(Delay());
            GameObject homingBullet = Instantiate(homingPrefab, homingTransform.transform.position, homingTransform.transform.rotation, homingTransform);
            homingBullet.transform.LookAt(sensor.player.transform.position);
            StartCoroutine(HomingTrack(homingBullet));
            canSpawn = false;

            count++;
        }

        if (count >= 1)
        {
            meleeAttack = false;
            medianAttack = false;
            longAttack = false;
            count = 0;
            StartCoroutine(AttackDelay());
        }
    }

    private IEnumerator HomingTrack(GameObject homingBullet)
    {
        while (Vector3.Distance(sensor.player.transform.position, homingBullet.transform.position) > .3f)
        {
            homingBullet.transform.position += (sensor.player.transform.position - homingBullet.transform.position).normalized * 3f * Time.deltaTime;
            homingBullet.transform.LookAt(sensor.player.transform.position);

            yield return null;
        }

        Destroy(homingBullet);
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(.3f);
        canSpawn = true;
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