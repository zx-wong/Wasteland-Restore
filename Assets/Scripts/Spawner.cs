using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform enemyTransform;

    [SerializeField] GameObject[] enemyPrefab;

    [SerializeField] public int totalEnemy, rangeX, rangeZ;
    private int enemyCount, randomNum;

    private void Start()
    {
        //if (enemyPrefab.Length == 0)
        //{
        //    return;
        //}
    }

    public void StartWave(int totalEnemy)
    {
        StartCoroutine(SpawnEnemy(totalEnemy));
    }

    private int RandomEnemy()
    {
        randomNum = Random.Range(0, enemyPrefab.Length);

        return randomNum;
    }

    IEnumerator SpawnEnemy(int totalEnemy)
    {
        while(enemyCount < totalEnemy)
        {
            var randomX = Random.Range(transform.position.x + (rangeX / 2), transform.position.x - (rangeX / 2));
            var randomZ = Random.Range(transform.position.z + (rangeZ / 2), transform.position.z - (rangeZ / 2));

            GameObject enemy = Instantiate(enemyPrefab[RandomEnemy()], new Vector3(randomX, 0, randomZ), Quaternion.identity, enemyTransform);

            if (enemy.gameObject.name == "Mutant" || enemy.gameObject.name == "Mutant(Clone)")
            {
                enemy.GetComponent<MutantController>().getAttacked = true;
            }

            if (enemy.gameObject.name == "Creepy" || enemy.gameObject.name == "Creepy(Clone)")
            {
                enemy.GetComponent<CreepyController>().getAttacked = true;
            }

            if (enemy.gameObject.name == "Skull" || enemy.gameObject.name == "Skull(Clone)")
            {
                enemy.GetComponent<SkullController>().getAttacked = true;
            }

            if (enemy.gameObject.name == "Spider" || enemy.gameObject.name == "Spider(Clone)")
            {
                enemy.GetComponent<SpiderController>().getAttacked = true;
            }

            if (enemy.gameObject.name == "Slug" || enemy.gameObject.name == "Slug(Clone)")
            {
                enemy.GetComponent<SlugController>().getAttacked = true;
            }

            yield return new WaitForSeconds(.15f);
            enemyCount += 1;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, 1, rangeZ));
    }
}
