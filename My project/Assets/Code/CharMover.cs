using UnityEngine;

public class CharMover : MonoBehaviour
{
    public float legLength;
    public float cToLStart;
    public float movementSpeed;
    float tmSpeed;
    public float friction;
    public float jumpHeight;
    public float jumpTime;
    public float weight;
    public float resetSpeed;
    public float lookDirSpeed;
    public float headRestDist;
    RaycastHit hit;
    Rigidbody rb;
    float jumpTimer;
    public GameObject camBase;
    public GameObject head;
    bool isGrounded;
    bool leftLegExists, rightLegExists;
    Vector3 leftFootPos, rightFootPos;
    public GameObject leftLeg, rightLeg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        head.transform.rotation = camBase.transform.rotation;
        float rotY = Mathf.LerpAngle(transform.eulerAngles.y, head.transform.rotation.eulerAngles.y, lookDirSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        rb.maxAngularVelocity = 0;

        jumpTimer -= Time.deltaTime;
        Debug.DrawLine(transform.position - (transform.up * cToLStart), transform.position - (transform.up * (legLength + cToLStart)), Color.red);
        if (Physics.Raycast(transform.position - (transform.up * cToLStart), -transform.up, out hit, legLength, 1 << 3))
        {
            float dTg = 1 + hit.distance;
            rb.AddForce(transform.up * weight / dTg);
        }
        else if (rb.linearVelocity.y > 0 && jumpTimer <= 0)
        {
            float velocityY = Mathf.Lerp(rb.linearVelocity.y, 0, resetSpeed * Time.deltaTime);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, velocityY, rb.linearVelocity.z);
        }

        if (Physics.Raycast(transform.position - (transform.up * cToLStart), -transform.up, out hit, legLength + 1, 1 << 3))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight);
            jumpTimer = jumpTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            tmSpeed = movementSpeed;
        }
        else tmSpeed = movementSpeed / 2;

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * tmSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(transform.forward * -tmSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * tmSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(transform.right * -tmSpeed);
        }
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * friction, rb.linearVelocity.y, rb.linearVelocity.z * friction);

        Vector3 startPos = transform.position - (transform.up * cToLStart);
        if (!leftLegExists)
        {
            Vector3 speedOffset = new Vector3(rb.linearVelocity.x * -1, 0, rb.linearVelocity.z * -1);
            speedOffset = Vector3.ClampMagnitude(speedOffset, legLength / 2);
            Vector3 leftFootGoal = transform.position - (transform.up * (cToLStart + legLength + 1) + speedOffset);
            Vector3 goalDir = (leftFootGoal - (startPos - transform.right)).normalized;

            Debug.DrawLine(startPos, startPos + goalDir * legLength * 2, Color.cyan);

            if (Physics.Raycast(startPos, goalDir, out hit, legLength + 1, 1 << 3))
            {
                Vector3 centerPos = startPos + (goalDir * (hit.distance / 2));
                leftLeg.transform.position = centerPos;
                leftLeg.transform.localScale = new Vector3(.5f, hit.distance, .5f);
                leftLeg.transform.rotation = Quaternion.LookRotation(goalDir, transform.forward);
                leftLegExists = true;
            }
        }
        else if (Vector3.Distance(leftFootPos, startPos) > legLength + 1)
        {
            leftLegExists = false;
        }

        if (!rightLegExists)
        {
            Vector3 speedOffset = new Vector3(rb.linearVelocity.x * -1, 0, rb.linearVelocity.z * -1);
            speedOffset = Vector3.ClampMagnitude(speedOffset, legLength / 2);
            Vector3 leftFootGoal = transform.position - (transform.up * (cToLStart + legLength + 1) + speedOffset);
            Vector3 goalDir = (leftFootGoal - (startPos + transform.right)).normalized;

            Debug.DrawLine(startPos, startPos + goalDir * legLength * 2, Color.cyan);

            if (Physics.Raycast(startPos, goalDir, out hit, legLength + 1, 1 << 3))
            {
                Vector3 centerPos = startPos + (goalDir * (hit.distance / 2));
                rightLeg.transform.position = centerPos;
                rightLeg.transform.localScale = new Vector3(.5f, hit.distance, .5f);
                rightLeg.transform.rotation = Quaternion.LookRotation(goalDir, transform.forward);
                rightLegExists = true;
            }
        }
        else if (Vector3.Distance(rightFootPos, startPos) > legLength + 1)
        {
            rightLegExists = false;
        }

    }
    private void FixedUpdate()
    {
        head.transform.position = transform.position + transform.up * headRestDist;
    }
}
