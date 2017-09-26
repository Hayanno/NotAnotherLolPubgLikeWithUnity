using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    // A refaire avec une meilleure connaissances des structure du C#
    #region PLAYER_STRUCT
    public bool isBootsEquiped;
    #endregion

    #region PUBLIC_VARIABLE
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float speed = 2.0f;
    public float bulletSpeed = 3.0f;
    public float shootDistance = 10f;
    public float shootRate = .5f;
    #endregion

    #region PRIVATE_VARIABLE
    private NavMeshAgent navMeshAgent;
    private Transform targetedEnemy;
    private Ray shootRay;
    private RaycastHit shootHit;
    private bool walking;
    private bool enemyClicked;
    private float nextFire;
    #endregion

    #region PRIVATE_METHOD
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        // Si c'est n'est pas le personnage du joueur, on ne peut pas le controler
        if (!isLocalPlayer)
            return;

        MoveToMouse();
    }

    // Déplace le joueur à la position de la souris lors d'un clique droit
    private void MoveToMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("Fire2")) {
            if (Physics.Raycast(ray, out hit, 100)) {
                if (hit.collider.CompareTag("Enemy")) {
                    targetedEnemy = hit.transform;
                    enemyClicked = true;
                } else {
                    walking = true;
                    enemyClicked = false;
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.isStopped = false;
                }
            }
        }

        if (enemyClicked) {
            MoveAndShoot();
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
                walking = false;
        } else {
            walking = true;
        }
    }

    private void MoveAndShoot() {
        if (targetedEnemy == null)
            return;

        navMeshAgent.destination = targetedEnemy.position;

        if (navMeshAgent.remainingDistance >= shootDistance) {

            navMeshAgent.isStopped = false;
            walking = true;
        }

        if (navMeshAgent.remainingDistance <= shootDistance) {
            transform.LookAt(targetedEnemy);
            //Vector3 dirToShoot = targetedEnemy.transform.position - transform.position;

            if (Time.time > nextFire) {
                nextFire = Time.time + shootRate;

                CmdFire();
            }

            navMeshAgent.isStopped = true;
            walking = false;
        }
    }

    [Command]
    private void CmdFire() {
        var bullet = Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, bulletSpeed);
    }
    #endregion

    #region PUBLIC_METHOD
    public void SetSpeed(float speed) {
        GetComponent<NavMeshAgent>().speed = speed;
    }

    public void IsBootEquiped(bool isEquiped) {
        isBootsEquiped = isEquiped;
    }

    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = Color.blue;

        Camera.main.GetComponent<CameraFollow>().SetTarget(gameObject);
    }
    #endregion
}