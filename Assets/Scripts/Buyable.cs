using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buyable : MonoBehaviour {

    public BuyableData data;

    public Text costUI;
    public Button buttonUI;
    public Text ownedText;

    private double cost;
    private int owned = 0;
    private MoneyManager moneyManager;
    private GameManager gameManager;
    private UIButton uiButton;
    private float buyCooldown;
    private float nextBuyTime;

	void Start () {
        moneyManager = MoneyManager.Instance;
        gameManager = GameManager.Instance;
        uiButton = buttonUI.GetComponent<UIButton>();
        MoneyManager.onMoneyChanged += OnMoneyChanged;

        cost = data.baseCost;

        UpdateUI();

    }

    void Update() {
        if(uiButton.Pressed) {
            if(Time.time > nextBuyTime) {
                buyCooldown -= 0.01f; // make it so that holding the button down increases buy speed over time (until released)
                nextBuyTime = Time.time + buyCooldown;
                if (moneyManager.CanAffordPurchase(cost))
                    OnPurchase();
            }
        } else {
            buyCooldown = gameManager.baseBuyCooldown;
        }
    }

    void OnPurchase() {
        owned++;

        if (owned == 1)
            StartCoroutine(Run());

        moneyManager.ReduceMoney(cost);
        cost *= data.upgradeCostIncrement;

        UpdateUI();
    }

    void UpdateUI() {
        uiButton.SetState(moneyManager.CanAffordPurchase(cost));
        costUI.text = moneyManager.GetFormattedMoney(cost, false);
        ownedText.text = owned.ToString();
    }

    void OnMoneyChanged() {
        UpdateUI();
    }

    IEnumerator Run() {
        float i = 0.01f; // smaller number means smoother UI but more calculations
        float t = 0;
        while(true) {
            t += i;
            if(t >= data.baseSpeed) {
                t = 0;
                moneyManager.AddMoney(data.profit * owned);
            }
            yield return new WaitForSeconds(i);
        }
    }
}
