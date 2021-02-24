using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotate : MonoBehaviour {
    [SerializeField] private GameObject playerCam;

    private void Update() {
        gameObject.transform.localRotation = playerCam.transform.rotation;
    }
}
