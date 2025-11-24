using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerResourcesCollector : MonoBehaviour
{
    private int money = 0;
    private int meat = 0;
    private int wood = 0;

    

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MoneyBag"))
        {
            Destroy(collision.gameObject);
            money ++;
            //Debug.Log($"Tenemos: {money} de dinero");
            UIManager.Instance.updateMoneyCounter(money);
        }

        if (collision.gameObject.CompareTag("Meat"))
        {
            Destroy(collision.gameObject);
            meat ++;
            //Debug.Log($"Tenemos:  {meat} de carne");
            UIManager.Instance.updateMeatCounter(meat);
        }

        if (collision.gameObject.CompareTag("Wood"))
        {
            Destroy(collision.gameObject);
            wood ++;
            //Debug.Log($"Tenemos: {wood} de madera");
            UIManager.Instance.updateWoodCounter(wood);
        }
    }
}
