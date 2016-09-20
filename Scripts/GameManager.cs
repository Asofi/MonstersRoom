using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Menu menu; //Доступ к меню

    public Text status; //Состояние игры
    public Text monstersLeft; //Оставшиеся враги


    private bool ended = false; //Закончена ли игра
    private float searchCountdown = 1f; //Частота проверки на победу

	void Update () {


        if (Input.GetKeyDown(KeyCode.Escape) && !ended) //Вызов меню
        {
            menu.activateMenu();
        }

        if (!EnemyIsAlive())
            win(); 

	}

    public void restart() //Перезапуск
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void quit() //Выход
    {
        Application.Quit();
    }

    bool EnemyIsAlive() //Проверка на наличие живых монстров
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            monstersLeft.text = "Monsters Left: " + GameObject.FindGameObjectsWithTag("Enemy").Length;
            searchCountdown = 1f;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                return false;
            }
        }
        return true;

    }

    public void playerDie() //Смерть игрока
    {
        status.text = "You lose!";
        ended = true;
        menu.activateMenu();
    }

    public void win() //Победа
    {
        status.text = "You win!";
        ended = true;
        menu.activateMenu();
    }
}
