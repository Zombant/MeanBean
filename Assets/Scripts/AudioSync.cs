using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(AudioSource))]
public class AudioSync : NetworkBehaviour {

    private AudioSource source;

    public AudioClip[] clips;

    private void Start() {
        source = gameObject.GetComponent<AudioSource>();
    }

    public void PlaySound(int id) {
        if (id >= 0 && id < clips.Length) {
            CmdSendServerSoundID(id);
        }
    }

    [Command]
    void CmdSendServerSoundID(int id) {
        RpcSendSoundIDToClients(id);
    }

    [ClientRpc]
    void RpcSendSoundIDToClients(int id) {
        source.PlayOneShot(clips[id]);
    }

}
