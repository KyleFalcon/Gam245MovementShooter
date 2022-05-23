using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Ground, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Transform weaponAttackPoint;
    public float meleeAttackRange;
    public LayerMask enemyLayers;
    public Animator animator;

    public float chargeTime;
    private float charge;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        charge = chargeTime;
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground)) walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);


        if (this.CompareTag("Melee"))
        {
            MeleeAttack();
            //alreadyAttacked = false;
        }
        else if (this.CompareTag("Sniper"))
        {
            charge -= Time.deltaTime;
            Debug.Log(charge);
            if (charge < 0)
            {
                Debug.Log("shoot");
                SniperAttack();
            }
        }

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void MeleeAttack()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(weaponAttackPoint.position, meleeAttackRange, enemyLayers);
        animator.SetBool("Swinging", true);

        foreach(Collider player in hitPlayer)
        {
            Debug.Log("Hit " + player.name);
        }
    }

    public void SniperAttack()
    {
        RaycastHit hit;
        Debug.DrawRay(weaponAttackPoint.position, weaponAttackPoint.forward * attackRange, Color.red);
        bool shotSomething = Physics.Raycast(weaponAttackPoint.position, weaponAttackPoint.forward, out hit, attackRange);
        
        if (shotSomething) Debug.Log(hit.transform.name);

    }

    private void ResetAttack()
    {
        animator.SetBool("Swinging", false);
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(weaponAttackPoint.position, meleeAttackRange);   
    }
}
