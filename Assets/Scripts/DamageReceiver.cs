using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Drop")]
    public GameObject itemToDrop;

    public Rigidbody2D rb2d;

    public float forceImpulse = 5f;

    void Start()
    {
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
        Instantiate(itemToDrop, transform.position + Vector3.up, Quaternion.identity);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
