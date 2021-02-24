using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {

    public float coolDown;

    public GameObject itemToCreate;
    
    public void CreateNewObject() {
        Debug.Log("Respawn Initiated");
        Instantiate(itemToCreate, transform);
    }

    private void OnTriggerEnter(Collider other) {

        //Make summoned object a child
        if(other.gameObject.layer == LayerMask.NameToLayer("Pickups")) {
            other.transform.parent = gameObject.transform;
        }
    }

    private void Start() {
        Instantiate(itemToCreate, transform);
    }




}
