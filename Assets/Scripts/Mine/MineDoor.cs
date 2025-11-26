using UnityEngine;

public class MineDoor : MonoBehaviour
{
    public Transform transformToTransport;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.transform.position = transformToTransport.position;
        } 
    }
}
