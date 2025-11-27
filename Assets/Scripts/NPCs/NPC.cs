using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.AI;
using System.Collections;

public class NPC : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected Transform playerTransform;
    private Coroutine currentMovementRoutine;

    [Header("Movement Type")]
    public MovementType movementType;
    public enum MovementType
    {
        Static,
        Path,
        RandomMovement
    }

    [Header("Random Movement")]
    public float movementRadius = 5f;
    public float waitTimeRandom = 3f;


    [Header("Skin")]
    public AnimatorController[] animatorControllers;

    //Seleccionar el color de la skin del NPC
    public NPCSkin selectedSkin;
    public enum NPCSkin
    {
        Blue, Purple, Red, Yellow
    }

    [Header("Path")]
    public Transform[] pathPoints;

    //Tiempo de espera en cada punto del path
    public float waitTimeAtPoint = 5f;

    //ID de los puntos donde vamos a pasar
    private int indexPath = 0;

    [Header("Player Chase")]
    public bool canChasePlayer = false;
    public float chaseRadius = 4f;
    public float stopDistanceFromPlayer = 1.5f;
    protected bool isChasingPlayer = false;

    [Header("Flee Behavior")]
    public bool canFlee = false;
    public float fleeRange = 4f;
    public float fleeDistance = 5f;
    private bool isFleeing = false;
    private MovementType previousMovementType;

    [Header("Return to Origin")]
    public bool returnToOrigin = false;
    public float maxDistanceFromOrigin =1f;
    protected Vector3 originPosition;
    

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navMeshAgent.updateRotation = false; //Desactivar rotacion automatica del goblin
        navMeshAgent.updateUpAxis = false; //Desactivar eje Z

        applySkin();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        originPosition = transform.position;
        ResumeOriginalBehavior();
    }

    protected virtual void Update()
    {
        AdjustAnimationsAndRotation();
        HandleChaseLogic();
        HandleFleeLogic();
        HandleReturnToOrigin();
    }

    void applySkin()
    {
        if(animatorControllers != null &&animatorControllers.Length > 0)
        {
            int skinIndex = (int)selectedSkin;
            if(animator != null &&skinIndex < animatorControllers.Length)
            {
                animator.runtimeAnimatorController = animatorControllers[skinIndex];
            }
        }
        
    }

    void ResumeOriginalBehavior()
    {
        ResumeMovementBehavior(movementType);
    }

    public void AdjustAnimationsAndRotation()
    {
        bool isMoving = navMeshAgent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool("isRunning",isMoving);

        if(navMeshAgent.desiredVelocity.x > 0.01f) transform.localScale = new Vector3(1,1,1);
        if(navMeshAgent.desiredVelocity.x < -0.01f) transform.localScale = new Vector3(-1,1,1);
    }

    private void ResumeMovementBehavior(MovementType type)
    {
        StopCurrentRoutine();
        switch (type)
        {
            case MovementType.Static:
                navMeshAgent.ResetPath();
                break;
            case MovementType.Path:
                StartCoroutine();
                break;
            case MovementType.RandomMovement:
                StartRandomRoutine();
                break;
            default:
                break;            
        }
    }

    protected void StartCoroutine()
    {
        StopCurrentRoutine();
        currentMovementRoutine = StartCoroutine(Followpath());
    }

    protected void StartRandomRoutine()
    {
        StopCurrentRoutine();
        currentMovementRoutine = StartCoroutine(RandomMovement());
    }

    void StopCurrentRoutine()
    {
        if(currentMovementRoutine != null)
        {
            StopCoroutine(currentMovementRoutine);
            currentMovementRoutine = null;
        }
    }

    protected void HandleChaseLogic(){
        if(!canChasePlayer || playerTransform == null || isFleeing)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position,playerTransform.position);

        if(distanceToPlayer <= chaseRadius){
            if(!isChasingPlayer){
                StopCurrentRoutine();
                isChasingPlayer = true;
            }

            if(distanceToPlayer > stopDistanceFromPlayer){
                navMeshAgent.stoppingDistance = stopDistanceFromPlayer;
                navMeshAgent.SetDestination(playerTransform.position);
            }else{
                navMeshAgent.ResetPath();
            }
        }else if(isChasingPlayer){
            navMeshAgent.ResetPath();
            isChasingPlayer = false;
            ResumeOriginalBehavior();
        }
    }

    protected void HandleFleeLogic(){
        if (!canFlee || playerTransform == null)
        {
            return;    
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if(distanceToPlayer < fleeRange &&!isFleeing){
            isFleeing = true;
            previousMovementType = movementType;
            StopCurrentRoutine();
            Vector3 fleeDirection = (transform.position - playerTransform.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

            if(NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas)){
                navMeshAgent.SetDestination(hit.position);
            }
        }
        else if (distanceToPlayer >= fleeRange && isFleeing)
        {
            isFleeing = false;
            ResumeMovementBehavior(previousMovementType);
        }
         
    }

    protected void HandleReturnToOrigin(){
        if(!returnToOrigin || isFleeing || isChasingPlayer){
            return;
        }

        float distance = Vector3.Distance(transform.position,originPosition);

        if(distance > maxDistanceFromOrigin){
            StopCurrentRoutine();
            navMeshAgent.SetDestination(originPosition);
        }
    }

    protected IEnumerator Followpath()
    {
        while (true)
        {
            //Comprobar que haya puntos en el path
            if(pathPoints.Length > 0)
            {
                //Establecer el primer destino pathPoints[0].position
                navMeshAgent.SetDestination(pathPoints[indexPath].position);

                //Verifica una ruta pendiente y si esta por llegar al punto(mayor a 0 significa que no ha llegado)
                yield return WaitUntilDestinationReached();
                yield return new WaitForSeconds(waitTimeAtPoint);//Esperar el tiempo que se tenga que esperar en el punto

                //Avanzar al siguiente punto 0 --> 1
                //% pathPoints.Length hace que vuelva al inicio cuando llegue al final
                indexPath = (indexPath + 1) % pathPoints.Length;
            }
            yield return null;
        }
    }

    private IEnumerator WaitUntilDestinationReached(){
        while(!navMeshAgent.pathPending && navMeshAgent.remainingDistance > 0.0f){
            yield return null;
        }
    }

    protected IEnumerator RandomMovement(){
        while(true){
            Vector3 randomPos = GetRandomNavMeshPosition();
            navMeshAgent.SetDestination(randomPos);
            yield return WaitUnitlDestinationReached();
            yield return new WaitForSeconds(waitTimeRandom);
        }
    }

    private Vector3 GetRandomNavMeshPosition(){
        Vector3 randomDirection = Random.insideUnitSphere * movementRadius + transform.position;
        
        if(NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, movementRadius, NavMesh.AllAreas)){
            return hit.position;
        }
        return transform.position; //Si no se encuentra una posicion valida, devuelve la posicion actual 
    }

    private IEnumerator WaitUnitlDestinationReached()
    {
        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.05f)
        {
            yield return null;
        }
    }
}
