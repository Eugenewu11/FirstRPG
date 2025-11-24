using TMPro;
using UnityEngine;  

public class UIManager : MonoBehaviour
{
    //Singleton: Es un patron de diseño que permite que una clase tenga una única instancia y proporciona un punto de acceso global a ella.

    //Utilizado comunmente para UIManager, GameManager, AudioManager, etc.
    public static UIManager Instance {get; private set;}

    public GameObject inventory;
    public GameObject pauseMenu;

    public TMP_Text moneyCounterTxt;
    public TMP_Text meatCounterTxt;
    public TMP_Text woodCounterTxt;
    public TMP_Text healthCounterTxt;

    private void Awake()
    {
        //Con awake antes de que se inicie el juego, se asegura que solo exista una instancia del UIManager

        //En caso de que no exista una instancia se le asigna esta nueva, si no se destruye
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenOrCloseInventory()
    {
        inventory.SetActive(!inventory.activeSelf);
    }

    public void updateMoneyCounter(int amount)
    {
        moneyCounterTxt.text = amount.ToString();
    }

    public void updateMeatCounter(int amount)
    {
        meatCounterTxt.text = amount.ToString();
    }

    public void updateWoodCounter(int amount)
    {
        woodCounterTxt.text = amount.ToString();
    }

    public void updateHealth(int value)
    {
        healthCounterTxt.text = value.ToString();
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; //Detiene el tiempo en el juego
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
