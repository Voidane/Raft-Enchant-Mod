using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity;

public class Experience_Bar
{
    public GameObject expBar;
    public Image expBackground;
    public Slider expSlider;
    public Image expIcon;
    public Text expText;

    public int level;
    public double percent;

    public int minExp;
    public int maxExp;
    public float totalExp;

    public Experience_Bar(AssetBundle asset)
    {
        expBar = asset.LoadAsset<GameObject>("exp_bar");
        expBackground = expBar.transform.GetChild(0).GetComponent<Image>();
        expSlider = expBar.transform.GetChild(1).GetComponent<Slider>();
        expIcon = expBar.transform.GetChild(2).GetComponent<Image>();
        expText = expBar.transform.GetChild(3).GetComponent<Text>();

        level = 0;
        percent = 0;
        minExp = 0;
        maxExp = 100;
        totalExp = 0;

        UpdateExpBarValue();
    }

    public void Resetup()
    {
        expBackground = expBar.transform.GetChild(0).GetComponent<Image>();
        expSlider = expBar.transform.GetChild(1).GetComponent<Slider>();
        expIcon = expBar.transform.GetChild(2).GetComponent<Image>();
        expText = expBar.transform.GetChild(3).GetComponent<Text>();

        UpdateExpBarValue();
    }

    public void Enable()
    {
        expBar.SetActive(true);
    }

    public void Disable()
    {
        expBar.SetActive(false);
    }

    public void AddExperience(float amount)
    {
        Network_Player player = RAPI.GetLocalPlayer();
        player.SendChatMessage($"{EnchantingSystem.MOD_NAME} Gained {amount} exp.");

        float total = totalExp + amount;
        
        if (total >= maxExp)
        {
            float rest = total - maxExp;
            maxExp = (int) (maxExp * 1.25);
            totalExp = rest;
            level++;
            OnLevelUp();
        }
        else
        {
            totalExp = totalExp + amount;
        }

        UpdateExpBarValue();
    }

    public void UpdatePercentage(float total, int max) 
    {
        percent = Math.Round((double)(total * 100.0) / max, 2);
        Debug.Log($"percent: {percent} from {total} * 100 / {max}");
    }
    
    public void UpdateExpBarValue() 
    {
        UpdatePercentage(totalExp, maxExp);
        expText.text = $"Level: {level} ({percent}%) {totalExp}/{maxExp}";
        expSlider.value = totalExp;
    }

    public void SaveState(RGD_Game world)
    {
        var additionalData = world.GetAdditionalData();
        additionalData.SaveFromExpBar(this);
    }

    private void OnLevelUp()
    {
        Network_Player player = RAPI.GetLocalPlayer();
        player.SendChatMessage($"{EnchantingSystem.MOD_NAME} You leveled up from " + (level - 1) + " -> " + level);
    }

}
