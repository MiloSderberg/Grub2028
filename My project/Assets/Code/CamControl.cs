using UnityEngine;

public class CameControl : MonoBehaviour
{
    public GameObject camBase;
    public Camera cam;
    float camCamDistance;
    public float camCamDistanceT;
    public float mouseSensitivity;
    public float camMaxDistance;
    public float responsiveness;
    public float resetSpeed;
    float unabstructedZoom;
    RaycastHit hit2;
    bool isLookingAround;
    float timer;
    public float yaw;
    public float pitch;
    float camMx;
    float camMy;
    public bool isForPlane;
    public GameObject head;
    public GameObject map;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unabstructedZoom = camCamDistanceT;
    }

    // Update is called once per frame
    void Update()
    {
        if (isForPlane) camBase.transform.position = transform.position;
        camBase.transform.position = head.transform.position;

        RotateCamera();
        ZoomCameraWithCollision();
    }

    void RotateCamera()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) Cursor.lockState = CursorLockMode.Locked;

        camMx = Input.GetAxis("Mouse X") * mouseSensitivity;
        camMy = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += camMx;
        pitch -= camMy;

        if (Input.GetKey(KeyCode.L))
        {
            // Make a separate lockon system where you can select an enemy that you currently are engaging.
            // This enemy should be able to be looked at and it should hold priority when locking on to enemies.
        }
        else
        {
            timer -= Time.deltaTime;

            if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
            {
                isLookingAround = true; 
                timer = 0.25f;
            }
            else if (timer <= 0 && isForPlane) isLookingAround = false;

            if (isLookingAround)
            {
                if (isForPlane) yaw = Mathf.Clamp(yaw, -180f, 180f);
                pitch = Mathf.Clamp(pitch, -75, 75);
                camBase.transform.rotation = Quaternion.Lerp(camBase.transform.rotation, 
                    Quaternion.AngleAxis(yaw, transform.up) * map.transform.rotation * Quaternion.AngleAxis(pitch, Vector3.right),
                    resetSpeed * Time.deltaTime * Quaternion.Angle(camBase.transform.rotation, 
                    Quaternion.AngleAxis(yaw, transform.up) * map.transform.rotation * Quaternion.AngleAxis(pitch, Vector3.right)));
            }
            else
            {
                yaw = Vector3.SignedAngle(Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.up), Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.up), camBase.transform.up);
                pitch = Vector3.SignedAngle(Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.right), Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.right), camBase.transform.right);

                camBase.transform.rotation = Quaternion.Lerp(camBase.transform.rotation, transform.rotation, resetSpeed * Time.deltaTime);
            }
        }
        camMx = 0;
        camMy = 0;
    }

    void ZoomCameraWithCollision()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0)
        {
            camCamDistance += 0.5f;
        }
        if (scroll > 0)
        {
            camCamDistance -= 0.5f;
        }
        camCamDistanceT += camCamDistance;
        unabstructedZoom += camCamDistance;

        camCamDistanceT = unabstructedZoom;
        if (Physics.Raycast(camBase.transform.position, (cam.transform.position - camBase.transform.position).normalized, out hit2, camMaxDistance))
        {
            float dist1 = Vector3.Distance(hit2.point, camBase.transform.position);

            camCamDistanceT = Mathf.Clamp(camCamDistanceT, 0, dist1);
        }
        else
        {
            camCamDistanceT = Mathf.Clamp(camCamDistanceT, 0, camMaxDistance);
        }
        Vector3 dir = camBase.transform.position - camBase.transform.forward * camCamDistanceT; // + camBase.transform.up * camCamDistanceT / 4;
        cam.transform.position = Vector3.Lerp(cam.transform.position, dir, responsiveness * Time.deltaTime);
        cam.transform.rotation = camBase.transform.rotation;
        camCamDistance = 0;

        unabstructedZoom = Mathf.Clamp(unabstructedZoom, 0, camMaxDistance);
    }
}
