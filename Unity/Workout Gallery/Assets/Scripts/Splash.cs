using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Splash : MonoBehaviour
{
    public Image SplashBgColor;
    public Image SplashBg;
    public Image Logo;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SplashIn()
    {
        Sequence splashIn = DOTween.Sequence();
        splashIn.Append(Logo.DOFade(1, 1));
        splashIn.Insert(0.25f, SplashBg.DOFade(1, 1)).OnComplete(SplashOut);
    }

    private void SplashOut()
    {
        Sequence splashOut = DOTween.Sequence();
        splashOut.Append(Logo.DOFade(0, 1));
        splashOut.Insert(0.25f, SplashBg.DOFade(0, 1));
        splashOut.Insert(0.75f, SplashBgColor.DOFade(0, 1)).OnComplete(HideSplash);
        splashOut.PrependInterval(1f);
    }

    private void HideSplash()
    {
        this.gameObject.SetActive(false);
    }
}
