﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idles : MonoBehaviour {

    public static Idles Instance;

    // Public Variables
    [Tooltip("The prefab that will be created for each buyable")] public Buyable idlePrefab;
    [Tooltip("The container that the idle prefabs will be put in")]public Transform prefabContainer;
    public enum DisplayModes { DisplayAll, DisplayNextAfterPurchase, DisplayWhenFundsAvailable };
    [Tooltip("The display mode for the idles.\n\nDisplayAll will show all when the game starts\n\nDisplayNextAfterPurchase will show the next unlock after the previous is unlocked\n\nDisplayWhenFundsAvailable will show once the player has the funds to unlock it")]
    public DisplayModes displayMode;
    public BuyableData[] idles;

    // Private Variables
    private bool initiated;
    private int displayIndex;

	void Awake () {
        Instance = this;
	}

    void Start () {
        DisplayBuyables();
    }

    void DisplayBuyables() {

        // Show all buyables from the start
        if(displayMode == DisplayModes.DisplayAll) {
            if(!initiated) {
                initiated = true;
                foreach(BuyableData buyable in idles) {
                    DisplayBuyable(buyable);
                }
            }
        }

        // 
        else if (displayMode == DisplayModes.DisplayNextAfterPurchase) {

        }
    }

    void DisplayBuyable(BuyableData buyableData) {
        Buyable prefab = Instantiate(idlePrefab, prefabContainer);
        prefab.Init(buyableData);
    }
}
