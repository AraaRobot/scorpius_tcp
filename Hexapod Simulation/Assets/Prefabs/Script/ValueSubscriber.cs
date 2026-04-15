using RosMessageTypes.ScorpiusMain;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;


public class ValueSubscriber : MonoBehaviour
{
    ROSConnection ros;
    private float[] servoAngles = new float[12];

    [SerializeField] private ArduinoScript arduinoScript;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RosIPAddress = "127.0.0.1";
        ros.RosPort = 10000;
        ros.Subscribe<ServoAnglesMsg>("/scorpius/teleop", AnglesCallback);
        ros.Subscribe<JoyMsg>("/scorpius/joy", InputCallback);
    }

    void AnglesCallback(ServoAnglesMsg msg)
    {
        // Debug.Log("Angles message received.");
        servoAngles[0] = msg.vert_a + 90f;
        servoAngles[1] = msg.vert_b + 90f;
        servoAngles[2] = msg.vert_c + 90f;
        servoAngles[3] = msg.vert_d + 90f;
        servoAngles[4] = msg.vert_e + 90f;
        servoAngles[5] = msg.vert_f + 90f;
        servoAngles[6] = -msg.horiz_a;
        servoAngles[7] = -msg.horiz_b;
        servoAngles[8] = -msg.horiz_c;
        servoAngles[9] = -msg.horiz_d;
        servoAngles[10] = -msg.horiz_e;
        servoAngles[11] = -msg.horiz_f;
        // Debug.Log("Received servo angles: " + string.Join(", ", servoAngles));

        arduinoScript.SetServoAngles(servoAngles);
    }

    void InputCallback(JoyMsg msg)
    {
        // Debug.Log("Input message received.");
        Vector2 flip_direction = new Vector2(msg.joy_data[1], msg.joy_data[0]);
        if (msg.joy_data[14] == 1.0f)
        {
            arduinoScript.DoAFlip(flip_direction);
            // Debug.Log("Flip command received. Direction: " + flip_direction);
        }
    }

    public float[] GetReceivedValue()
    {
        return servoAngles;
    }
}
