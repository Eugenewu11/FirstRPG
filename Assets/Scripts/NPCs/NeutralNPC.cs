using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class NeutralNPC : MonoBehaviour
{
    private Rigidbody2D rb2d;
    //public Transform targetTransform;
    NavMeshAgent navMeshAgent;

    [Header("Skin")]
    public AnimatorController[] animatorControllers;

    //Seleccionar el color de la skin del NPC
    public NPCSkin selectedSkin;
    public enum NPCSkin
    {
        Blue, Purple, Red, Yellow
    }

    //Tipo de movimiento del NPC
    [Header("Movement Type")]
    public MovementType movementType;

    public enum MovementType
    {
        Path,
        RandomMovement
    }

    //Path para decirle hacia donde moverse y tiempo que queremos que espere en cada punto
    //Puntos por los que va a pasar el NPC
    [Header("Path")]
    public Transform[] pathPoints;

    //Tiempo de espera en cada punto del path
    public float waitTimeAtPoint = 5f;

    //ID de los puntos donde vamos a pasar
    private int indexPath = 0;


    Animator animator;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navMeshAgent.updateRotation = false; //Desactivar rotacion automatica del goblin
        navMeshAgent.updateUpAxis = false; //Desactivar eje Z

        applySkin();
        
        if(movementType == MovementType.Path)
        {
            StartCoroutine(Followpath());
        }
    }


    void Update()
    {
        //navMeshAgent.SetDestination(targetTransform.position);
        AdjustAnimationsAndRotation();
    }

    public void AdjustAnimationsAndRotation()
    {
        bool isMoving = navMeshAgent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool("isRunning",isMoving);

        if(navMeshAgent.desiredVelocity.x > 0.01f) transform.localScale = new Vector3(1,1,1);
        if(navMeshAgent.desiredVelocity.x < -0.01f) transform.localScale = new Vector3(-1,1,1);
    }

    public void applySkin()
    {
        int skinIndex = (int)selectedSkin;
        animator.runtimeAnimatorController = animatorControllers[skinIndex];
    }

    IEnumerator Followpath()
    {
        while (true)
        {
            //Comprobar que haya puntos en el path
            if(pathPoints.Length > 0)
            {
                //Establecer el primer destino pathPoints[0].position
                navMeshAgent.SetDestination(pathPoints[indexPath].position);

                //Verifica una ruta pendiente y si esta por llegar al punto(mayor a 0 significa que no ha llegado)
                while(!navMeshAgent.pathPending && navMeshAgent.remainingDistance > 0.1f)
                {
                    yield return null; 
                }
                yield return new WaitForSeconds(waitTimeAtPoint);//Esperar el tiempo que se tenga que esperar en el punto

                //Avanzar al siguiente punto 0 --> 1
                //% pathPoints.Length hace que vuelva al inicio cuando llegue al final
                indexPath = (indexPath + 1) % pathPoints.Length;
            }
            yield return null;
        }
    }




}
