using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
// Importa el sistema de navegación de Unity (NavMeshAgent).
// Permite usar SetDestination, velocity, desiredVelocity, etc.

public class Enemy : NPC
{
    public float attackRange = 1.5f;
    public float stopDistance = 0.5f;
    public float attackCooldown = 2f;

    private float lastAttackTime = 0;
    private bool isAttacking = false;
    private bool canMove = true;

    public LayerMask targetLayer;
    private Vector2 playerDirection;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if(playerTransform == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position,playerTransform.position);

        if(distance <= attackRange && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            attackPlayer();
            lastAttackTime = Time.time;
        }
    }

    private void attackPlayer()
    {
        isAttacking = true;
        canMove = false;
        navMeshAgent.ResetPath();

        playerDirection = (playerTransform.position - transform.position).normalized;

        int attackDirection = GetDirectionIndex(playerDirection);

        if(playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }

        animator.SetInteger("AttackDirection",attackDirection);
        animator.SetTrigger("DoAttack");

        Invoke("ResetAttack",0.5f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
        canMove = true;
    }

    private void FixedUpdate() {
        navMeshAgent.isStopped = !canMove;
    }

    private int GetDirectionIndex(Vector2 dir)
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
        Vector2 attackPoint = (Vector2)transform.position + playerDirection.normalized * attackRange * 0.5f;
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint,attackRange,targetLayer);

        HashSet<GameObject> damagedTargets = new HashSet<GameObject>();
        
        foreach(Collider2D target in hitTargets)
        {
            
            GameObject obj = target.gameObject;

            if(damagedTargets.Contains(obj)){
                continue;
            }

            int layer = obj.layer;

            if (layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 hitDirection = target.transform.position - transform.position;
                obj.GetComponent<DamageReceiverPlayer>().applyDamage(1,true,false,hitDirection);
                damagedTargets.Add(obj);
            } 
        }
    }
}
