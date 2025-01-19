using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity;

class Experience_Bar
{
    public GameObject expBar;
    public Image expBackground;
    public Slider expSlider;
    public Image expIcon;
    public Text expText;

    public Experience_Bar(AssetBundle asset)
    {
        expBar = asset.LoadAsset<GameObject>("exp_bar");
        expBackground = expBar.transform.GetChild(0).GetComponent<Image>();
        expSlider = expBar.transform.GetChild(1).GetComponent<Slider>();
        expIcon = expBar.transform.GetChild(2).GetComponent<Image>();
        expText = expBar.transform.GetChild(3).GetComponent<Text>();
    }

    public void Resetup()
    {
        expBackground = expBar.transform.GetChild(0).GetComponent<Image>();
        expSlider = expBar.transform.GetChild(1).GetComponent<Slider>();
        expIcon = expBar.transform.GetChild(2).GetComponent<Image>();
        expText = expBar.transform.GetChild(3).GetComponent<Text>();
    }

    public void Enable()
    {
        expBar.SetActive(true);
    }

    public void Disable()
    {
        expBar.SetActive(false);
    }
}
