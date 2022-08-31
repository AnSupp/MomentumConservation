using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public bool animationFlag; //флаг, оповещающий об окончании анимации кия

    public void AnimationEnd()
    {
        animationFlag = true; 
    }
}
