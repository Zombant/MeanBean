using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerShoot : NetworkBehaviour {

    //Ammo Stuff
    [SyncVar] public Gun currentGun;
    private float timeBetweenShots;
    private float currentTimeBetweenShots;

    [Header("Types of Ammo")]
    public List<Gun> possibleGuns;

    [SerializeField] private int ammoLeft;


    //References
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask;
    private AudioSync audioSync;
    private PlayerMotor playerMotor;
    private GameObject playerUIInstance;

    
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject gunTip;
    private Vector3 lineCreatePoint;

    private bool shouldDrawPistolGraphics = false;
    private bool shouldDrawShotgunGraphics = false;


    private const string REMOTE_PLAYER_TAG = "RemotePlayer";

    private void Start() {
        if (cam == null) {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
        audioSync = gameObject.GetComponent<AudioSync>();
        playerMotor = gameObject.GetComponent<PlayerMotor>();
        playerUIInstance = gameObject.GetComponent<PlayerSetup>().playerUIInstance;

    }

    private void Update() {
        if (Input.GetButtonDown("Fire1") && currentGun.name != "Empty" && currentTimeBetweenShots <= 0) {
            currentTimeBetweenShots = timeBetweenShots;
            if (ammoLeft >= currentGun.ammoUsage) {
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            ammoLeft = 10;
        }
        currentTimeBetweenShots -= Time.deltaTime;
        GameObject.FindGameObjectWithTag("AmmoLeftText").GetComponent<TextMeshProUGUI>().SetText(ammoLeft.ToString());
    }

    private void LateUpdate() {
        if (shouldDrawPistolGraphics) {
            shouldDrawPistolGraphics = false;
            CmdDrawPistolGraphics(gunTip.transform.position, lineCreatePoint);
        }

        if (shouldDrawShotgunGraphics) {
            shouldDrawShotgunGraphics = false;
            CmdDrawShotgunGraphics(gunTip.transform.position);
        }
    }

    [Client]
    private void Shoot() {
        if (currentGun.name == "Pistol") {

            //Subtract ammoLeft by the ammoUsage
            ammoLeft -= currentGun.ammoUsage;
            if (ammoLeft <= 0) {
                ammoLeft = 0;
            }

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentGun.range, mask)) {
                if (hit.collider.tag == REMOTE_PLAYER_TAG) {
                    CmdPlayerShot(hit.collider.name, currentGun.damage);
                    Debug.Log("Shot" + hit.collider.name);
                }
                lineCreatePoint = hit.point;
            } else {
                lineCreatePoint = gunTip.transform.right * -currentGun.range;
            }

            //Line renderer
            shouldDrawPistolGraphics = true;
            PistolSound();
        }

        if (currentGun.name == "Shotgun") {
            Debug.Log("shotgun");
            //Subtract ammoLeft by the ammoUsage
            ammoLeft -= currentGun.ammoUsage;
            if (ammoLeft <= 0) {
                ammoLeft = 0;
            }

            //Random number of pellets
            int pelletsToFire = Random.Range(currentGun.minNumOfPellets, currentGun.maxNumOfPellets);

            //Raycast for each pellet
            for (int i = 0; i < pelletsToFire; i++) {

                Vector3 direction = cam.transform.forward;
                Vector3 spread = new Vector3();
                spread += cam.transform.up * Random.Range(-1f, 1f);
                spread += cam.transform.right * Random.Range(-1f, 1f);
                direction += spread.normalized * Random.Range(0f, 0.2f);
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, direction, out hit, currentGun.range, mask)) {
                    if (hit.collider.tag == REMOTE_PLAYER_TAG) {
                        CmdPlayerShot(hit.collider.name, currentGun.damage);                        
                    }
                    Debug.DrawLine(cam.transform.position, hit.point, Color.red, 1000);
                    
                    lineCreatePoint = hit.point;
                } else {
                    lineCreatePoint = gunTip.transform.right * -currentGun.range;
                }
                
            }

            shouldDrawShotgunGraphics = true;
            ShotgunSound();
        }

    }

    [Client]
    private void PistolSound() {
        audioSync.PlaySound(0);
    }
    [Command]
    void CmdDrawPistolGraphics(Vector3 startPos, Vector3 endPos) {

        gameObject.GetComponent<PlayerManager>().RpcShowPistolLines(startPos, endPos);
    }

    
    [Client]
    private void ShotgunSound() {
        //audioSync.PlaySound(0);
    }
    [Command]
    void CmdDrawShotgunGraphics(Vector3 startPos) {
        gameObject.GetComponent<PlayerManager>().RpcShotgunBurstEffect(startPos, cam.transform.rotation);
    }




    [Command]
    void CmdPlayerShot(string playerShot, int damage) {
        Debug.Log(playerShot + " has been shot");
        PlayerManager playerShotManager = GameManager.GetPlayer(playerShot);
        playerShotManager.RpcTakeDamage(damage);
    }



    public void OnTriggerEnter(Collider other) {

        //When gun pickup is touched
        foreach (Gun _gun in possibleGuns) {
            if (_gun.name == other.gameObject.tag) {

                SpawnObject spawner = other.transform.parent.GetComponent<SpawnObject>();
                spawner.Invoke("CreateNewObject", spawner.coolDown);

                timeBetweenShots = _gun.timeToNextShot;

                currentGun = _gun;


                Destroy(other.gameObject);

                Debug.Log("Item collected");
            }
        }

        //When ammo is touched
        if (other.gameObject.tag == "Ammo") {
            SpawnObject spawner = other.transform.parent.GetComponent<SpawnObject>();
            spawner.Invoke("CreateNewObject", spawner.coolDown);

            ammoLeft += other.gameObject.GetComponent<GiveAmmo>().ammoToGive;

            Destroy(other.gameObject);
        }
    }
}
