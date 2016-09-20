using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Menu : MonoBehaviour {

    public GameObject pauseMenu; //Доступ к меню

    FirstPersonController controller; //Комнтроллер персонажа

    [SerializeField]
    Shooting gun; //Скрипт стрельбы

    [SerializeField]
    AudioSource gunSound; //Звуки стрельбы

    private bool menuFlag = false; //Открыто ли меню

	// Use this for initialization
	void Start () {

        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

	}
	

    public void activateMenu() //Активация и деактивация меню
    {
        if (!menuFlag)
        {
            menuFlag = true;
            pauseMenu.SetActive(menuFlag);
            controller.enabled = false;
            gun.enabled = false;
            gunSound.Stop();
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            menuFlag = false;
            pauseMenu.SetActive(menuFlag);
            controller.enabled = true;
            gun.enabled = true;
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
