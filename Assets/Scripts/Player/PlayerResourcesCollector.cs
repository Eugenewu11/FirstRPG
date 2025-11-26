using NUnit.Framework;                 // Librería para pruebas unitarias (no es necesaria aquí)
using Unity.VisualScripting;           // Librería usada por Visual Scripting (tampoco es necesaria)
using UnityEngine;                     // Librería principal de Unity

public class PlayerResourcesCollector : MonoBehaviour
{
    // Variables privadas que almacenan los recursos del jugador
    private int money = 0;             // Dinero recogido
    private int meat = 0;              // Carne recogida
    private int wood = 0;              // Madera recogida

    void Start()
    {
        // Método vacío, se ejecuta una vez al iniciar el objeto
    }

    void Update()
    {
        // Método vacío, se ejecuta una vez por frame
    }

    // Se ejecuta automáticamente cuando el jugador entra en contacto con un collider con "Is Trigger" activado
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto con el que colisiona tiene el tag "MoneyBag"
        if (collision.gameObject.CompareTag("MoneyBag"))
        {
            Destroy(collision.gameObject);                 // Elimina el objeto del mapa
            money++;                                       // Suma 1 al contador de dinero
            UIManager.Instance.updateMoneyCounter(money);  // Actualiza la UI
        }

        // Si colisiona con un objeto etiquetado como "Meat"
        if (collision.gameObject.CompareTag("Meat"))
        {
            Destroy(collision.gameObject);                 // Elimina el objeto
            meat++;                                        // Suma carne
            UIManager.Instance.updateMeatCounter(meat);    // Actualiza contador de carne
        }

        // Si colisiona con un objeto etiquetado como "Wood"
        if (collision.gameObject.CompareTag("Wood"))
        {
            Destroy(collision.gameObject);                 // Elimina el objeto
            wood++;                                        // Suma madera
            UIManager.Instance.updateWoodCounter(wood);    // Actualiza contador de madera
        }
    }
}
