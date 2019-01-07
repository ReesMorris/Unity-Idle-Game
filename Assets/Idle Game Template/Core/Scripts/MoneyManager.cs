using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoneyManager : MonoBehaviour {

    public delegate void OnMoneyChanged(double newAmount);
    public static OnMoneyChanged onMoneyChanged;

    // Define the instance so we can use it easily from other scripts
    public static MoneyManager Instance;

    [Header("Config")]
    [Tooltip("The filename of the text file containing names of money. Should be in the resources folder. Format should be FullName|SingleLetter (ie. Million|M)")] public string moneyNamesFile;
    [Tooltip("The amount of money the player starts the game with")] public float startingMoney;
    [Tooltip("The currency icon used throughout the game")] public string currencyIcon;
    [Tooltip("The minimum length of a number's digits before the first name (ie. Thousands) is displayed. Must be divisible by 3")] public int firstNameLength = 3;
    [Tooltip("The number of decimal places to display")] public int decimalPlaces = 3;
    [Tooltip("If enabled, the user's balance will never drop below zero")] public bool allowNegativeBalance;
    [Tooltip("After the length of MoneyNames.txt is exceeded, generate values from aa, ab, ac, ...?")] public bool autoNameBigMoney = true;
    [Tooltip("If not automatically naming big money, the string to appear once MoneyNames.txt is exceeded")] public string bigMoneyString = "a lot";

    // Private Variables
    public double Money { get; protected set; }
    List<string> moneyNames; // ie. Trillion
    List<string> moneyQuickhand; // ie. T
    GameManager gameManager;

    void Awake() {
        Instance = this;
    }

    void OnSetup() {
        // Set up the lists to contain all money names used in the game
        SetupMoneyNames();

        // Add references
        gameManager = GameManager.Instance;
    }

    // Set up the money (called during init phase of GameManager)
    public void Setup() {
        OnSetup();
        SetStartingMoney();
    }

    // Set the starting money UI
    void SetStartingMoney() {
        gameManager.ProcessBegin();
        if (PlayerPrefs.GetString("Money") == "" || PlayerPrefs.GetString("Money") == "0")
            SetMoney(startingMoney);
        else
            SetMoney(double.Parse(PlayerPrefs.GetString("Money")));
        gameManager.ProcessComplete();
    }
    
    // Set our balance to a specific amount
    public void SetMoney(double amount) {
        Money = amount;

        // Tell other scripts our balance has changed
        if (onMoneyChanged != null)
            onMoneyChanged(Money);

        // Save the new balance
        SaveData();
    }

    public void AddMoney(double amount) {
        Money += amount;

        // Tell other scripts our balance has changed
        if(onMoneyChanged != null)
            onMoneyChanged(Money);

        // Save the new balance
        SaveData();
    }

    public void ReduceMoney(double amount) {
        Money -= amount;

        if (!allowNegativeBalance)
            if (Money < 0)
                Money = 0;

        // Tell other scripts our balance has changed
        if (onMoneyChanged != null)
            onMoneyChanged(Money);

        // Save the new balance
        SaveData();
    }

    public bool CanAffordPurchase(double cost) {
        return Money > cost;
    }

    public string GetStringMoney(double money) {
        if (money.ToString("F0").Length > firstNameLength)
            return money.ToString("F0");
        return money.ToString("F" + decimalPlaces);
    }

    public int GetMoneyLength(double money) {
        string moneyString = GetStringMoney(money);
        if (moneyString.IndexOf('.') != -1)
            moneyString = moneyString.Split('.')[0];
        return moneyString.Length;
    }

    public string GetFormattedMoney(double money, bool showFullName) {
        string stringMoney = GetStringMoney(money);
        string displayMoney = stringMoney;
        int moneyLength = GetMoneyLength(money);

        // Add a decimal if there isn't one (and trim the fat)
        if (displayMoney.IndexOf('.') == -1) {

            // Add the decimal
            int decimalPos = moneyLength % 3;
            if (decimalPos == 0) decimalPos = 3;
            displayMoney = displayMoney.Insert(decimalPos, ".");

            // Trim the fat
            int preferredLength = decimalPos + (1 + decimalPlaces);
            if (displayMoney.Length > preferredLength)
                displayMoney = displayMoney.Substring(0, preferredLength);
        }

        // Calculate what name to show
        displayMoney += " ";
        if (moneyLength > firstNameLength) {
            int index = Mathf.FloorToInt((float)(moneyLength-1) / 3f) - (firstNameLength / 3);
            if(index >= moneyQuickhand.Count) {
                displayMoney += bigMoneyString;
            } else {
                if (showFullName)
                    displayMoney += moneyNames[index];
                else
                    displayMoney += moneyQuickhand[index];
            }

        }

        // Add the currency character
        displayMoney = displayMoney.Insert(0, currencyIcon);

        return displayMoney;
    }

    void SetupMoneyNames() {
        moneyNames = new List<string>();
        moneyQuickhand = new List<string>();

        TextAsset textFile = (TextAsset)Resources.Load(moneyNamesFile);
        StringReader reader = new StringReader(textFile.text);
        string fileContents = reader.ReadToEnd();
        reader.Close();

        string[] lines = fileContents.Split("\n"[0]);
        foreach(string line in lines) {
            string[] split = line.Split('|');
            moneyNames.Add(split[0]);
            moneyQuickhand.Add(split[1]);
        }

        if(autoNameBigMoney) {
            int index = 0;
            int pos = 0;
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for (int i = 0; i < (Mathf.Pow(letters.Length, 2)-1); i++) {
                pos++;
                if (pos >= letters.Length) {
                    index++;
                    pos = 0;
                }
                moneyNames.Add(letters[index] + letters[pos].ToString());
                moneyQuickhand.Add(letters[index] + letters[pos].ToString());
            }
        }
    }

    // Calculates total profits per minute
    public double CalculateMinutelyProfit() {
        double total = 0;
        foreach(BuyableData data in Idles.Instance.idles) {
            total += data.IdleEarnings;
        }
        return total * (gameManager.idleEarnings / 100f);
    }

    // Save money data when the player quits
    void SaveData() {
        PlayerPrefs.SetString("Money", Money.ToString());
    }

    // Reset all data
    public void ResetData() {
        PlayerPrefs.SetString("Money", "");
        SetMoney(startingMoney);
    }
}