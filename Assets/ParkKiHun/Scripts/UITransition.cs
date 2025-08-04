using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransition : MonoBehaviour
{
    public Animator menuAnimator;

    public void OnStartButtonClick()
    {
        menuAnimator.SetTrigger("GameStart");
    }

    public void PlayGameStartAnimation()
    {
        menuAnimator.SetTrigger("GameStart");
    }
}
