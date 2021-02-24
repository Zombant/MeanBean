using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {

    public TMP_Text playersList;
    public Button startButton;
    public TMP_Text teamText;


    private void Start() {
        GameObject.FindGameObjectWithTag("MainMenu").SetActive(false);
    }

    public void OnStartClick() {
        transform.parent.gameObject.GetComponent<PlayerSetup>().StartGame();
    }

    public void OnTeamChangeClick() {
        transform.parent.gameObject.GetComponent<PlayerSetup>().ChangeTeam();
        
    }
}
