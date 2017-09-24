using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    #region PUBLIC_VARIABLE
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    //public float speed = 2.0f;
    public float bulletSpeed = 3.0f;

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    #endregion

    #region PRIVATE_VARIABLE
    private CharacterController controller;

    private Vector3 dir;
    private Vector3 baseDir;

    private Vector3 moveDirection = Vector3.zero;

    private float moveTime = 0;
    #endregion

    void Start() {
        baseDir = transform.position;
        dir = baseDir;
    }

    void Update() {
        // Si c'est n'est pas le personnage du joueur, on ne peut pas le controller
        if (!isLocalPlayer)
            return;

        /*
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                moveTime = 0;
                dir = hit.point;
                baseDir = transform.position;
            }
        }

        moveTime += speed * Time.deltaTime;

        transform.position = new Vector3(Mathf.Lerp(baseDir.x, dir.x, moveTime), 0, Mathf.Lerp(baseDir.z, dir.z, moveTime));
        */

        MoveToMouse();
        HandleSpells();
    }

    void FixedUpdate() {
        RotateToMouse();
    }

    void HandleSpells() {
        // /!\ Changer le KeyCode par une touche administrable par Unity
        if (Input.GetKeyDown(KeyCode.A))
            CmdFire();
    }

    // Déplace le joueur à la position de la souris lors d'un clique droit
    void MoveToMouse() {
        CharacterController controller = GetComponent<CharacterController>();

        if (controller.isGrounded) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit)) {
                    moveTime = 0;
                    dir = hit.point;
                    baseDir = transform.position;
                }
            }

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void RotateToMouse() {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist)) {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }

    [Command]
    void CmdFire() {
        var bullet = (GameObject) Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, bulletSpeed);
    }

    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = Color.blue;

        Camera.main.GetComponent<CameraFollow>().SetTarget(gameObject.transform);
    }
}