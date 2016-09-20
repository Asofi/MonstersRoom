using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    GameManager _gm;

    [Header("Stats:")]
    [SerializeField] int currentHealth = 1;

    void Awake()
    {
        _gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>() ;
    }

    public void takingDamage(int dmg) //Функция получения урона
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
            _gm.playerDie();
    }
}
