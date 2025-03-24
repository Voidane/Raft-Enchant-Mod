using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EnchantmentMenu_Inventory : Inventory
{
    protected override void Awake()
    {
        Debug.Log("EnchantmentMenu_Inventory override awake");

        // Check if RectTransform exists
        this.invRectTransform = GetComponent<RectTransform>();
        if (this.invRectTransform == null)
        {
            Debug.LogError("Missing RectTransform component on EnchantmentMenu_Inventory");
            return; // Early return to prevent further errors
        }

        // Check if gridLayoutGroup exists and is initialized
        if (this.gridLayoutGroup == null)
        {
            this.gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
            if (this.gridLayoutGroup == null)
            {
                Debug.LogError("Missing GridLayoutGroup component for EnchantmentMenu_Inventory");
                return;
            }
        }

        // Make sure hoverTransform exists
        if (this.hoverTransform == null)
        {
            Debug.LogError("hoverTransform is null in EnchantmentMenu_Inventory");
        }
        else
        {
            this.hoverTransform.sizeDelta = this.gridLayoutGroup.cellSize;
        }

        // Make sure darkenedTransform exists
        if (this.darkenedTransform == null)
        {
            Debug.LogError("darkenedTransform is null in EnchantmentMenu_Inventory");
        }
        else
        {
            this.darkenedTransform.sizeDelta = this.gridLayoutGroup.cellSize;
        }

        // Make sure draggingImage exists
        if (this.draggingImage == null)
        {
            Debug.LogError("draggingImage is null in EnchantmentMenu_Inventory");
        }
        else
        {
            this.draggingImage.rectTransform.sizeDelta = this.gridLayoutGroup.cellSize * 0.75f;
        }

        // Initialize slots if needed
        if (this.allSlots == null)
        {
            this.allSlots = new List<Slot>();
        }

        this.InitializeSlots();

        // Call base.Awake() after our initialization to ensure our setup is done first
        base.Awake();
    }
}
