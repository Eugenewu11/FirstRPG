using UnityEngine;
using UnityEngine.AI;  
// Importa el sistema de navegación de Unity (NavMeshAgent).
// Permite usar SetDestination, velocity, desiredVelocity, etc.

public class Enemy : MonoBehaviour
{
    
    // Referencia al Rigidbody2D de enemigo
    private Rigidbody2D rb2d;  

    // El objetivo que el enemigo seguirá (por ej., el jugador).
    public Transform targetTransform;  
    

    // Componente que controla el movimiento en el NavMesh.
    NavMeshAgent navMeshAgent;  
    
    Animator animator;  
    // Para controlar animaciones del enemigo (correr, idle, etc.).

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
    }

    void Update()
    {
        // Envía al agente hacia la posición del objetivo (player)
        navMeshAgent.SetDestination(targetTransform.position);

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
}
