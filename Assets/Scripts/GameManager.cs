using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    //Add player to dictionary and name player
    public static void RegisterPlayer(string _netID, PlayerManager _player) {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    //Remove player from dictionary
    public static void UnRegisterPlayer(string _playerID) {
        players.Remove(_playerID);
    }

    //Return player from dictionary
    public static PlayerManager GetPlayer(string _playerID) {
        return players[_playerID];
    }

}