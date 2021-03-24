using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    enum State
    {
        patrolling, 
        chasing, 
        searching,
        attacking,
        retreating
    }

    static State state;

    public GameObject player;
    private Vector3 playerPosition;
    private Vector3 LastPlayerPosition;
    private Vector3 enemyPosition;
    

    [SerializeField]
    private Transform[] points;
    public NavMeshAgent enemy;
    private int patrolDestinationPoint = 0;
    [SerializeField]
    private float remainingDistance = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        enemy.autoBraking = false;
        GoToNextPoint();
    }
    void GoToNextPoint()
    {
        if (points.Length == 0)
        {
            enabled = false;
            return;
        }

        enemy.destination = points[patrolDestinationPoint].position;
        //cycles through points
        patrolDestinationPoint = (patrolDestinationPoint + 1) % points.Length;
    }
    static void TransitionToState(State newState)
    {
        state = newState;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
        //Debug.Log(distance);
        //constantly sets a vector 3 to players current position
        playerPosition = player.transform.position;
        enemyPosition = enemy.transform.position;
        //Debug.Log("player: " + playerPosition);
        //Debug.Log("Enemy: " + enemyPosition);


        if (distance <= 15)
        {
            //enemy constantly tries to get to that position (vector 3 being set)
            Debug.Log("Player and enemy are close");
            enemy.SetDestination(playerPosition);
        }
        else
        {
            if (!enemy.pathPending && enemy.remainingDistance < remainingDistance)
            {
                GoToNextPoint();
            }
        }
  
    }
}
