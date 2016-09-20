using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

    public enum enemyState { IDLE, LOOKING, CHASING, ATTACKING}; //Перечень состояний противника

    public float targetDistance = 50f; //Расстояние до игрока
    public float lookDistance = 50f; //Расстояние на котором враг начинает следить за игроком
    public float chaseDistance = 30f; //Расстояние на котором враг начинает преследование
    public float attackDistance = 5f; //Расстояние атаки
    public float movementSpeed = 10f; //Коэффицент скорости передвижения
    public float damping = 5f; //Коэффицент сглаживания поворота
    public WaitForSeconds attackRepeatTime = new WaitForSeconds(0.5f); //Время перед атакой
    public float chasingTime = 5f; //Время следования вперед после пропажи ирока из зоны видимости
    public Transform target; //Цель

    public AudioClip hited;//Звук удара по монстру
    public AudioClip attacking; //Звук удара по игроку
    public AudioClip dying;//Звук смерти

    float timeToForget; //Момент времени от которого пойдет отсчет 5 секунд
    CharacterController controller; //Компонент контроллера
    Animator anim; //Аниматор
    float gravity = 20f; //Кожффицент гравитации
    Vector3 moveDirection = Vector3.zero; //Вектор движения
    Player targetStats; //Доступ к данным о игроке
    AudioSource enemyAudio; //Компонент аудио

    [Header("Stats:")]
    [SerializeField]
    int currentHealth = 3; //Количество очков здоровья
    [SerializeField]
    int damage = 1; //Урон
    [SerializeField]
    enemyState state = enemyState.IDLE; //Инициализация переменной состояния



    // Инициализация требуемых компонентов и переменных
    void Awake () {
        target = GameObject.FindGameObjectWithTag("Player").transform; 
        targetStats = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        controller = GetComponent<CharacterController>();
        timeToForget = Time.time;
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();

    }
	



	void FixedUpdate () {

        targetDistance = Vector3.Distance(target.position, transform.position);

        RaycastHit hitPlayer;
        Vector3 rayOrigin = transform.position;

        Debug.DrawRay(rayOrigin, (target.position - transform.position), Color.blue);

        Physics.Raycast(rayOrigin, (target.position - transform.position), out hitPlayer, targetDistance); //Бросание луча в сторону игрока

        //Управление анимацией в зависимости от состояния
        switch (state)
        {
            case enemyState.ATTACKING:
                anim.SetBool("Attacking", true);
                anim.SetBool("Chasing", false);
                break;
            case enemyState.CHASING:
                anim.SetBool("Attacking", false);
                anim.SetBool("Chasing", true);
                break;
            case enemyState.LOOKING:
            case enemyState.IDLE:
                anim.SetBool("Attacking", false);
                anim.SetBool("Chasing", false);
                break;
        }

        if (targetDistance < lookDistance) //Начать слежение, если игрок ближе необходимой дистанции и путь к нему не прегражден
        {

            if (hitPlayer.collider.gameObject.tag == "Player")
            {
                lookAtPlayer();
            }
            else
                state = enemyState.IDLE;              
                
        }


        if (targetDistance < attackDistance) //Атаковать, если игрок ближе расстояния атаки и путь к нему не прегражден
        {

            if (hitPlayer.collider.gameObject.tag == "Player")
            {
                StartCoroutine(attack());
            }
            else enemyAudio.clip = hited;

        }
        else
        {
            if ((targetDistance < chaseDistance)) //Преследовать если враг не атакует и игрок ближе дистанции преследования и путь к нему не прегражден
            {
                if (hitPlayer.collider.gameObject.tag == "Player")
                {
                    chase();
                    timeToForget = Time.time + chasingTime;
                }
                else
                {
                    if (Time.time < timeToForget)
                        chase();
                }
            }
        }            

    }

    void lookAtPlayer() //Функция слежения за игроком
    {
        state = enemyState.LOOKING;
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        rotation.x = 0f;
        rotation.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    void chase() //Функция пресследования
    {
        state = enemyState.CHASING;
        moveDirection = transform.forward;
        moveDirection *= movementSpeed;

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

    }

    IEnumerator attack() //Функция атаки
    {
        state = enemyState.ATTACKING;
        enemyAudio.clip = attacking;
        enemyAudio.Play();
        yield return attackRepeatTime;
        targetStats.takingDamage(damage);

        
    }

    public void takingDamage(int dmg) //Функция получения урона
    {       
        lookDistance += 20f;// При нанесении урона враг увеличивает радиус наблюдения  и преследует игрока
        chaseDistance = lookDistance; 
        if (!anim.GetBool("Dead"))
        {
            enemyAudio.Play();
            int takingDmgAnim = Random.Range(0, 2);
            switch (takingDmgAnim) //Случайная анимация получения урона
            {
                case 0:
                    anim.SetTrigger("takingDmg0");
                    break;
                case 1:
                    anim.SetTrigger("takingDmg1");
                    break;
            }

            currentHealth -= dmg;
            if (currentHealth == 0)
                Die();
        }
        
    }
   
    void Die() //Функция смерти
    {
        enemyAudio.clip = dying;
        enemyAudio.Play();
        anim.SetBool("Dead", true);
        anim.SetTrigger("die");
        //enemyAudio.enabled = false;
        gameObject.GetComponent<EnemyAI>().enabled = false;
        Destroy(gameObject, 3.9f);
    }

}
