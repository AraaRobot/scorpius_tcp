using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class ArduinoScript : MonoBehaviour
{
    // Vertical servomotors
    [SerializeField] private ServomotorScript servoVertA;
    [SerializeField] private ServomotorScript servoVertB;
    [SerializeField] private ServomotorScript servoVertC;
    [SerializeField] private ServomotorScript servoVertD;
    [SerializeField] private ServomotorScript servoVertE;
    [SerializeField] private ServomotorScript servoVertF;
    
    // Vertical servomotor control variables
    public bool EnableVerticalServos = false;
    public bool up = true;
    public float upAngle = 45;
    public float downAngle = -45f;

    // Horizontal servomotors
    [SerializeField] private ServomotorScript servoHorizA;
    [SerializeField] private ServomotorScript servoHorizB;
    [SerializeField] private ServomotorScript servoHorizC;
    [SerializeField] private ServomotorScript servoHorizD;
    [SerializeField] private ServomotorScript servoHorizE;
    [SerializeField] private ServomotorScript servoHorizF;

    // Horizontal servomotor control variables
    public bool EnableHorizontalServos = false;
    public bool forward = true;
    public float forwardAngle = 45f;
    public float backwardAngle = -45f;
    public float neutralAngle = 0f;

    // Position control variable
    public int position = 0;

    // Flip control variable
    public GameObject HexapodCopy;
    public float jumpForce = 5f;
    public float flipTorque = 10f;
    public float offsetMultiplier = 1f;
    private float timeSinceLastFlip = 0f;
    private float flipCooldown = 2.4f;
    private bool isFlipping = false;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Public method to set servo angles from external scripts
    public void SetServoAngles(float[] angles)
    {
        if (angles.Length != 12)
        {
            Debug.LogError("Invalid number of angles. Expected 12, got " + angles.Length);
            return;
        }

        // Set vertical servo angles
        servoVertA.targetAngle = Mathf.Clamp(angles[0], -45f, 65f);
        servoVertB.targetAngle = Mathf.Clamp(angles[1], -45f, 65f);
        servoVertC.targetAngle = Mathf.Clamp(angles[2], -45f, 65f);
        servoVertD.targetAngle = Mathf.Clamp(angles[3], -45f, 65f);
        servoVertE.targetAngle = Mathf.Clamp(angles[4], -45f, 65f);
        servoVertF.targetAngle = Mathf.Clamp(angles[5], -45f, 65f);

        // Set horizontal servo angles
        servoHorizA.targetAngle = Mathf.Clamp(angles[6]+30, -65f, 65f);
        servoHorizB.targetAngle = Mathf.Clamp(angles[7], -65f, 65f);
        servoHorizC.targetAngle = Mathf.Clamp(angles[8]-30, -65f, 65f);
        servoHorizD.targetAngle = Mathf.Clamp(-angles[9]-30, -65f, 65f);
        servoHorizE.targetAngle = Mathf.Clamp(-angles[10], -65f, 65f);
        servoHorizF.targetAngle = Mathf.Clamp(-angles[11]+30, -65f, 65f);
    }

    public void DoAFlip(Vector2 flip_direction)
    {
        if (Time.time - timeSinceLastFlip >= flipCooldown)
        {
            // Turn off rendering to prevent visual glitches during the flip
            Renderer[] renderers = transform.parent.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            // Turn on the copy of the hexapod to show the flip animation
            HexapodCopy.SetActive(true);
            Rigidbody rb_copy = HexapodCopy.GetComponent<Rigidbody>();
            rb_copy.transform.position = transform.position;
            rb_copy.transform.rotation = transform.rotation;
            // Calculate the offset based on the flip direction and apply it to the jump force
            Vector3 offset = new Vector3(flip_direction.normalized[0], 0, flip_direction.normalized[1]) * offsetMultiplier;
            // Apply an upward force to the center of mass
            rb_copy.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Apply a torque to induce the flip
            rb_copy.AddTorque(-Vector3.Cross(rb.transform.up, Vector3.RotateTowards(offset, rb.transform.forward, (Vector3.Angle(rb.transform.forward, offset)-Vector3.Angle(Vector3.forward, offset)) * Mathf.Deg2Rad, 0)).normalized * (flipTorque + 0.5f * Vector3.Angle(offset, Vector3.right) / 90f), ForceMode.Impulse);
            timeSinceLastFlip = Time.time;
            isFlipping = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeSinceLastFlip >= flipCooldown && isFlipping)
        {
            // Turn off rendering to prevent visual glitches during the flip
            Renderer[] renderers = transform.parent.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
            // Turn off the copy of the hexapod after the flip animation is done
            // rb.transform.position = HexapodCopy.GetComponent<Rigidbody>().transform.position;
            // rb.transform.rotation = HexapodCopy.GetComponent<Rigidbody>().transform.rotation;
            HexapodCopy.SetActive(false);
            isFlipping = false;

        }
    //     // Position inputs
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         position = 1;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         position = 2;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha3))
    //     {
    //         position = 3;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha4))
    //     {
    //         position = 4;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha0))
    //     {
    //         position = 0;
    //     }

    //     // Position
    //     float ua = Mathf.Clamp(-upAngle + 90f, -45f, 65f);
    //     float da = Mathf.Clamp(-downAngle + 90f, -45f, 65f);
    //     float na = Mathf.Clamp(-neutralAngle + 90f, -45f, 65f);
    //     switch (position)
    //     {
    //         case 1:
    //             servoHorizA.targetAngle = forwardAngle;
    //             servoHorizC.targetAngle = forwardAngle;
    //             servoHorizE.targetAngle = forwardAngle;
    //             servoHorizB.targetAngle = backwardAngle;
    //             servoHorizD.targetAngle = backwardAngle;
    //             servoHorizF.targetAngle = backwardAngle;
    //             servoVertA.targetAngle = da;
    //             servoVertB.targetAngle = da;
    //             servoVertC.targetAngle = da;
    //             servoVertD.targetAngle = da;
    //             servoVertE.targetAngle = da;
    //             servoVertF.targetAngle = da;
    //             break;
    //         case 2:
    //             servoHorizA.targetAngle = neutralAngle;
    //             servoHorizC.targetAngle = neutralAngle;
    //             servoHorizE.targetAngle = neutralAngle;
    //             servoHorizB.targetAngle = neutralAngle;
    //             servoHorizD.targetAngle = neutralAngle;
    //             servoHorizF.targetAngle = neutralAngle;
    //             servoVertA.targetAngle = ua;
    //             servoVertB.targetAngle = da;
    //             servoVertC.targetAngle = ua;
    //             servoVertD.targetAngle = da;
    //             servoVertE.targetAngle = ua;
    //             servoVertF.targetAngle = da;
    //             break;
    //         case 3:
    //             servoHorizA.targetAngle = backwardAngle;
    //             servoHorizC.targetAngle = backwardAngle;
    //             servoHorizE.targetAngle = backwardAngle;
    //             servoHorizB.targetAngle = forwardAngle;
    //             servoHorizD.targetAngle = forwardAngle;
    //             servoHorizF.targetAngle = forwardAngle;
    //             servoVertA.targetAngle = da;
    //             servoVertB.targetAngle = da;
    //             servoVertC.targetAngle = da;
    //             servoVertD.targetAngle = da;
    //             servoVertE.targetAngle = da;
    //             servoVertF.targetAngle = da;
    //             break;
    //         case 4:
    //             servoHorizA.targetAngle = neutralAngle;
    //             servoHorizC.targetAngle = neutralAngle;
    //             servoHorizE.targetAngle = neutralAngle;
    //             servoHorizB.targetAngle = neutralAngle;
    //             servoHorizD.targetAngle = neutralAngle;
    //             servoHorizF.targetAngle = neutralAngle;
    //             servoVertA.targetAngle = da;
    //             servoVertB.targetAngle = ua;
    //             servoVertC.targetAngle = da;
    //             servoVertD.targetAngle = ua;
    //             servoVertE.targetAngle = da;
    //             servoVertF.targetAngle = ua;
    //             break;
    //         case 0:
    //             servoHorizA.targetAngle = neutralAngle;
    //             servoHorizC.targetAngle = neutralAngle;
    //             servoHorizE.targetAngle = neutralAngle;
    //             servoHorizB.targetAngle = neutralAngle;
    //             servoHorizD.targetAngle = neutralAngle;
    //             servoHorizF.targetAngle = neutralAngle;
    //             servoVertA.targetAngle = neutralAngle;
    //             servoVertB.targetAngle = neutralAngle;
    //             servoVertC.targetAngle = neutralAngle;
    //             servoVertD.targetAngle = neutralAngle;
    //             servoVertE.targetAngle = neutralAngle;
    //             servoVertF.targetAngle = neutralAngle;
    //             break;
    //         default:
    //             break;
    //     }

        // Vertical servomotors
        //if (EnableVerticalServos)
        //{
        //    if (up)
        //    {
        //        float angle = Mathf.Clamp(-upAngle + 90f, -45f, 65f);
        //        servoVertA.targetAngle = angle;
        //        servoVertB.targetAngle = angle;
        //        servoVertC.targetAngle = angle;
        //        servoVertD.targetAngle = angle;
        //        servoVertE.targetAngle = angle;
        //        servoVertF.targetAngle = angle;
        //    }
        //    else
        //    {
        //        float angle = Mathf.Clamp(-downAngle + 90f, -45f, 65f);
        //        servoVertA.targetAngle = angle;
        //        servoVertB.targetAngle = angle;
        //        servoVertC.targetAngle = angle;
        //        servoVertD.targetAngle = angle;
        //        servoVertE.targetAngle = angle;
        //        servoVertF.targetAngle = angle;
        //    }
        //}
        //else
        //{
        //    servoVertA.targetAngle = 0f;
        //    servoVertB.targetAngle = 0f;
        //    servoVertC.targetAngle = 0f;
        //    servoVertD.targetAngle = 0f;
        //    servoVertE.targetAngle = 0f;
        //    servoVertF.targetAngle = 0f;
        //}

        //// Horizontal servomotors
        //if (EnableHorizontalServos)
        //{
        //    if (forward)
        //    {
        //        servoHorizA.targetAngle = forwardAngle;
        //        servoHorizB.targetAngle = forwardAngle;
        //        servoHorizC.targetAngle = forwardAngle;
        //        servoHorizD.targetAngle = forwardAngle;
        //        servoHorizE.targetAngle = forwardAngle;
        //        servoHorizF.targetAngle = forwardAngle;
        //    }
        //    else
        //    {
        //        servoHorizA.targetAngle = backwardAngle;
        //        servoHorizB.targetAngle = backwardAngle;
        //        servoHorizC.targetAngle = backwardAngle;
        //        servoHorizD.targetAngle = backwardAngle;
        //        servoHorizE.targetAngle = backwardAngle;
        //        servoHorizF.targetAngle = backwardAngle;
        //    }
        //}
        //else
        //{
        //    servoHorizA.targetAngle = 0f;
        //    servoHorizB.targetAngle = 0f;
        //    servoHorizC.targetAngle = 0f;
        //    servoHorizD.targetAngle = 0f;
        //    servoHorizE.targetAngle = 0f;
        //    servoHorizF.targetAngle = 0f;
        //}
    }
}
