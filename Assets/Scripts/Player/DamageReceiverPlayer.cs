using UnityEngine;

public class DamageReceiverPlayer : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    private int currentHealth;


    private Rigidbody2D rb2d;
    private Animator animator;

    public float forceImpulse = 4f;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Actualizamos la barra de vida en el UI
        UIManager.Instance.updateHealth(currentHealth, maxHealth);
    }

    public void applyDamage(int amount,bool applyForceOrNot, bool applyHitAnimation, Vector2 hitDirection)
    {
        currentHealth -=amount;
        // Actualizamos la barra de vida en el UI
        UIManager.Instance.updateHealth(currentHealth,maxHealth);

        if (applyForceOrNot)
        {
            GetComponent<PlayerMovement>().canMove = false;
            rb2d.AddForce(hitDirection.normalized * forceImpulse, ForceMode2D.Impulse);
            Invoke("ResetMovement",0.1f);
        }

        if (applyHitAnimation)
        {
            animator.SetTrigger("Hit");
        }

        if(currentHealth <=0)
        {
            Die();
        }
    }


    private void ResetMovement()
    {
        GetComponent<PlayerMovement>().canMove = true;
    }

    public void GainHealth(int healthAmount){
        maxHealth += healthAmount;
        currentHealth = maxHealth;
        UIManager.Instance.updateHealth(currentHealth, maxHealth);
    }

    void Die()
    {
        //Reset Level
    }
}
