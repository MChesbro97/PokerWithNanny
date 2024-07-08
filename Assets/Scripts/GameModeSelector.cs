using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameModeSelector : MonoBehaviour
{
    public TMP_Dropdown gameModeDropdown;
    public PokerGameMode selectedGameMode;

    private void Start()
    {
        PopulateDropdown();

        // Add listener for when the dropdown value changes
        gameModeDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(gameModeDropdown);
        });

        // Set initial game mode based on dropdown value
        SetGameMode(gameModeDropdown.value);
    }
    void PopulateDropdown()
    {
        // Clear existing options
        gameModeDropdown.ClearOptions();

        // Get the names of the enum values
        var options = new List<string>(System.Enum.GetNames(typeof(PokerGameMode)));

        // Add the options to the dropdown
        gameModeDropdown.AddOptions(options);
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        // Set the game mode based on the dropdown value
        SetGameMode(change.value);
    }

    void SetGameMode(int index)
    {
        selectedGameMode = (PokerGameMode)index;
        Debug.Log("Selected Game Mode: " + selectedGameMode);
        Card.SetGameMode(selectedGameMode); // Assuming you have a method in your Card class to set the game mode
    }
    public PokerGameMode GetSelectedGameMode()
    {
        return (PokerGameMode)gameModeDropdown.value;
    }
}
