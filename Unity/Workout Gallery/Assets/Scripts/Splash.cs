using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Splash : MonoBehaviour
{
    public Image SplashBg;
    public Image Logo;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(Logo.DOFade(1, 1)).SetAutoKill(true);
        mySequence.Insert(0.25f, SplashBg.DOFade(1, 1)).SetAutoKill(true);

        mySequence.AppendInterval(1.5f);

        mySequence.Append(Logo.DOFade(0, 1)).SetAutoKill(true);
        mySequence.Append(SplashBg.DOFade(0, 1)).SetAutoKill(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
