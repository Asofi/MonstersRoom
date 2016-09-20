using UnityEngine;
using System.Collections;

public class BtnScript : MonoBehaviour {

    public bool isOpen = false; //Открыта ли дверь

    public Transform btn; //Объект кнопки
    public Camera camera; //Камера игрока

    [SerializeField] AudioClip openSound; //Звук открытия
    [SerializeField] AudioClip closeSound; //Звук закрытия

    Animator _doorAnimator; //Аниматор двери
    Animator _btnAnimator; //Аниматор кнопки

    AudioSource _audioSource; //Компонент аудио

    float fiedlOfView; //Зона видимости
    float distToOpen = 5f; //Дистанция на которой возмоно взаимодействие
    float timeBetweenAnimation = 2.5f; //Время между открытием и закрытием
    float animTime; //Минимальное время слеующего нажатия


    // Инициализация требуемых компонентов и переменных
    void Awake () {

        _doorAnimator = GetComponent<Animator>();
        _btnAnimator = btn.gameObject.GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        animTime = Time.time;

    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        Vector3 rayOrigin = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)); //Точка в центре экрана
        Physics.Raycast(rayOrigin, camera.transform.forward, out hit, distToOpen); //Бросок луча на расстояние активации


        if (hit.collider != null)
                if (Input.GetKeyDown(KeyCode.E) && Time.time > animTime && hit.collider.gameObject.tag == "Btn") //Активация, если смотрим на кнопку и в данный момент дверь не движется
                {
                    animTime = Time.time + timeBetweenAnimation;
                    if (!isOpen)
                    {
                        _audioSource.clip = openSound;
                        _audioSource.Play();
                        isOpen = true;
                        _doorAnimator.SetTrigger("Activate");
                        _btnAnimator.SetTrigger("Pressed");
                        _doorAnimator.SetBool("isOpen", isOpen);
                        
                                              
                    }
                    else
                    {
                        _audioSource.clip = closeSound;
                        _audioSource.Play();
                        isOpen = false;
                        _doorAnimator.SetTrigger("Activate");
                        _btnAnimator.SetTrigger("Pressed");
                        _doorAnimator.SetBool("isOpen", isOpen);
                    }
                }

	
	}


}
