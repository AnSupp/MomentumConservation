using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject strikeBall;   //зеленый шар
    public GameObject hitBall;      //красный щар
    public GameObject clueStick;    //кий

    public InputField inputDegree;      //поле ввода угла
    public InputField inputVelocity;    //поле ввода скорости

    [HideInInspector] public Vector3 impulseVector; //вектор импульса

    private Vector3 inputImpulseVector;     //вектор направления импульса
    private Vector3 strikeBallCoords;       //координаты зеленого мяча (0,0,0) - для расчета
    private Vector3 defStrikeBallCoords;    //начальные координаты зеленого шара
    private Vector3 defHitBallCoords;       //начальные координаты красного шара

    [HideInInspector] public BallStrikeScript ballStrikeScript;
    public GameObject errorPanel;           //окно ошибки ввода

    [HideInInspector] public bool startedFlag = false;  //флаг, запускающий опыт
    [HideInInspector] public float speed;               //скорость зеленого шара

    public Animation clueStickAnimation;

    private float prevYrotate;      //предыдущее значение поворота кия, относительно у
    private int rad = 1;            //рариус (для расчета векторов)
    private float x;
    private float y = 5.168f; //значение у шаров (нужно, чтобы шары не меняли у координату)
    private float z;

    // Start is called before the first frame update
    void Start()
    {
        prevYrotate = 360 - clueStick.transform.localEulerAngles.y; //сохраняем начальное значение поворота кия
                                                                    //из 360 вычитаем для того, 
                                                                    //чтобы почить значение в стандартной 360 окружности
        //сохраняем начальные координаты шаров
        defStrikeBallCoords = strikeBall.transform.position;
        defHitBallCoords = hitBall.transform.position;
    }

    /// Функция проверки формата введенных ответов
    /// При корректном вводе - запускает анимацию кия и дает разрешение на старт опыта
    /// При неправильном - выводит на экран окно с ошибкой
    public void AnswersInput()
    {
        string pattern = "^[0-9]{1,3}([.]?[0-9]{1,3})?$"; //шаблон ввода ответов
        
        Regex regex = new Regex(pattern);

        if ((regex.IsMatch(inputDegree.text)) && (regex.IsMatch(inputVelocity.text)))
        {
                  
            clueStick.transform.Rotate(0.0f, prevYrotate, 0.0f, Space.Self); //возвращаем кию предыдущее положение поворота
                                                                             //(нужно для корректного прокрута кия)
            clueStick.transform.Rotate(0.0f, - Convert.ToSingle(inputDegree.text), 0.0f, Space.Self); //поворачиваем кий
                                                                                                      //минус в у, т.к. в градусной окнружности
                                                                                                      //в Unity 0гр. находится слева
            prevYrotate = 360 - clueStick.transform.localEulerAngles.y;        //сохраняем предыдущее значение поворота

            //расчет x и z координат вектора импульса
            x = Convert.ToSingle(rad * Math.Sin((90 - Convert.ToDouble(inputDegree.text)) * Math.PI / 180));
            z = Convert.ToSingle(rad * Math.Cos((90 - Convert.ToDouble(inputDegree.text)) * Math.PI / 180));

            clueStickAnimation.Play("ClueStickAnimation"); //запуск анимации кия

            inputImpulseVector = new Vector3(x, y, z);
            impulseVector = inputImpulseVector - strikeBallCoords;  //высчитываем вектор импульса

            speed = Convert.ToSingle(inputVelocity.text);
            startedFlag = true;    //разрешаем старт опыта
        }
        else if ((regex.IsMatch(inputDegree.text)) && !(regex.IsMatch(inputVelocity.text)))
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Значение скорости введено в некорректром формате.\n" +
                "Введи значение в виде десятичной дроби с точностью до трех знаков после точки.";
        }
        else if (!(regex.IsMatch(inputDegree.text)) && (regex.IsMatch(inputVelocity.text)))
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Значение угла введено в некорректром формате.\n" +
                "Введи значение в виде десятичной дроби с точностью до трех знаков после точки.";
        }
        else
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Значения введены в некорректром формате.\n" +
                "Введи значения скорости и угла в виде десятичной дроби с точностью до трех знаков после точки.";
        }
    }
    /// Функция которая "перезагружает опыт"
    /// Восстанавливает начальные местоположения шаров
    public void Restart()
    {
        ballStrikeScript.gameManager.startedFlag = false; //отключаем проведение опыта

        strikeBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);   //зануляем скорость шара
        strikeBall.GetComponent<Rigidbody>().freezeRotation = true;  //отключаем вращение (чтобы шар полностью остановился)
        strikeBall.GetComponent<Rigidbody>().freezeRotation = false; //включаем обратно (для дальнейших опытов)
        strikeBall.transform.position = defStrikeBallCoords;         //перемещаем шар в начальное положение
        
        hitBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        hitBall.GetComponent<Rigidbody>().freezeRotation = true;
        hitBall.GetComponent<Rigidbody>().freezeRotation = false;
        hitBall.transform.position = defHitBallCoords;
  
        ballStrikeScript.oneTime = false;   //выключаем флаг, который отвечает за то, что импульс сработает единожды
    }
}
