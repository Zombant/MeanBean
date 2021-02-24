using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
public class MainMenuUI : MonoBehaviour {

    [SerializeField] private BeanNetworkManager networkManager;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Canvas mainMenuCanvas;

    [HideInInspector] public string userName;

    public void OnHostButtonClick() {
        userName = nameField.text;
        if (nameField.text == null || nameField.text == "") {
            return;
        }

        networkManager.StartHost();
        mainMenuCanvas.gameObject.SetActive(false);
        
    }

    public void OnConnectButtonClick() {
        
        userName = nameField.text;
        if (nameField.text == null || nameField.text == "") {
            return;
        }

        networkManager.networkAddress = ipField.text;

        
        networkManager.StartClient();
        cancelButton.gameObject.SetActive(true);
        connectButton.gameObject.SetActive(false);
        ipField.interactable = false;
        nameField.interactable = false;
        hostButton.interactable = false;
        
    }

    public void OnCancelClick() {
        networkManager.StopClient();
        cancelButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(true);
        ipField.interactable = true;
        nameField.interactable = true;
        hostButton.interactable = true;
    }

}
