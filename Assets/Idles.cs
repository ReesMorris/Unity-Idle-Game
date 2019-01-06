using System.Collections;
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
    private int displayIndex;
    private MoneyManager moneyManager;
    private List<Buyable> prefabs;

    void Awake () {
        Instance = this;
        prefabs = new List<Buyable>();
        GameManager.resetAllData += ResetData;
        Buyable.onBuyablePurchase += OnBuyablePurchase;
        moneyManager = MoneyManager.Instance;
        displayIndex = PlayerPrefs.GetInt("IdlesDisplayIndex");
    }

    void Start () {
        DisplayBuyables();
    }

    void DisplayBuyables() {
        if(idles.Length > 0) {

            // Display all buyables from the start
            if(displayMode == DisplayModes.DisplayAll) {
                if (idles.Length == 0)
                foreach(BuyableData buyable in idles) {
                    DisplayBuyable(buyable);
                }
            }

            // Other options
            else {
                // Show the first one regardless
                DisplayBuyable(idles[0]);

                // Also display ones that have already been purchased (+1 to show the next purchase)
                for (int i = 1; i < displayIndex + 1; i++) {
                    if(i < idles.Length)
                        DisplayBuyable(idles[i]);
                }
            }
        }
    }

    // Called when a buyable is purchased (meaning 1 is now owned)
    void OnBuyablePurchase(Buyable b) {
        if(displayIndex < idles.Length) {
            if(displayMode == DisplayModes.DisplayNextAfterPurchase) {
                if(b.Data == idles[displayIndex]) {
                    if (IncrementDisplayIndex() < idles.Length)
                        DisplayBuyable(idles[displayIndex]);
                }
            }
        }
    }

    int IncrementDisplayIndex() {
        PlayerPrefs.SetInt("IdlesDisplayIndex", ++displayIndex);
        return displayIndex;
    }

    void DisplayBuyable(BuyableData buyableData) {
        Buyable prefab = Instantiate(idlePrefab, prefabContainer);
        prefabs.Add(prefab);
        prefab.Init(buyableData);
    }

    // Reset all data
    void ResetData() {
        while (prefabs.Count > 0) {
            prefabs[0].Data.ResetData();
            Destroy(prefabs[0].gameObject);
            prefabs.RemoveAt(0);
        }
        displayIndex = 0;
        PlayerPrefs.SetInt("IdlesDisplayIndex", 0);
        moneyManager.ResetData();
        DisplayBuyables();
    }
}
