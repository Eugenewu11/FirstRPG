using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public float speed = 5;
    private Rigidbody2D rb2d;
    public Transform targetTransform;
    NavMeshAgent navMeshAgent;

    Animator animator;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navMeshAgent.updateRotation = false; //Desactivar rotacion automatica del goblin
        navMeshAgent.updateUpAxis = false; //Desactivar eje Z
        
    }


    void Update()
    {
        navMeshAgent.SetDestination(targetTransform.position);
        AdjustAnimationsAndRotation();
    }

    public void AdjustAnimationsAndRotation()
    {
        bool isMoving = navMeshAgent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool("isRunning",isMoving);

        if(navMeshAgent.desiredVelocity.x > 0.01f) transform.localScale = new Vector3(1,1,1);
        if(navMeshAgent.desiredVelocity.x < -0.01f) transform.localScale = new Vector3(-1,1,1);
    }
}
