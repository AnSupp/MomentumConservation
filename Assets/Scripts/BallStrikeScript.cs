using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class BallStrikeScript : MonoBehaviour
{
    public GameObject strikeBall;   //������� ���
    public GameObject hitBall;      //������� ���

    [HideInInspector] public GameManager gameManager;

    public GameObject resultPanel;  //���� ���������� ���������� �����

    [HideInInspector] public bool oneTime = false; //���� �����������, ����� ��������� ������� ��������
    private bool collisionFlag = false; //����, ��������������� ������������ �����
    private bool victoryFlag = false;   //����, ��������������� � ��������� �������

    public GameObject top;  //������� ����
    public GameObject down; //������ ����
    private Vector3 topCoords;  //���������� ������� ����
    private Vector3 downCoords; //���������� ������ ����

    public AnimationScript animationScript; //������ �������� ���� ��������� ��������
                                            //����� ������������ ��� ���������� ������ � ������

    // Start is called before the first frame update
    void Start()
    {
        //��������� ���������� ���
        topCoords = top.transform.position;
        downCoords = down.transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //�������� ����, ��� ���� ����� ��������
        //animationFlag ��������� ��� �������:
        //1) �������� ������� ��� ������ ����� ��������� ��������
        //2) ����������� ������ ������� 
        if ((gameManager.startedFlag) && (animationScript.animationFlag)) 
        {
            if (!oneTime) //��������� ������ � ������ ����� �� �������� ����
            {
                strikeBall.GetComponent<Rigidbody>().AddForce(gameManager.impulseVector * gameManager.speed * 4.5f, ForceMode.Impulse); //������������ ������� � �������� ����
                AnswerCheck();  //��������� �����
                oneTime = true; //������� ��������, ���� �������
            }

            if ((collisionFlag) && (victoryFlag)) //��������� ������������ � ����� ����������
            {
                MoveDirection();    //���������� ���� � ����
                Invoke("VictoryMenu", 1.5f);    //���� � ���������� �� ������ �������� ����� 1,5 ���.
                collisionFlag = false;          
                victoryFlag = false;
                animationScript.animationFlag = false;  //��������� ������ �������
            }
            else if ((collisionFlag) && !(victoryFlag)) //��������� ������������ � ����� ������������
            {
                //������������ �������� �����
                strikeBall.GetComponent<Rigidbody>().velocity *= 1.7f;
                hitBall.GetComponent<Rigidbody>().velocity *= 2.4f;
                collisionFlag = false;
                animationScript.animationFlag = false;  //��������� ������ �������
            }
            
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("HitBall"))
        {
            collisionFlag = true;
            animationScript.animationFlag = true; //���������� ��� ������� ������������ ��� �������� ������
        }

    }
 
    /// ������� �������� ���������� ������
    /// � ������ ������������ - �������� ���� ������
    /// � ������ ������������ ������� - �������� ���� � ������� ���� � ����������
    private void AnswerCheck()
    {
        string pattern = "^2[.]8(2[8]?)?$"; //������ �������� ������

        Regex regex = new Regex(pattern);

        if ((regex.IsMatch(gameManager.inputVelocity.text)) && (gameManager.inputDegree.text == "15"))
        {
            victoryFlag = true;         
        }
        else
        {
            animationScript.animationFlag = false; //������ ������ � ������
            Invoke("FailMenu", 2f); //���� � ���������� � ������� �������� ����� 2 ���.
        }
    }
    /// ������� ���������� ���� � ���� ��� ���������� ������
    private void MoveDirection()
    {
        //����������� ������� ����������� ��������
        Vector3 strikeBallHitedVector = downCoords - strikeBall.transform.position;
        Vector3 hitBallHitedVector = topCoords - hitBall.transform.position;

        //�������� �������� �����, ��� ������������
        strikeBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        hitBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        //������������ ������� � �����
        strikeBall.GetComponent<Rigidbody>().AddForce(strikeBallHitedVector * gameManager.speed * 0.75f, ForceMode.Impulse);
        hitBall.GetComponent<Rigidbody>().AddForce(hitBallHitedVector * gameManager.speed * 0.6f, ForceMode.Impulse);    
    }
    private void VictoryMenu()
    {
        resultPanel.SetActive(true);
        resultPanel.GetComponentInChildren<Text>().text = "�����!";
        resultPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "����� ����������. �������!";
    }
    private void FailMenu()
    {
        collisionFlag = false; //�������� ���� �������� (�� ������, ���� ������������ ����)
        resultPanel.SetActive(true);
        resultPanel.GetComponentInChildren<Text>().text = "�������!";
        resultPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "����� ������������.\n�������� ��� ���.";
    }
}
