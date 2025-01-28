using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ES_Tools
{
    public static Dictionary<string, int> DictItemDifferenceFromInventory(Dictionary<string, int> before, Dictionary<string, int> after)
    {
        Dictionary<string, int> differences = new Dictionary<string, int>();

        foreach (var kvp in after)
        {
            int initQuantity = before.ContainsKey(kvp.Key) ? before[kvp.Key] : 0;

            if (kvp.Value > initQuantity)
            {
                differences.Add(kvp.Key, kvp.Value - initQuantity);
            }
        }
        return differences;
    }

    public static Dictionary<ItemInstance, int> ItemDifferenceFromInventory(Dictionary<ItemInstance, int> before, Dictionary<ItemInstance, int> after)
    {
        Dictionary<ItemInstance, int> differences = new Dictionary<ItemInstance, int>();

        foreach (var kvp in after)
        {
            int initQuantity = before.ContainsKey(kvp.Key) ? before[kvp.Key] : 0;

            if (kvp.Value > initQuantity)
            {
                differences.Add(kvp.Key, kvp.Value - initQuantity);
            }
        }
        return differences;
    }

    public static void CreateInventorySnapshotDictionary(List<Slot> inventory, Dictionary<ItemInstance, int> dict)
    {
        foreach (var slot in inventory)
        {
            if (!slot.IsEmpty && slot.itemInstance != null)
            {
                ItemInstance item = slot.itemInstance;
                int amount = slot.GetCurrentTotalUses();

                if (dict.ContainsKey(item))
                {
                    dict[item] += amount;
                }
                else
                {
                    dict[item] = amount;
                }
            }
        }
    }
}
