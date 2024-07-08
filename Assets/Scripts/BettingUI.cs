using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BettingUI : MonoBehaviour
{
    public Button foldButton;
    public Button callButton;
    public Button raiseButton;
    public Slider raiseSlider;
    public TextMeshProUGUI totalPotText;
    public TextMeshProUGUI currentBetText;
    public TextMeshProUGUI playerChipsText;

    private BetManager betManager;
    private Player currentPlayer;

    void Start()
    {
        // Initialize BetManager with dummy players for demonstration
        List<Player> players = new List<Player>
        {
            new Player(1000),
            new Player(1000),
            new Player(1000)
        };
        betManager = new BetManager(players, 10, 20);
        currentPlayer = players[0];

        // Add listeners to buttons
        foldButton.onClick.AddListener(OnFoldButtonClick);
        callButton.onClick.AddListener(OnCallButtonClick);
        raiseButton.onClick.AddListener(OnRaiseButtonClick);

        // Initialize UI
        UpdateUI();
    }

    public void OnFoldButtonClick()
    {
        betManager.PlayerFold();
        UpdateUI();
    }

    public void OnCallButtonClick()
    {
        betManager.PlayerCall();
        UpdateUI();
    }

    public void OnRaiseButtonClick()
    {
        int raiseAmount = (int)raiseSlider.value;
        betManager.PlayerRaise(raiseAmount);
        UpdateUI();
    }

    void UpdateUI()
    {
        totalPotText.text = "Total Pot: " + betManager.GetTotalPot();
        currentBetText.text = "Current Bet: " + betManager.GetCurrentBet();
        playerChipsText.text = "Your Chips: " + currentPlayer.TotalChips;
    }
}
