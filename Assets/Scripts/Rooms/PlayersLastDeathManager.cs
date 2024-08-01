using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLastDeathManager : MonoBehaviour
{
    //Each room has a Room_LastUpgradeHolder.
    //When player dies we save the index of that room in the GameState
    //If when died it still had any upgrade, the bool in the GameState "isLostUpgradeAvailable" is turnt on
    //The script in every room, when the room is completed will check if its own index matches with the index in the gameState. If it does, spawns an upgrade
    //The upgrade lost to spawn is selected from the players_upgradeManager

    [SerializeField] GameState gameState;
    
    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += SetRoomsUpgrade;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= SetRoomsUpgrade;
    }
    void SetRoomsUpgrade()
    {
        gameState.IndexOfLostUpgradeRoom = gameState.currentPlayersRooms[gameState.currentPlayersRooms.Count - 1].indexInCompleteList;

        if (gameState.playerUpgrades.Count == 0) 
        {
            gameState.isLostUpgradeAvailable = false;
            return; 
        }

        gameState.isLostUpgradeAvailable = true;
    }
}
