using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject strikeBall;   //������� ���
    public GameObject hitBall;      //������� ���
    public GameObject clueStick;    //���

    public InputField inputDegree;      //���� ����� ����
    public InputField inputVelocity;    //���� ����� ��������

    [HideInInspector] public Vector3 impulseVector; //������ ��������

    private Vector3 inputImpulseVector;     //������ ����������� ��������
    private Vector3 strikeBallCoords;       //���������� �������� ���� (0,0,0) - ��� �������
    private Vector3 defStrikeBallCoords;    //��������� ���������� �������� ����
    private Vector3 defHitBallCoords;       //��������� ���������� �������� ����

    [HideInInspector] public BallStrikeScript ballStrikeScript;
    public GameObject errorPanel;           //���� ������ �����

    [HideInInspector] public bool startedFlag = false;  //����, ����������� ����
    [HideInInspector] public float speed;               //�������� �������� ����

    public Animation clueStickAnimation;

    private float prevYrotate;      //���������� �������� �������� ���, ������������ �
    private int rad = 1;            //������ (��� ������� ��������)
    private float x;
    private float y = 5.168f; //�������� � ����� (�����, ����� ���� �� ������ � ����������)
    private float z;

    // Start is called before the first frame update
    void Start()
    {
        prevYrotate = 360 - clueStick.transform.localEulerAngles.y; //��������� ��������� �������� �������� ���
                                                                    //�� 360 �������� ��� ����, 
                                                                    //����� ������ �������� � ����������� 360 ����������
        //��������� ��������� ���������� �����
        defStrikeBallCoords = strikeBall.transform.position;
        defHitBallCoords = hitBall.transform.position;
    }

    /// ������� �������� ������� ��������� �������
    /// ��� ���������� ����� - ��������� �������� ��� � ���� ���������� �� ����� �����
    /// ��� ������������ - ������� �� ����� ���� � �������
    public void AnswersInput()
    {
        string pattern = "^[0-9]{1,3}([.]?[0-9]{1,3})?$"; //������ ����� �������
        
        Regex regex = new Regex(pattern);

        if ((regex.IsMatch(inputDegree.text)) && (regex.IsMatch(inputVelocity.text)))
        {
                  
            clueStick.transform.Rotate(0.0f, prevYrotate, 0.0f, Space.Self); //���������� ��� ���������� ��������� ��������
                                                                             //(����� ��� ����������� �������� ���)
            clueStick.transform.Rotate(0.0f, - Convert.ToSingle(inputDegree.text), 0.0f, Space.Self); //������������ ���
                                                                                                      //����� � �, �.�. � ��������� �����������
                                                                                                      //� Unity 0��. ��������� �����
            prevYrotate = 360 - clueStick.transform.localEulerAngles.y;        //��������� ���������� �������� ��������

            //������ x � z ��������� ������� ��������
            x = Convert.ToSingle(rad * Math.Sin((90 - Convert.ToDouble(inputDegree.text)) * Math.PI / 180));
            z = Convert.ToSingle(rad * Math.Cos((90 - Convert.ToDouble(inputDegree.text)) * Math.PI / 180));

            clueStickAnimation.Play("ClueStickAnimation"); //������ �������� ���

            inputImpulseVector = new Vector3(x, y, z);
            impulseVector = inputImpulseVector - strikeBallCoords;  //����������� ������ ��������

            speed = Convert.ToSingle(inputVelocity.text);
            startedFlag = true;    //��������� ����� �����
        }
        else if ((regex.IsMatch(inputDegree.text)) && !(regex.IsMatch(inputVelocity.text)))
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "�������� �������� ������� � ������������ �������.\n" +
                "����� �������� � ���� ���������� ����� � ��������� �� ���� ������ ����� �����.";
        }
        else if (!(regex.IsMatch(inputDegree.text)) && (regex.IsMatch(inputVelocity.text)))
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "�������� ���� ������� � ������������ �������.\n" +
                "����� �������� � ���� ���������� ����� � ��������� �� ���� ������ ����� �����.";
        }
        else
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "�������� ������� � ������������ �������.\n" +
                "����� �������� �������� � ���� � ���� ���������� ����� � ��������� �� ���� ������ ����� �����.";
        }
    }
    /// ������� ������� "������������� ����"
    /// ��������������� ��������� �������������� �����
    public void Restart()
    {
        ballStrikeScript.gameManager.startedFlag = false; //��������� ���������� �����

        strikeBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);   //�������� �������� ����
        strikeBall.GetComponent<Rigidbody>().freezeRotation = true;  //��������� �������� (����� ��� ��������� �����������)
        strikeBall.GetComponent<Rigidbody>().freezeRotation = false; //�������� ������� (��� ���������� ������)
        strikeBall.transform.position = defStrikeBallCoords;         //���������� ��� � ��������� ���������
        
        hitBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        hitBall.GetComponent<Rigidbody>().freezeRotation = true;
        hitBall.GetComponent<Rigidbody>().freezeRotation = false;
        hitBall.transform.position = defHitBallCoords;
  
        ballStrikeScript.oneTime = false;   //��������� ����, ������� �������� �� ��, ��� ������� ��������� ��������
    }
}
