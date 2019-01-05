using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buyable : MonoBehaviour {

    public BuyableData data;

    public Text costUI;
    public Button buttonUI;
    public Text ownedText;
    public Image ownershipMultiplierFill;
    public Text incomeText;
    public Image fillProgress;
    public Text timeText;

    private double cost;
    private int owned = 0;
    private MoneyManager moneyManager;
    private GameManager gameManager;
    private UIButton uiButton;
    private float buyCooldown;
    private float nextBuyTime;
    private float speed;
    private double profit;

	void Start () {
        moneyManager = MoneyManager.Instance;
        gameManager = GameManager.Instance;
        uiButton = buttonUI.GetComponent<UIButton>();
        MoneyManager.onMoneyChanged += OnMoneyChanged;

        speed = data.baseSpeed;
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

        if (owned > 0 && owned % gameManager.ownershipMultiplier == 0)
            speed /= 2;

        profit = data.profit * owned * data.profitMultiplier;

        UpdateUI();
        print(speed);
    }

    void UpdateUI() {
        uiButton.SetState(moneyManager.CanAffordPurchase(cost));
        costUI.text = moneyManager.GetFormattedMoney(cost, false);
        ownedText.text = owned.ToString();
        incomeText.text = moneyManager.GetFormattedMoney(profit, false);
        ownershipMultiplierFill.fillAmount = (owned % gameManager.ownershipMultiplier) / (float)gameManager.ownershipMultiplier;
    }

    void OnMoneyChanged() {
        UpdateUI();
    }

    IEnumerator Run() {
        float i = 0.01f; // smaller number means smoother UI but more calculations
        float t = 0;
        while(true) {
            if (speed > 0.25f)
                fillProgress.fillAmount = (t / speed);
            else
                fillProgress.fillAmount = 1f;

            t += i;
            if (t >= speed) {
                t = 0;
                moneyManager.AddMoney(profit);
            }
            yield return new WaitForSeconds(i);
        }
    }
}
