using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnchantmentBenchObject : MonoBehaviour_ID_Network, IRaycastable
{

    private CanvasHelper canvas;
    private Network_Player player;
    private bool isOpen;
    private bool showingText;
    public bool placed = false;
    private bool canCloseWithUseButton;
    private StorageManager storageManager;

    public void Update()
    {
        if (isOpen && PlayerItemManager.IsBusy)
        {
            if (MyInput.GetButtonDown("Inventory") || MyInput.GetButtonDown("Cancel"))
            {
                Debug.Log("CANCEL");
                PlayerItemManager.IsBusy = false;
                this.isOpen = false;
            }
        }
    }

    public async void OnIsRayed()
    {
        if (!placed)
            return;

        while (this.canvas == null)
        {
            this.canvas = ComponentManager<CanvasHelper>.Value;
            await Task.Delay(1);
        }

        while (this.player == null)
        {
            this.player = RAPI.GetLocalPlayer();
            await Task.Delay(1);
        }

        if (CanvasHelper.ActiveMenu != MenuType.None || !Helper.LocalPlayerIsWithinDistance(base.transform.position, Player.UseDistance))
        {
            if (this.showingText)
            {
                this.canvas.displayTextManager.HideDisplayTexts();
                this.showingText = false;
            }
            return;
        }

        if (this.storageManager == null)
        {
            Network_Player value = ComponentManager<Network_Player>.Value;
            if (value != null)
            {
                this.storageManager = value.StorageManager;
            }
            return;
        }

        if (!this.isOpen && !PlayerItemManager.IsBusy && this.canvas.CanOpenMenu && Helper.LocalPlayerIsWithinDistance(base.transform.position, Player.UseDistance + 0.0F))
        {
            this.canvas.displayTextManager.ShowText("Enchant an Item", MyInput.Keybinds["Interact"].MainKey, 0, 0, true);
            this.showingText = true;

            if (MyInput.GetButtonDown("Interact"))
            {
                this.canvas.displayTextManager.HideDisplayTexts();

                // TODO: Load Asset Menu for enchant table
                this.canvas.OpenMenuCloseOther(MenuType.Inventory, false);
                PlayerItemManager.IsBusy = true;
                this.isOpen = true;
                base.CancelInvoke("AllowCloseWithUseButton");
                base.Invoke("AllowCloseWithUseButton", 0.15f);
            }
        }
        else
        {
            this.canvas.displayTextManager.HideDisplayTexts();
        }
    }

    public void OnRayEnter() { }

    public void OnRayExit()
    {
        if (this.canvas != null && this.canvas.displayTextManager != null)
        {
            this.canvas.displayTextManager.HideDisplayTexts();
        }
    }

    protected override void OnDestroy()
    {
        NetworkIDManager.RemoveNetworkID(this);
        Debug.Log("OnDestroy sent");
        base.OnDestroy();
    }

    public void OnBlockPlaced()
    {
        placed = true;
        NetworkIDManager.AddNetworkID(this);
    }

    public override RGD Serialize_Save()
    {
        return new RGD_Block(RGDType.Block, base.GetComponent<Block>());
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
