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
    public static GameObject expBar;
    public static Image expBackground;
    public static Slider expSlider;
    public static Image expIcon;
    public static Text expText;

    public static int level;
    public static double percent;

    public static int minExp;
    public static int maxExp;
    public static float totalExp;

    public Experience_Bar(AssetBundle asset)
    {
        expBar = asset.LoadAsset<GameObject>("exp_bar");
        expBackground = expBar.transform.GetChild(0).GetComponent<Image>();
        expSlider = expBar.transform.GetChild(1).GetComponent<Slider>();
        expIcon = expBar.transform.GetChild(2).GetComponent<Image>();
        expText = expBar.transform.GetChild(3).GetComponent<Text>();

        level = 1;
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

    public static void AddExperience(float amount)
    {

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

    public static void UpdatePercentage(float total, int max) 
    {
        percent = Math.Round((double)(total * 100.0) / max, 2);
    }
    
    public static void UpdateExpBarValue() 
    {
        UpdatePercentage(totalExp, maxExp);
        expText.text = $"Level: {level} ({percent}%) {totalExp}/{maxExp}";
        expSlider.maxValue = maxExp;
        expSlider.value = totalExp;
    }

    public void SaveState(RGD_Game world)
    {
        var additionalData = world.GetAdditionalData();
        additionalData.SaveFromExpBar(this);
    }

    public static void ResetValues()
    {
        level = 1;
        percent = 0;
        minExp = 0;
        maxExp = 100;
        totalExp = 0;
    }

    private static void OnLevelUp()
    {
        Network_Player player = RAPI.GetLocalPlayer();
        player.SendChatMessage($"{EnchantingSystem.MOD_NAME} You leveled up from " + (level - 1) + " -> " + level);
    }

    public static void AddExperienceFromItem(Item_Base item, int quantity, Dictionary<string, float> _type) => CheckConfigForItem(item.UniqueName, quantity, _type);
    public static void AddExperienceFromItem(ItemInstance item, int quantity, Dictionary<string, float> _type) => CheckConfigForItem(item, quantity, _type);
    public static void AddExperienceFromItem(PickupItem item, int quantity, Dictionary<string, float> _type) => CheckConfigForItem(item.PickupName, quantity, _type);
    public static void AddExperienceFromMob(AI_NetworkBehaviourType mob, int quantity, Dictionary<string, float> _type) => CheckConfigForItem(mob.ToString(), quantity, _type);

    private static void CheckConfigForItem(ItemInstance item, int quantity, Dictionary<string, float> _type)
    {
        string _name = item.UniqueName.Replace(" ", "_").ToLower();
        Debug.Log($"Alt names UnitqueName: {item.UniqueName}, Index: {item.UniqueIndex}, base name: {item.baseItem.UniqueName}, unique index: {item.baseItem.UniqueIndex} ");
        CheckConfigForItem(_name, quantity, _type);
    }

    private static void CheckConfigForItem(string item, int quantity, Dictionary<string, float> _type)
    {
        string _name = item.Replace(" ", "_").ToLower();
        if (ModConfig.IsConfigEventType(_type, _name, quantity, out float exp))
            AddExperience(exp);
    }
}
