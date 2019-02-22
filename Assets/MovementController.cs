using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public GameObject playerRig;
    public GameObject rightHandObject;
    public GameObject handModels;
    public GameObject airplane;
    public GameObject mainCamera;

    LeapProvider provider;
    float v_magnitude;
    float max_speed;
    float speed_increment;
    public static Vector3 v_dir;

    public static bool usePlane;

    // Start is called before the first frame update
    void Start()
    {
        usePlane = false;
        airplane.SetActive(false);
        airplane.transform.position = playerRig.transform.position;
        provider = FindObjectOfType<LeapProvider>();

        
        speed_increment = 0.002f;
        max_speed = 5.0f;
        v_magnitude = 0.0f;
        v_dir = playerRig.transform.rotation * Vector3.forward;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int speedLevel = 0;
        Frame frame = provider.CurrentFrame;
        Hand rightHand = null;
        Hand leftHand = null;

        if (usePlane)
        {
            airplane.SetActive(true);
            airplane.transform.LookAt(airplane.transform.position - v_dir); // subtract to do the 180 deg rotation req to face airplane forward
            //mainCamera.transform.Translate(-v_dir * 5.0f);
        }

        playerRig.GetComponent<AudioSource>().pitch = v_magnitude + 1;

        if (!TrackController.movementEnabled) {
            v_magnitude = 0.0f; // reset speed to 0 when not moving
            return;
        }


        if(frame.Hands.Capacity > 0)
        {
            foreach (Hand h in frame.Hands)
            {
                //Debug.Log(h);
                if (h.IsLeft)
                    leftHand = h;
                if (h.IsRight)
                    rightHand = h;
            }
        }

        if (leftHand != null)
        {
            // Increase speed level based on number of fingers extended to control max speed
            foreach (Finger f in leftHand.Fingers)
            {
                if (f.IsExtended)
                {
                    speedLevel++;
                }
            }

            if (speedLevel > 0) // add threshold per finger extended
            {
                //Debug.Log("left index finger extended");
                //playerRig.transform.Translate()
                if (v_magnitude < max_speed * speedLevel / 5.0f)
                {
                    v_magnitude += speed_increment * speedLevel * 2.0f;
                }
                else if(v_magnitude > max_speed * speedLevel / 5.0f)
                {
                    v_magnitude -= speed_increment * 10.0f;
                }
            }
            else // Slow user down if cannot detect LHand or L index finger not extended
            {
                if (v_magnitude > 0)
                {
                    v_magnitude -= speed_increment * 30.0f;
                }
                else
                {
                    v_magnitude = 0;
                }
            }
        }



        if (rightHand != null && rightHand.Fingers[1].IsExtended)
        {
            //Debug.Log("right index finger extended");
            //Debug.Log(rightHand.Rotation.angle);
            //rightHand.Rotation = (LeapQuaternion)(new Quaternion(0.0f, 0.0f, 0.0f, 1.0f));
            Quaternion temp = new Quaternion(rightHand.Rotation.x, rightHand.Rotation.y, rightHand.Rotation.z, rightHand.Rotation.w);
            //Debug.Log(temp.eulerAngles);

        }
        if(rightHand != null)
        {
            Vector dir = rightHand.Direction;
            v_dir = rightHandObject.transform.localToWorldMatrix * new Vector4(dir.x, dir.y, dir.z, 0.0f);

            Debug.Log("vdir:"+v_dir);
        }
        if(usePlane)
        {
            // Move player forward
            airplane.transform.Translate(v_magnitude * v_dir, Space.World);
            playerRig.transform.position = airplane.transform.position - v_dir * 2.0f;
            playerRig.transform.LookAt(airplane.transform.position);
        }
        else
        {
            // Move player forward
            playerRig.transform.Translate(v_magnitude * v_dir, Space.World);
        }

        
        
    }
}
