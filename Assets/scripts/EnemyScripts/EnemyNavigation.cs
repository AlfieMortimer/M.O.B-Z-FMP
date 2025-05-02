using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class EnemyNavigation : NetworkBehaviour
{
    public GameObject movementTarget;
    GameObject[] playerObjects;
    public EnemySpawner mySpawner;

    public Animator animator;

    bool inSpawnRoom;
    public NavMeshAgent agent;
    private void Start()
    {
       agent = GetComponent<NavMeshAgent>();
        if(inSpawnRoom == false)
        {
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            movementTarget = GetClosestPlayer(playerObjects);
        }
        else
        {
            movementTarget = mySpawner.spawnRoomExit;
        }
    }
    private void Update()
    {
        SpawnChecks();
    }

    public void SpawnChecks()
    {
        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);


        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Spawn")
        {
            agent.isStopped = true;

        }
        else
        {
            agent.isStopped = false;

            if (inSpawnRoom == false)
            {
                movementTarget = GetClosestPlayer(playerObjects);
                agent.destination = movementTarget.transform.position;
            }
        }
    }

    public void FindMySpawn(bool spawnroom, EnemySpawner spawner)
    {
        inSpawnRoom = spawnroom;
        mySpawner = spawner;
    }

    GameObject GetClosestPlayer(GameObject[] players)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in players)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

}
