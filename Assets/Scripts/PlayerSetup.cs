using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    private Behaviour[] componentsToDisable;
    [SerializeField] private Behaviour[] componentsToStartDisabled;

    [SerializeField] private LobbyUI lobbyUI;

    [SerializeField] private string remotePlayerLayerName = "RemotePlayer";

    [SerializeField] private GameObject playerUIPrefab;
    public GameObject playerUIInstance;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private Rigidbody rb;

    private Camera sceneCamera;

    public GameObject beanMesh;

    private bool isHost = true;
    [SyncVar] public string userName;
    [SyncVar] public bool hasStarted = false;

    [SerializeField] private float attackersWalkingSpeed;
    [SerializeField] private float attackersRunnningSpeed;
    [SerializeField] private float defendersWalkingSpeed;
    [SerializeField] private float defendersRunningSpeed;

    private void Start() {

        DisableSceneCamera();

        if (!(isLocalPlayer && isServer)) {
            isHost = false;
            lobbyUI.startButton.gameObject.SetActive(false);
        }
        
        if (!isLocalPlayer) {
            DisableComponents();
            
            AssignRemoteLayer();
            ChangePhysicsMaterial();
            lobbyUI.gameObject.SetActive(false);
        } else {
            CmdUpdateUserName(GameObject.Find("MainMenuUI").GetComponent<MainMenuUI>().userName);
            CreatePlayerUI();
            beanMesh.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

    }

    

    private void Update() {
        UpdateUsernameList();
        UpdateTeamText();

        if (!(isLocalPlayer && isServer)) {
            
            GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("RemotePlayer");
            foreach (GameObject obj in otherPlayers) {
                if (obj.GetComponent<PlayerSetup>().hasStarted) {
                    StartGame();
                }
            }
        }
    }

    private void OnDisable() {

        //Remove player ui
        DestroyPlayerUI();

        //Turn back on scene camera
        EnableSceneCamera();

        UnRegisterPlayerWithGameManager();

    }

    public override void OnStartClient() {
        base.OnStartClient();
        RegisterPlayerWithGameManager();
        GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("RemotePlayer");
        foreach (GameObject obj in otherPlayers) {
            if (obj.GetComponent<PlayerSetup>().hasStarted) {
                NetworkClient.Disconnect();
            }
        }
    }

    //Add max friction to remote players to prevent sliding around
    private void ChangePhysicsMaterial() {
        PhysicMaterial material = new PhysicMaterial("Max Friction");
        material.dynamicFriction = 1f;
        material.staticFriction = 1f;
        material.bounciness = 0f;
        material.frictionCombine = PhysicMaterialCombine.Maximum;
        material.bounceCombine = PhysicMaterialCombine.Average;
        collider.material = material;
        rb.isKinematic = false;
    }


    //Add player to dictionary of players
    private void RegisterPlayerWithGameManager() {
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    //Remove player from dictionary of players
    private void UnRegisterPlayerWithGameManager() {
        GameManager.UnRegisterPlayer(transform.name);
    }

    //Disable components in componentsToDisable on non-local players
    public void DisableComponents() {
        if (!isLocalPlayer) {
            for (int i = 0; i < componentsToDisable.Length; i++) {
                componentsToDisable[i].enabled = false;
            }
        }
    }

    public void StartGame() {
        
        CmdTurnOffLobbyUI();
    }

    [Command]
    public void CmdTurnOffLobbyUI() {
        if (!hasStarted) {
            RpcTurnOffLobbyUI();
            hasStarted = true;
        }
    }

    [ClientRpc]
    public void RpcTurnOffLobbyUI() {
        if (!isLocalPlayer) return;
        for (int i = 0; i < componentsToStartDisabled.Length; i++) {
            componentsToStartDisabled[i].enabled = true;
        }
        lobbyUI.gameObject.SetActive(false);
        DisableComponents();
        DisableSceneCamera();
    }

    //Assign non-local players to the RemotePlayer layer
    private void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remotePlayerLayerName);
        //beanMesh.layer = LayerMask.NameToLayer(remotePlayerLayerName);
        gameObject.tag = remotePlayerLayerName;
        //beanMesh.tag = remotePlayerLayerName;
        
    }

    private void DisableSceneCamera() {
        sceneCamera = Camera.main;
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(false);
        }
    }

    private void EnableSceneCamera() {
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
        }
    }

    private void CreatePlayerUI() {
        playerUIInstance = Instantiate(playerUIPrefab);
        playerUIInstance.name = playerUIPrefab.name;
    }

    private void DestroyPlayerUI() {
        Destroy(playerUIInstance);
    }

    
    private void UpdateUsernameList() {
        string _nameList = "";


        if (GetComponent<PlayerManager>().team == PlayerManager.Team.attackers) {
            _nameList += userName + " - " + "Attackers";
        } else {
            _nameList += userName + " - " + "Defenders";
        }


        GameObject[] list = GameObject.FindGameObjectsWithTag("RemotePlayer");
        foreach(GameObject obj in list) {
            if (obj.GetComponent<PlayerManager>().team == PlayerManager.Team.attackers) {
                _nameList = _nameList + "\n" + obj.GetComponent<PlayerSetup>().userName + " - " + "Attackers";
            } else {
                _nameList = _nameList + "\n" + obj.GetComponent<PlayerSetup>().userName + " - " + "Defenders";
            }
        }
        lobbyUI.playersList.text = _nameList;
    }

    private void UpdateTeamText() {
        if(GetComponent<PlayerManager>().team == PlayerManager.Team.attackers) {
            lobbyUI.teamText.text = "Attackers";
        } else {
            lobbyUI.teamText.text = "Defenders";
        }
    }

    public void ChangeTeam() {
        CmdChangeTeam();
    }

    [Command]
    public void CmdChangeTeam() {
        if (GetComponent<PlayerManager>().team == PlayerManager.Team.attackers) {
            GetComponent<PlayerManager>().team = PlayerManager.Team.defenders;
            GetComponent<PlayerController>().ChangeWalkingSpeed(defendersWalkingSpeed);
            GetComponent<PlayerController>().ChangeRunningSpeed(defendersRunningSpeed);
        } else {
            GetComponent<PlayerManager>().team = PlayerManager.Team.attackers;
            GetComponent<PlayerController>().ChangeWalkingSpeed(attackersWalkingSpeed);
            GetComponent<PlayerController>().ChangeRunningSpeed(attackersRunnningSpeed);
        }
    }

    [Command]
    public void CmdUpdateUserName(string name) {
        userName = name;
    }
}
