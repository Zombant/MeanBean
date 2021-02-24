using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnCollisionEnter(Collision other) {
        if(other.gameObject.GetComponent<PlayerManager>() == null) { return; }
        if(GetComponent<PlayerManager>().team == PlayerManager.Team.attackers && other.gameObject.GetComponent<PlayerManager>().team == PlayerManager.Team.defenders) {
            other.gameObject.GetComponent<PlayerManager>().RpcTakeDamage(100);
        }
    }*/
}
