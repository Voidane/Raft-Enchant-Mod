using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigData
{
    public Dictionary<string, float> Pickup_Exp;
    public Dictionary<string, float> Mob_Kill_Exp;
    public Dictionary<string, float> Fishing_Exp;
    public Dictionary<string, float> Tree_Harvest_Exp;
}