using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idles : MonoBehaviour {

    public static Idles Instance;

    // Public Variables
    [Tooltip("The prefab that will be created for each buyable")] public Buyable idlePrefab;
    [Tooltip("The container that the idle prefabs will be put in")]public Transform prefabContainer;
    public enum DisplayModes { DisplayAll, DisplayNextAfterPurchase, DisplayWhenFundsAvailable, Custom };
    [Tooltip("The display mode for the idles.\n\nDisplayAll will show all when the game starts\n\nDisplayNextAfterPurchase will show the next unlock after the previous is unlocked\n\nDisplayWhenFundsAvailable will show once the player has the funds to unlock it (even if the previous upgrade is not yet purchased)\n\nCustom by default will not do anything, but you are welcome to implement your own code in this script")]
    public DisplayModes displayMode;
    public BuyableData[] idles;

    // Private Variables
    private int displayIndex;
    private MoneyManager moneyManager;
    private List<Buyable> prefabs;
    private bool setupComplete;
    private GameManager gameManager;

    void Awake () {
        Instance = this;
        prefabs = new List<Buyable>();
    }

    void OnSetup () {
        moneyManager = MoneyManager.Instance;
        gameManager = GameManager.Instance;
        displayIndex = PlayerPrefs.GetInt("IdlesDisplayIndex");

        // Add event listeners
        Buyable.onBuyablePurchase += OnBuyablePurchase;
        MoneyManager.onMoneyChanged += OnMoneyChanged;
        GameManager.resetAllData += ResetData;
    }

    // Set up the idles (called during init phase of GameManager)
    public void Setup() {
        OnSetup();
        DisplayBuyables();
    }

    void DisplayBuyables() {
        gameManager.ProcessBegin();
        if(idles.Length > 0) {

            // Display all buyables from the start
            if(displayMode == DisplayModes.DisplayAll) {
                foreach(BuyableData buyable in idles) {
                    DisplayBuyable(buyable);
                }
            }

            // Other options
            else if (displayMode != DisplayModes.Custom) {
                // Show the first one regardless
                DisplayBuyable(idles[0]);

                // Also display ones that have already been purchased (+1 to show the next purchase)
                for (int i = 1; i < displayIndex + 1; i++) {
                    if(i < idles.Length) {
                        DisplayBuyable(idles[i]);
                    }
                }

                // So that we don't end up showing the first buyable twice
                if (displayIndex == 0)
                    displayIndex = 1;

                // We're done here, other functions can now handle adding more
                setupComplete = true;
            }

            // Custom option
            else {
                /*
                 *  Feel free to implement your own code here to handle when to display a buyable
                 *  To add a buyable to the screen, call DisplayBuyable() passing in BuyableData as a parameter
                 *  The BuyableData can be fetched from the 'idles' variable - ie. DisplayBuyable(idles[0]) will show the first
                 */
            }
        }
        gameManager.ProcessComplete();
    }

    // Called when a buyable is purchased (meaning 1 is now owned)
    void OnBuyablePurchase(Buyable b) {
        if(displayIndex < idles.Length && setupComplete) {
            if(displayMode == DisplayModes.DisplayNextAfterPurchase) {
                if(b.Data == idles[displayIndex]) {
                    DisplayBuyable(idles[displayIndex]);
                    IncrementDisplayIndex();
                }
            }
        }
    }

    // Called when the user's income changes
    void OnMoneyChanged(double amount) {
        if (displayIndex < idles.Length && setupComplete) {
            if(displayMode == DisplayModes.DisplayWhenFundsAvailable) {
                if(amount >= idles[displayIndex].Cost) {
                    DisplayBuyable(idles[displayIndex]);
                    IncrementDisplayIndex();
                }
            }
        }
    }

    // Increment the current index of idle purchases and also save it
    int IncrementDisplayIndex() {
        PlayerPrefs.SetInt("IdlesDisplayIndex", ++displayIndex);
        return displayIndex;
    }

    // Draw up the prefab for the buyable on the screen
    void DisplayBuyable(BuyableData buyableData) {
        Buyable prefab = Instantiate(idlePrefab, prefabContainer);
        prefabs.Add(prefab);
        prefab.Init(buyableData);
    }

    // Resets all data
    void ResetData() {
        while (prefabs.Count > 0) {
            prefabs[0].Data.ResetData();
            Destroy(prefabs[0].gameObject);
            prefabs.RemoveAt(0);
        }
        displayIndex = 0;
        setupComplete = false;
        PlayerPrefs.SetInt("IdlesDisplayIndex", 0);
        moneyManager.ResetData();
        DisplayBuyables();
    }
}
