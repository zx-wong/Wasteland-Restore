using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] public GameObject player;

    [SerializeField] [Range(0, 360)] public float angle;
    [SerializeField] public float visionRadius;
    [SerializeField] public float hearRadius;
    [SerializeField] public float fleeRadius;

    public float distance;
    public float fleeDistance;

    [SerializeField] public LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;

    [SerializeField] public bool sawPlayer;
    [SerializeField] public bool hearPlayer;

    [SerializeField] public bool withinFleeRange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(VisionRoutine(.2f));
        StartCoroutine(FleeRoutine(.2f));
        //StartCoroutine(HearRoutine());
    }

    void Update()
    {
        //VisionCheck();
        //HearingCheck();
    }

    void VisionCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    sawPlayer = true;
                    distance = distanceToTarget;
                }
                else
                {
                    sawPlayer = false;
                }
            }
            else
            {
                sawPlayer = false;
            }
        }
        else if (sawPlayer)
        {
            sawPlayer = false;
        }
    }

    void HearingCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, hearRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            float hearDistance = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, hearDistance, obstructionMask))
            {
                //if (firstPersonController.isWalking)
                //{
                //    hearPlayer = true;
                //    FaceTarget(target.transform.position);
                //}
            }
            else
            {
                hearPlayer = false;
            }
        }
    }

    void FleeCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, fleeRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            fleeDistance = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, fleeDistance, obstructionMask))
            {
                withinFleeRange = true;
            }
            else
            {
                withinFleeRange = false;
            }
        }
        else if (withinFleeRange)
        {
            withinFleeRange = false;
        }
    }

    IEnumerator VisionRoutine(float delay)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return true;
            VisionCheck();
        }
    }

    IEnumerator HearRoutine(float delay)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return true;
            HearingCheck();
        }
    }

    IEnumerator FleeRoutine(float delay)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return true;
            FleeCheck();
        }
    }
}
