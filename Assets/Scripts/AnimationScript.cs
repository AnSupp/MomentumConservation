using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public bool animationFlag; //����, ����������� �� ��������� �������� ���

    public void AnimationEnd()
    {
        animationFlag = true; 
    }
}
