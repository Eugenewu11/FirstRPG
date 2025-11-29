using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEditor.Animations;


public class PlayerMovement : MonoBehaviour
{
    // Velocidad de movimiento del jugador
    public float speed = 3;

    // Controla si el juego está pausado
    private bool gameIsPaused = false;

    // Referencia al Rigidbody2D para mover al jugador con física
    Rigidbody2D rb2d;

    // Vector donde se guardará la dirección del movimiento
    Vector2 movementInput;

    // Referencia al Animator para animaciones del jugador
    private Animator animator;

    //Variable para saber si estamos atacando
    private bool isAttacking = false;

    //Variable para controlar el movimiento durante el ataque
    [HideInInspector]
    public bool canMove = true;

    Vector2 lastMovementDir = Vector2.right;
    Vector2 attackDir;
    public float attackRange = 1.1f;
    public int attackDamage = 1;
    public LayerMask targetLayer;

    private int xp = 0;
    [HideInInspector]
    public int currentLevel = 1;

    [Header("Skin")]
    public AnimatorController[] animatorControllers;
    public NPCSkin selectedSkin;
    public enum NPCSkin
    {
        Blue, Purple, Red, Yellow
    }

    void Start()
    {
        // Obtenemos el componente Rigidbody2D del jugador
        rb2d = GetComponent<Rigidbody2D>();

        // Obtenemos el Animator del jugador
        animator = GetComponent<Animator>();
        UIManager.Instance.updatePlayerStats(xp, currentLevel, speed, attackDamage);

        applySkin();
    }

    void Update()
    {
        if(movementInput != Vector2.zero)
        {
            lastMovementDir = movementInput;
        }

        // Leemos el input horizontal (A/D o flechas)
        movementInput.x = Input.GetAxisRaw("Horizontal");

        // Leemos el input vertical (W/S o flechas)
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Normalizamos para evitar moverse más rápido en diagonal
        movementInput = movementInput.normalized;

        // Enviamos valores al Animator para las animaciones
        animator.SetFloat("Horizontal", Math.Abs(movementInput.x));
        animator.SetFloat("Vertical", Math.Abs(movementInput.y));

        // Cambia la escala del sprite (flip) según dirección
        CheckFlip();

        // Abrir/cerrar el inventario con tecla I
        OpenCloseInventory();

        // Abrir/cerrar el menú de pausa con Escape
        OpenClosePauseMenu();

        OpenCloseStatsPlayer();
        Attack();
    }

    private void FixedUpdate()
    {

        if(canMove)
        {
            // Aplicamos movimiento al jugador usando física
            rb2d.linearVelocity = movementInput * speed;
        }
    }

    // Invierte la escala del sprite para mirar a izquierda o derecha
    void CheckFlip()
    {
        // Si se mueve a la derecha y está mirando a la izquierda, flip
        // O si se mueve a la izquierda y está mirando a la derecha, flip
        if ((movementInput.x > 0 && transform.localScale.x < 0) ||
            (movementInput.x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(
                transform.localScale.x * -1, 
                transform.localScale.y, 
                transform.localScale.z
            );
        }
    }

    // Abre o cierra el inventario presionando la tecla I
    void OpenCloseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.OpenOrCloseInventory();
        }
    }

    void OpenCloseStatsPlayer()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UIManager.Instance.OpenOrCloseStatsPlayer();
        }
    }

    // Maneja abrir/cerrar el menú de pausa
    void OpenClosePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si ya está pausado → reanudar
            if (gameIsPaused)
            {
                UIManager.Instance.ResumeGame();
                gameIsPaused = false;
            }
            else
            {
                // Si está en juego → pausar
                UIManager.Instance.PauseGame();
                gameIsPaused = true;
            }
        }
    }

    void Attack()
    {
        if(Input.GetMouseButtonDown(0) && !isAttacking)
        {
            int dir = GetDirectionIndex(lastMovementDir);
            attackDir = GetAttackInputDirection();
            int attackDirection = GetDirectionIndex(attackDir);
            animator.SetInteger("AttackDirection", attackDirection);

            int randomIndex = UnityEngine.Random.Range(0,2);
            animator.SetInteger("AttackIndex", randomIndex);
            animator.SetTrigger("DoAttack");
        }
    }

    public void startAttack()
    {
        isAttacking = true;
        rb2d.linearVelocity = Vector2.zero;
        canMove = false;
    }

    public void endAttack()
    {
        isAttacking = false;
        canMove = true;
    }

    Vector2 GetAttackInputDirection()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")).normalized;

        if(inputDir != Vector2.zero)
        {
            return inputDir;
        }
        else
        {
            if(transform.localScale.x > 0)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
    }

    int GetDirectionIndex(Vector2 dir)
    {
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? 0 : 1; // Derecha : Izquierda
        }
        else
        {
            return dir.y > 0 ? 2 : 3; // Arriba : Abajo
        }
    }


    public void DetectAndDamageTargets()
    {
        Vector2 attackPoint = (Vector2)transform.position + attackDir.normalized * attackRange * 0.5f;
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint,attackRange,targetLayer);

        foreach(Collider2D target in hitTargets)
        {
            Vector2 hitDirection = target.transform.position - transform.position;
            GameObject obj = target.gameObject;
            int layer = obj.layer;

            DamageReceiver damageReceiver = target.GetComponent<DamageReceiver>();

            if (layer == LayerMask.NameToLayer("Enemy"))
            {
                obj.GetComponent<DamageReceiver>().applyDamage(attackDamage,true,false,hitDirection);
            } 
            else if (layer == LayerMask.NameToLayer("Sheep"))
            {
                obj.GetComponent<DamageReceiver>().applyDamage(attackDamage,true,false,hitDirection);
            }
            else if (layer == LayerMask.NameToLayer("Tree"))
            {
                obj.GetComponent<DamageReceiver>().applyDamage(attackDamage,false,true,hitDirection);
            }
        }
    }

    void OnEnable()
    {
        DamageReceiver.OnTargetKilled += AddExp;
    }

    void OnDisable(){
        DamageReceiver.OnTargetKilled -= AddExp;
    }

    public void AddExp(int xpAmount){
        xp += xpAmount;

        if(xp > 100){
            xp -= 100;
            LevelUp();
        }

        UIManager.Instance.updatePlayerStats(xp, currentLevel, speed, attackDamage);
    }

    private void LevelUp(){
        currentLevel += 1;

        switch(currentLevel){
            case 2:
                speed += 1;
                attackDamage += 1;
                GetComponent<DamageReceiverPlayer>().GainHealth(1);
                break;
            case 3:
                speed += 1;
                attackDamage += 2;
                GetComponent<DamageReceiverPlayer>().GainHealth(1);
                break;
            case 4:
                speed += 1;
                attackDamage += 2;
                GetComponent<DamageReceiverPlayer>().GainHealth(1);
                break;
            case 5:
                speed += 11;
                attackDamage += 1;
                GetComponent<DamageReceiverPlayer>().GainHealth(1);
                break;
            default:
                speed += 1;
                attackDamage += 1;
                break;
        }
    }
    void applySkin()
    {
        string savedskin = PlayerPrefs.GetString("MainPlayerSkin","Blue");

        if(System.Enum.TryParse(savedskin, out NPCSkin skin)){
            selectedSkin = skin;
        }
        else{
            selectedSkin = NPCSkin.Blue;
        }

        if(animatorControllers != null &&animatorControllers.Length > 0)
        {
            int skinIndex = (int)selectedSkin;
            if(animator != null &&skinIndex < animatorControllers.Length)
            {
                animator.runtimeAnimatorController = animatorControllers[skinIndex];
            }
        }
        
    }
}
