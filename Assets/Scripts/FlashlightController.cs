using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlashlightController : NetworkBehaviour {

    public GameObject flashlight;
    private bool flashlightOn = true;


    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (flashlightOn) {
                CmdTurnOffFlashlight();
                flashlightOn = false;
            } else {
                CmdTurnOnFlashlight();
                flashlightOn = true;
            }
        }

    }

    [Command]
    void CmdTurnOnFlashlight() {
        RpcTurnOnFlashlight();
    }

    [ClientRpc]
    void RpcTurnOnFlashlight() {
        flashlight.SetActive(true);
    }

    [Command]
    void CmdTurnOffFlashlight() {
        RpcTurnOffFlashlight();
    }

    [ClientRpc]
    void RpcTurnOffFlashlight() {
        flashlight.SetActive(false);
    }
}
