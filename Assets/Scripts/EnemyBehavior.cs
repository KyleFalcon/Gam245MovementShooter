using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public Transform playerHead;
    public LayerMask Ground, whatIsPlayer;
    public Transform hand;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float attackDamage;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public float lookSpeed = 1f;

    public Transform weaponAttackPoint;
    public float meleeAttackRange;
    public LayerMask enemyLayers;
    public Animator animator;


    PlayerHealth playerHealth;
    public GameObject projectile;
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    private float laserMaxLength = 5f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        playerHead = player.GetChild(0).GetChild(1);
        hand = transform.GetChild(1);
        Debug.Log(hand.name);


        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
       
        
        if (this.CompareTag("Sniper"))
            laserLineRenderer.enabled = true;
        else
        {
            laserLineRenderer.enabled = false;
        }
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

        //delayedLook(transform, player, lookSpeed);
        transform.LookAt(player);
        hand.LookAt(playerHead);
        ShootLaserFromTargetPosition(weaponAttackPoint.position, weaponAttackPoint.forward + new Vector3(0, 0.0015f, 0), attackRange);

        if (!alreadyAttacked)
        {
            //Attack Code Goes Here

            if (this.CompareTag("Melee"))
            {
                MeleeAttack();
            }
            else if (this.CompareTag("Sniper"))
            {
                SniperAttack();
            }
            else if (this.CompareTag("Grenadier"))
            {
                GrenadeAttack();
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void MeleeAttack()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(weaponAttackPoint.position, meleeAttackRange, enemyLayers);
        animator.SetBool("Swinging", true);
        foreach (Collider player in hitPlayer)
        {
            Debug.Log("Hit " + player.name);
            playerHealth.takeDamage(attackDamage);
        }
    }

    public void SniperAttack()
    {
        RaycastHit hit;
        bool shotSomething = Physics.Raycast(weaponAttackPoint.position, weaponAttackPoint.forward, out hit, attackRange);
        if (shotSomething) {
            Debug.Log(hit.transform.name);   
            if (hit.transform.CompareTag("Player"))
            {
                playerHealth.takeDamage(attackDamage);
            }
        }

    }

    public void GrenadeAttack()
    {
        GameObject grenade = Instantiate(projectile, weaponAttackPoint.position, weaponAttackPoint.rotation);
        Grenade grenadeScript = grenade.GetComponent<Grenade>();
        grenadeScript.damage = attackDamage;

        Rigidbody grenadeRB = grenade.GetComponent<Rigidbody>();
        grenadeRB.AddForce(transform.forward * 15f + transform.up * 2f, ForceMode.Impulse);

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
    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            endPosition = raycastHit.point;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }

    void delayedLook(Transform transform, Transform target, float speed)
    {
        Vector3 relativePos = (target.position - transform.position);
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, speed * Time.deltaTime);
    }
}