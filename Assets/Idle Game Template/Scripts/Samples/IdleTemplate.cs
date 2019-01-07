using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Buyable))]
public class IdleTemplate : MonoBehaviour {

    // This is a template script which contains listeners to all of the buyable events
    // Attach this to the idle prefab that you want to use this with and ensure that the
    // prefab itself is attached to the Idles script in GameManager
    // This script is intended to primarily be used to update the UI of your prefab so do
    // feel free to take a look at either Idle1.cs or Idle2.cs for some examples of how the
    // functions below can be used. You're also more than welcome to let me know if you have questions

    // Private Variables
    private Buyable buyable;
    private Utilities utilities;
    private MoneyManager moneyManager;

    void Start() {
        buyable = GetComponent<Buyable>();
        utilities = Utilities.Instance;
        moneyManager = MoneyManager.Instance;

        // Event listeners from the Buyable script
        Buyable.onVariableChanged += OnVariableChanged;
        Buyable.onProcessBegin += OnProcessBegin;
        Buyable.onProcessUpdate += OnProcessUpdate;
        Buyable.onProcessFinish += OnProcessFinish;
        Buyable.onManagerHired += OnManagerHired;
        Buyable.onBuyablePurchase += OnBuyablePurchase;
        BuyableData.onDataLoaded += OnBuyableDataLoaded;
        GameManager.onLoadingComplete += OnLoadingComplete;
    }

    
    /* Event listeners (in order of execution where applicable) */

    // Called after the data for a buyable has been loaded once the game begins
    void OnBuyableDataLoaded(BuyableData b) {
        if(b == buyable.Data) {



        }
    }

    // Called by GameManager once the game is fully loaded and all idles have been added
    void OnLoadingComplete() {
        GameManager.onLoadingComplete -= OnLoadingComplete;

    }

    // Called when a buyable variable is changed (ie. cost, speed, ...)
    void OnVariableChanged(Buyable b) {
        if (b == buyable) {



        }
    }

    // Called when a buyable is purchased (meaning 1 is now owned)
    void OnBuyablePurchase(Buyable b) {
        if(b == buyable) {



        }
    }

    // Called when the production process begins
    void OnProcessBegin(Buyable b) {
        if (b == buyable) {



        }
    }

    // Called when the production process percentage is changed
    void OnProcessUpdate(Buyable b, float progress) {
        if (b == buyable) {



        }
    }

    // Called when the production process finishes (money has already been given to the player)
    void OnProcessFinish(Buyable b) {
        if (b == buyable) {



        }
    }

    // Called after a manager has been purchased
    void OnManagerHired(Buyable b) {
        if (b == buyable) {



        }
    }
}