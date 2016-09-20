using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

    public int Damage = 1; //Урон
    public float fireRate = 1f; //Скорострельность
    public float weaponRange = 50f; //Дальность атаки
    public Transform firePoint; //Точка из которой рисуется лазер
    public Transform hitPrefab; //Префаб искр на конце лазера при попадании

    Transform hitParticle; //Искры на конце лазера
    private Camera fpsCam; //Компонент камеры
    private float shotDuration = 6.9f; //Продолжительность аудио
    private float timeToShot; //Время повторного запуска аудир
    private AudioSource gunAudio; //Звук стрельбы
    private LineRenderer laserLine; //Компонент прорисовки лазера
    private float nextFire; //Минимальное время слеующего выстрела

    // Инициализация требуемых компонентов и переменных
    void Awake () {

        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();
        timeToShot = Time.time;

    }

    void Update() {

        Vector3 lineOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(lineOrigin, fpsCam.transform.forward * weaponRange, Color.green);       
        

        if (Input.GetButton("Fire1")) //При зажатой кнопке огня работает лазер
        {
            laserLine.enabled = true;            

            RaycastHit hit;
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)); //Точка в центре экрана
            bool isHit = Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange); //Проверка попадания, а также вывод переменной hit

            if (GameObject.FindGameObjectWithTag("Hit") == null)//Если ведется стрельба новый эффект создавать не требуется
                hitParticle = Instantiate(hitPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal)) as Transform; //Создание искр на конце лазера
            else
                hitParticle.position = hit.point; //Перемещение искр вместе с мышью


            if (Time.time > timeToShot) //Воспроизведение звука лазера
            {
                timeToShot = Time.time + shotDuration;
                gunAudio.Play();
            }
                
            //Прорисовка лазера
            laserLine.SetPosition(0, firePoint.position);
            laserLine.SetPosition(1, hit.point);

            if (Time.time > nextFire) //Ограничение на частоту нанесения урона
            {                
                nextFire = Time.time + fireRate;   

                if (isHit) //Нанесение урона при попадании
                {

                    EnemyAI health = hit.collider.GetComponent<EnemyAI>();

                    if (health != null)
                    {
                        health.takingDamage(Damage);
                    }

                }
                else
                {
                    laserLine.SetPosition(1, rayOrigin = (fpsCam.transform.forward * weaponRange)); //Если в радиусе оружия цели нет, прорисовка лазера на длину атаки
                }
            }           
        }
        else
        {
            //Окончание стрельбы
            laserLine.enabled = false;
            gunAudio.Stop();
            timeToShot = Time.time;
            if(hitParticle != null)
            Destroy(hitParticle.gameObject, 0.01f);
        }
            
    }

}
