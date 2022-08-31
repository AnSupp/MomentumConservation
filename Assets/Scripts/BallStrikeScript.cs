using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class BallStrikeScript : MonoBehaviour
{
    public GameObject strikeBall;   //зеленый шар
    public GameObject hitBall;      //красный шар

    [HideInInspector] public GameManager gameManager;

    public GameObject resultPanel;  //окно результата проведения опыта

    [HideInInspector] public bool oneTime = false; //флаг необходимый, чтобы приложить импульс единожды
    private bool collisionFlag = false; //флаг, сигнализирующий столкновение шаров
    private bool victoryFlag = false;   //флаг, сигнализурующий о правиьных ответах

    public GameObject top;  //верхняя луза
    public GameObject down; //нижняя луза
    private Vector3 topCoords;  //координаты верхней лузы
    private Vector3 downCoords; //координаты нижней лузы

    public AnimationScript animationScript; //скрипт содержит флаг окончания анимации
                                            //также используется для отключения работы с шарами

    // Start is called before the first frame update
    void Start()
    {
        //сохраняем координаты луз
        topCoords = top.transform.position;
        downCoords = down.transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //проверка того, что опыт можно провести
        //animationFlag выполняет две функции:
        //1) запустит зеленый шар только после окончания анимации
        //2) заблокирует работу скрипта 
        if ((gameManager.startedFlag) && (animationScript.animationFlag)) 
        {
            if (!oneTime) //сработает только в момент удара по зеленому шару
            {
                strikeBall.GetComponent<Rigidbody>().AddForce(gameManager.impulseVector * gameManager.speed * 4.5f, ForceMode.Impulse); //прикладываем импульс к зеленому шару
                AnswerCheck();  //проверяем ответ
                oneTime = true; //импульс сработал, флаг взводим
            }

            if ((collisionFlag) && (victoryFlag)) //произошло столкновение и ответ правильный
            {
                MoveDirection();    //направляем шары в лузы
                Invoke("VictoryMenu", 1.5f);    //окно с сообщением об успехе появится через 1,5 сек.
                collisionFlag = false;          
                victoryFlag = false;
                animationScript.animationFlag = false;  //запрещаем работу скрипта
            }
            else if ((collisionFlag) && !(victoryFlag)) //произошло столкновение и ответ неправильный
            {
                //корректируем скорости шаров
                strikeBall.GetComponent<Rigidbody>().velocity *= 1.7f;
                hitBall.GetComponent<Rigidbody>().velocity *= 2.4f;
                collisionFlag = false;
                animationScript.animationFlag = false;  //запрещаем работу скрипта
            }
            
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("HitBall"))
        {
            collisionFlag = true;
            animationScript.animationFlag = true; //необходимо для случаев столкновения при неверном ответе
        }

    }
 
    /// Функция проверки введенного ответа
    /// В случае правильности - поднимет флаг успеха
    /// В случае неправильных ответов - отключит опыт и выведет окно с сообщением
    private void AnswerCheck()
    {
        string pattern = "^2[.]8(2[8]?)?$"; //шаблон проверки ответа

        Regex regex = new Regex(pattern);

        if ((regex.IsMatch(gameManager.inputVelocity.text)) && (gameManager.inputDegree.text == "15"))
        {
            victoryFlag = true;         
        }
        else
        {
            animationScript.animationFlag = false; //запрет работы с шарами
            Invoke("FailMenu", 2f); //окно с сообщением о неудачи появится через 2 сек.
        }
    }
    /// Функция направляет шары в лузы при правильном ответе
    private void MoveDirection()
    {
        //расчитываем вектора направления движения
        Vector3 strikeBallHitedVector = downCoords - strikeBall.transform.position;
        Vector3 hitBallHitedVector = topCoords - hitBall.transform.position;

        //зануляем скорость шаров, для корректности
        strikeBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        hitBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        //прикладываем импульс к шарам
        strikeBall.GetComponent<Rigidbody>().AddForce(strikeBallHitedVector * gameManager.speed * 0.75f, ForceMode.Impulse);
        hitBall.GetComponent<Rigidbody>().AddForce(hitBallHitedVector * gameManager.speed * 0.6f, ForceMode.Impulse);    
    }
    private void VictoryMenu()
    {
        resultPanel.SetActive(true);
        resultPanel.GetComponentInChildren<Text>().text = "УСПЕХ!";
        resultPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "Ответ правильный. Молодец!";
    }
    private void FailMenu()
    {
        collisionFlag = false; //обнуляем флаг коллизии (на случай, если столкновение было)
        resultPanel.SetActive(true);
        resultPanel.GetComponentInChildren<Text>().text = "НЕУДАЧА!";
        resultPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "Ответ неправильный.\nПопробуй еще раз.";
    }
}
