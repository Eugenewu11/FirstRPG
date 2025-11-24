using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class Sheep : MonoBehaviour
{
 // Referencia al Rigidbody2D de enemigo
    private Rigidbody2D rb2d;  

    // Componente que controla el movimiento en el NavMesh.
    NavMeshAgent navMeshAgent;  
    
    Animator animator;  
    // Para controlar animaciones del enemigo (correr, idle, etc.).

    //Movimiento de la oveja
    [Header("Movement Type")]
    public MovementType movementType;

    public enum MovementType
    {
        Path,
        RandomMovement
    }

    [Header("Path")]
    public Transform[] pathPoints;

    public float waitTimeAtPoint = 4f;
    private int indexPath = 0;



    void Start()
    {
        // Obtiene el Rigidbody2D del enemigo
        rb2d = GetComponent<Rigidbody2D>();

        // Obtiene el agente de navegación
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Obtiene el Animator del enemigo
        animator = GetComponent<Animator>();

        // En un juego 2D, Unity intenta rotar el agente (inútil y molesto). Se desactiva.
        navMeshAgent.updateRotation = false;

        // El NavMeshAgent usa el eje Y como “arriba” (3D).  
        // En 2D necesitamos que use X y Y, así que desactivamos el eje Z.
        navMeshAgent.updateUpAxis = false;

        if(movementType == MovementType.Path)
        {
            StartCoroutine(FollowPath());
        }
    }

    void Update()
    {

        // Ajusta animaciones y giro del sprite según la velocidad del agente
        AdjustAnimationsAndRotation();
    }

    public void AdjustAnimationsAndRotation()
    {
        // Revisa si el enemigo se está moviendo (velocity es vector)
        bool isMoving = navMeshAgent.velocity.sqrMagnitude > 0.01f;

        // Activa la animación de running si se mueve
        animator.SetBool("isRunning", isMoving);

        // Si la velocidad deseada del agente va a la derecha, giramos el sprite normal
        if(navMeshAgent.desiredVelocity.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);

        // Si va hacia la izquierda, invertimos el eje X
        if(navMeshAgent.desiredVelocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    IEnumerator FollowPath()
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
