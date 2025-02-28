using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnchantmentBenchObject : MonoBehaviour_ID_Network, IRaycastable
{
    public void OnIsRayed()
    {

    }

    public void OnRayEnter()
    {

    }

    public void OnRayExit()
    {

    }

    public static CostMultiple[] CostToCraft()
    {
        return new CostMultiple[]
        {
            new CostMultiple()
            {
                items = new Item_Base[] { ItemManager.GetItemByName("Plank") },
                amount = 1
            },
            new CostMultiple()
            {
                items = new Item_Base[] { ItemManager.GetItemByName("Clay") },
                amount = 3
            },
        };
    }

}
