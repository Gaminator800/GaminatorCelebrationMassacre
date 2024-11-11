using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CyborgController : MonoBehaviour
{
    public GameObject target;
    public float WalkingSpeed;
    public float RunningSpeed;
    public GameObject ragdoll;
    Animator anim;
    NavMeshAgent agent;

    enum STATE { IDLE, WANDER, ATTACK, CHASE, DEAD }
    STATE state = STATE.IDLE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
    }

    void TurnOffTriggers()
    {
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsDead", false);
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(target.transform.position, this.transform.position);
    }

    bool CanSeePlayer()
    {
        if(DistanceToPlayer() < 10)
            return true;
        return false;
    }
    
    bool ForgetPlayer()
    {
        if (DistanceToPlayer() > 20)
            return true;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Random.Range(0, 10) < 5)
            {
                GameObject rd = Instantiate(ragdoll, this.transform.position, this.transform.rotation);
                rd.transform.Find("DEF-spine").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
                Destroy(this.gameObject);
            }
            else
            {
                TurnOffTriggers();
                anim.SetBool("IsDead", true);
                state = STATE.DEAD;
            }
            return;
                
        }
        if(target == null)
        {
            target = GameObject.FindWithTag("Player");
            return;
        }
        switch (state)
        {
            case STATE.IDLE:
                if (CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0, 5000) < 5)
                    state = STATE.WANDER;
                break;
            case STATE.WANDER:
                if (!agent.hasPath)
                {
                    float newX = this.transform.position.x + Random.Range(-5, 5);
                    float newZ = this.transform.position.z + Random.Range(-5, 5);
                    float newY = this.transform.position.z + Random.Range(-5, 5);
                    Vector3 dest = new Vector3(newX, newY, newZ);
                    agent.SetDestination(dest);
                    agent.stoppingDistance = 0;
                    TurnOffTriggers();
                    agent.speed = WalkingSpeed;
                    anim.SetBool("IsWalking", true);
                }
                if (CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0, 5000) < 5)
                {
                    state = STATE.IDLE;
                    TurnOffTriggers();
                    agent.ResetPath();
                }
                break;
            case STATE.CHASE:
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 2;
                TurnOffTriggers();
                agent.speed = RunningSpeed;
                anim.SetBool("IsRunning", true);

                if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    state = STATE.ATTACK;
                }
                if(ForgetPlayer())
                {
                    state = STATE.WANDER;
                    agent.ResetPath();
                }

                break;
            case STATE.ATTACK:
                TurnOffTriggers();
                anim.SetBool("IsAttacking", true);
                this.transform.LookAt(target.transform.position);
                if(DistanceToPlayer() > agent.stoppingDistance + 2)
                    state = STATE.CHASE;
                break;
            case STATE.DEAD:
                break;
        }
       
    }
}
