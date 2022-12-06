using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private Transform movePositionTransform;
    private Rigidbody rb;
    private CapsuleCollider cldr;

    #region animations
    Animator animator;
    SkinnedMeshRenderer skinnedMesh;
    public float runspeed = 10.0f;
    #endregion

    #region navigation
    private NavMeshAgent navMeshAgent;
    public float maintainedDistance = 1.0f;
    public float speed = 10.0f;
    private Vector3 lastPos;
    #endregion

    #region behavior
    public float attackCD = 10f;
    private float attackUsed;
    private bool punchedWithLeft;
    bool active = false;
    private bool living = false;
    #endregion

    #region damage
    public float enemyHitRange = 10f;
    public float health = 10f;
    public float damage = 10f;
    #endregion

    #region audio
    private AudioSource myNoises;
    public AudioClip death;
    public AudioClip battle;
    bool died = true;
    #endregion

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cldr = GetComponent<CapsuleCollider>();
        myNoises = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        if(living)
        {
            #region animation
            Vector3 curPos = rb.transform.position;
            animator.SetFloat("Vertical", Vector3.Project(curPos - lastPos, rb.transform.forward).magnitude / speed * 200);
            animator.SetFloat("Horizontal", Vector3.Project(curPos - lastPos, rb.transform.right).magnitude / speed * 200);
            lastPos = curPos;
            if (navMeshAgent.velocity.magnitude >= runspeed)
            {
                animator.SetBool("running", true);
            }
            else
            {
                animator.SetBool("running", false);
            }
            #endregion


            #region navigation
            if (Vector3.Distance(curPos, movePositionTransform.position) >= maintainedDistance)
            {
                navMeshAgent.destination = movePositionTransform.position;
                Debug.Log("moving");
            }
            else
            {
                navMeshAgent.velocity = Vector3.zero;
                animator.SetFloat("Vertical", 0);
                animator.SetFloat("Horizontal", 0);
                if (attackUsed <= 0)
                {
                    attack();
                    attackUsed = attackCD;
                }
                else
                {
                    attackUsed--;
                }
            }
            #endregion

            if (health <= 0)
            {
                animator.SetTrigger("knockdown");
                living = false;
                died = false;
            }

            if(!myNoises.isPlaying)
            {
                myNoises.clip = battle;
                myNoises.Play();
            }
        }
        else if(!living && !died)
        {
            myNoises.Stop();
            myNoises.clip = death;
            myNoises.loop = false;
            myNoises.PlayDelayed(1f);
            died = true;

            GameObject[] door;
            door = GameObject.FindGameObjectsWithTag("Second Door");
            foreach (GameObject dooro in door)
            {
                dooro.SendMessage("openSesame");
            }
        }
    }

    private void gateOpened()
    {
        Debug.Log("I live");
        living = true;
    }

    private void attack()
    {
        if(punchedWithLeft)
        {
            animator.SetTrigger("punch_R");
            punchedWithLeft = false;
        }    
        else
        {
            animator.SetTrigger("punch_L");
            punchedWithLeft = true;
        }
        Debug.Log("EnemytryingtoHit");
        RaycastHit hit;
        active = Physics.Raycast(cldr.transform.position + cldr.center*rb.gameObject.transform.localScale.y, rb.transform.forward, out hit, enemyHitRange);
        Debug.DrawRay(cldr.transform.position+cldr.center * rb.gameObject.transform.localScale.y, rb.transform.forward* enemyHitRange, Color.green, 2);
        if (active == true)
        {
            Debug.Log("EnemyHitSomething");
            if (hit.collider.tag == "player")
            {
                hit.transform.SendMessage("HitByEnemy", damage);
            }

        }
    }

    public void HitByPlayer(float incomingDamage)
    {
        health -= incomingDamage;
        Debug.Log("enemyHit");
    }

}
