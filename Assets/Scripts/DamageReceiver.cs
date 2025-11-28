using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("Drop")]
    public GameObject[] itemToDrop;

    private Rigidbody2D rb2d;
    private Animator animator;

    public float forceImpulse = 5f;
    public int xpOnDeath = 25;
    public static event System.Action<int> OnTargetKilled;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void applyDamage(int amount,bool applyForceOrNot, bool applyHitAnimation, Vector2 hitDirection)
    {
        currentHealth -=amount;

        if (applyForceOrNot)
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.AddForce(hitDirection.normalized * forceImpulse, ForceMode2D.Impulse);
            Invoke("ReturnToKinematic", 0.2f);
        }

        if (applyHitAnimation)
        {
            animator.SetTrigger("Hit");
        }

        if(currentHealth <=0)
        {
            DropLoot();
            Die();
        }
    }

    void ReturnToKinematic()
    {
        rb2d.linearVelocity = Vector2.zero;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    void DropLoot()
    {
        for(int i=0; i < itemToDrop.Length; i++)
        {
            Instantiate(itemToDrop[i], transform.position, Quaternion.identity);
        }
        
    }

    void Die()
    {
        OnTargetKilled?.Invoke(xpOnDeath);
        Destroy(gameObject);
    }
}
