using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    private int health = 100;
    public Text healthText;
    private bool Attacking;
    
    public GameObject enemyColour;
    public GameObject player;
    private Vector3 lastPlayerPosition;
    private Vector3 enemyPosition;
    public int viewDistance = 15;

    public Color myColour;
    private float R;
    private float G;
    private float B;
    private float A;
    public Renderer myRenderer;

    [SerializeField]
    private Transform[] points;
    public NavMeshAgent enemy;
    private int patrolDestinationPoint;
    public int patrolDestinationAmount = 5;
    [SerializeField]
    private float remainingDistance = 0.5f;
    public float searchTime;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = enemyColour.GetComponent<Renderer>();
        patrolDestinationPoint = 0;
        enemy.autoBraking = false;
        enemy = GetComponent<NavMeshAgent>();
        //enemy.autoBraking = false;
        TransitionToState(State.patrolling);
        searchTime = Time.deltaTime - 20f;
    }
    public void TakeDamage(int damage)
    {
        
        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            Destroy(player);
            
            Reset();
        }
        


    }
    public void Reset()
    {
        TransitionToState(State.retreating);
        patrolDestinationPoint = 0;
    }
    public void Patrol()
    {
        enemy.autoBraking = false;
        if (points.Length == 0)
        {
            enabled = false;
            return;
        }

        enemy.destination = points[patrolDestinationPoint].position;
        //cycles through points
        patrolDestinationPoint = (patrolDestinationPoint + 1) % points.Length;
    }

    public void Chase()
    {
        lastPlayerPosition = player.transform.position;
        enemy.SetDestination(lastPlayerPosition);
        //return;
    }
    public void Retreat()
    {
        enemy.autoBraking = true;

        if ((enemyPosition.x == points[0].position.x) && (enemyPosition.z == points[0].position.z))
        {
            TransitionToState(State.patrolling);
        }
        enemy.SetDestination(points[0].position);
        //if (!enemy.pathPending && enemy.remainingDistance < remainingDistance)
        //{
        //    TransitionToState(State.patrolling);
        //}
    }
    public void Search()
    {
        
        enemy.autoBraking = true;
        enemy.SetDestination(lastPlayerPosition);
        float searchDistance = Vector3.Distance(lastPlayerPosition, enemy.transform.position);
        if (searchDistance <= 1) 
        {
            Debug.Log("MATCH");
            
            TransitionToState(State.retreating);
           
        }

    }
     void TransitionToState(State newState)
     {
        state = newState;
        //Debug.Log("Current State: " + state);
        switch (state)
        {
            case State.patrolling:
                Patrol();
                Debug.Log("PATROLLING");
                R = 0.0f;
                G = 128.0f;
                B = 0.0f;
                A = 255f;
                //Attacking = false;
                break;
            case State.retreating:
                Debug.Log("RETREATING");
                Retreat();
                R = 128.0f;
                G = 0.0f;
                B = 128.0f;
                A = 255f;
                //Attacking = false;
                break;
            case State.chasing:
                Debug.Log("CHASING");
                Chase();
                R = 255.0f;
                G = 165.0f;
                B = 0.0f;
                A = 255f;
                //Attacking = false;
                break;
            case State.searching:
                Debug.Log("SEARCHING");
                Search();
                R = 0.0f;
                G = 0.0f;
                B = 255.0f;
                A = 255f;
                //Attacking = false;
                break;
            case State.attacking:
                Debug.Log("ATTACKING");
                if (!Attacking)
                {
                    TakeDamage(15);
                    Attacking = true;
                }
                R = 255.0f;
                G = 0.0f;
                B = 0.0f;
                A = 255f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        healthText.text = health.ToString();
        enemyPosition = enemy.transform.position;
        
        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

        if (distance <= viewDistance)
        {
            //enemy constantly tries to get to that position (vector 3 being set)
            TransitionToState(State.chasing);

            if(distance <= 1)
            {
               
                    TransitionToState(State.attacking);
                   

            }
            else
            {
                Attacking = false;
            }
        }
        if (state == State.chasing)
        {
            if (distance >= viewDistance)
            {
                TransitionToState(State.searching);
               
            }
        }
        if (state == State.searching)
        {

            TransitionToState(State.searching);
          
        }
        if (state == State.retreating)
        {
            TransitionToState(State.retreating);
            if ((enemyPosition.x == points[0].position.x) && (enemyPosition.z == points[0].position.z))
            {
                TransitionToState(State.patrolling);
            }
        }
        else
        {
            if (!enemy.pathPending && enemy.remainingDistance < remainingDistance)
            {
                TransitionToState(State.patrolling);
            }
        }

        myColour = new Color(R, G, B);
        myRenderer.material.color = myColour;


    }
}
