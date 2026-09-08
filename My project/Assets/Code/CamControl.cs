using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameControl : MonoBehaviour
{
    public GameObject camBase;
    public Camera cam;
    float modifier;
    public float camCamDistance;
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
    float x;
    float y;
    public bool isForPlane;
    public GameObject head;
    public GameObject map;
    Vector3 camPos;
    bool freeCam;
    public float freeCamSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unabstructedZoom = camCamDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetKeyDown(KeyCode.B) && freeCam == false)
        {
            freeCam = true;
            camPos = head.transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.B) && freeCam == true)
        {
            freeCam = false;
        }

        x = Input.GetAxis("Mouse X") * mouseSensitivity;
        y = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += x;
        pitch -= y;

        if (freeCam) FreeCam();
        else NormalCamera();
        ZoomCameraWithCollision();

        x = y = 0;
    }

    void FreeCam()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            camPos -= camBase.transform.forward * freeCamSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            camPos += camBase.transform.forward * freeCamSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            camPos += camBase.transform.right * freeCamSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            camPos -= camBase.transform.right * freeCamSpeed;
        }
        camBase.transform.position = camPos;
        RotateCamera();
    }

    void NormalCamera()
    {
        if (isForPlane) camBase.transform.position = transform.position;
        camBase.transform.position = head.transform.position;

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
                RotateCamera();
            }
            else
            {
                yaw = Vector3.SignedAngle(Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.up), Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.up), camBase.transform.up);
                pitch = Vector3.SignedAngle(Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.right), Vector3.ProjectOnPlane(camBase.transform.forward, camBase.transform.right), camBase.transform.right);

                camBase.transform.rotation = Quaternion.Lerp(camBase.transform.rotation, transform.rotation, resetSpeed * Time.deltaTime);
            }
        }
    }

    void RotateCamera()
    {
        pitch = Mathf.Clamp(pitch, -75, 75);
        camBase.transform.rotation = Quaternion.Lerp(camBase.transform.rotation,
            Quaternion.AngleAxis(yaw, transform.up) * map.transform.rotation * Quaternion.AngleAxis(pitch, Vector3.right),
            resetSpeed * Time.deltaTime * Quaternion.Angle(camBase.transform.rotation,
            Quaternion.AngleAxis(yaw, transform.up) * map.transform.rotation * Quaternion.AngleAxis(pitch, Vector3.right)));
    }

    void ZoomCameraWithCollision()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0)
        {
            modifier += 0.5f;
        }
        if (scroll > 0)
        {
            modifier -= 0.5f;
        }
        camCamDistance += modifier;
        unabstructedZoom += modifier;

        camCamDistance = unabstructedZoom;
        if (Physics.Raycast(camBase.transform.position, (cam.transform.position - camBase.transform.position).normalized, out hit2, camMaxDistance))
        {
            float dist1 = Vector3.Distance(hit2.point, camBase.transform.position);

            camCamDistance = Mathf.Clamp(camCamDistance, 0, dist1);
        }
        else
        {
            camCamDistance = Mathf.Clamp(camCamDistance, 0, camMaxDistance);
        }
        Vector3 dir = camBase.transform.position - camBase.transform.forward * camCamDistance; // + camBase.transform.up * camCamDistanceT / 4;
        cam.transform.position = Vector3.Lerp(cam.transform.position, dir, responsiveness * Time.deltaTime);
        cam.transform.rotation = camBase.transform.rotation;
        modifier = 0;

        unabstructedZoom = Mathf.Clamp(unabstructedZoom, 0, camMaxDistance);
    }
}
