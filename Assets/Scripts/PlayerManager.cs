using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour {

    public GameObject shotgunBurst;
    private GameObject sceneCamera;
    public GameObject playerPrefab;

    private GameObject[] spawnPoints;

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SyncVar]
    [SerializeField]
    private int currentHealth;

    [SerializeField] private LineRenderer renderer;
    [SerializeField] private float pistolGraphicsDuration;



    [SerializeField] private int maxHealth = 100;

    [System.Serializable]
    public enum Team { attackers, defenders }
    
    [SyncVar] public Team team;

    private void Awake() {
        SetDefaultHealth();
        sceneCamera = GameObject.Find("SceneCamera");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnLocation");
    }

    public void SetDefaultHealth() {
        currentHealth = maxHealth;
    }

    public override void OnStartClient() {
        base.OnStartClient();
    }

    private void Update() {
        if (isLocalPlayer) {
            GameObject.FindGameObjectWithTag("HealthLeftText").GetComponent<TextMeshProUGUI>().SetText(currentHealth.ToString());
        }
        if (currentHealth <= 0) {
            Die();
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount) {
        if (isDead)
            return;
        currentHealth -= _amount;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }

    private void Die() {
        //Die logic
        //isDead = true;
        //Destroy(gameObject);
        //Debug.Log("dead");
        //sceneCamera.SetActive(true);

        //TODO: DELAY
        transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        currentHealth = maxHealth;
    }



    [ClientRpc]
    public void RpcShowPistolLines(Vector3 startPos, Vector3 endPos) {
        renderer.positionCount = 2;
        renderer.SetPosition(0, startPos);
        renderer.SetPosition(1, endPos);
        Invoke("HideZapLines", pistolGraphicsDuration);
    }

    private void HideZapLines() {
        renderer.positionCount = 0;
    }

    [ClientRpc]
    public void RpcShotgunBurstEffect(Vector3 startPos, Quaternion rotation) {
        //GameObject effect = Instantiate(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<BeanNetworkManager>().spawnPrefabs.Find(prefab => prefab.name == "ShotgunBurst"));
        Instantiate(shotgunBurst, startPos, rotation);
    }

}
