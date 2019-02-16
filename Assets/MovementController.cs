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

    LeapProvider provider;
    float v_magnitude;
    float max_speed;
    float speed_increment;
    Vector3 v_dir;

    // Start is called before the first frame update
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();

        
        speed_increment = 0.001f;
        max_speed = 3.0f;
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

        if(!TrackController.movementEnabled) {
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
            foreach (Finger f in leftHand.Fingers)
            {
                if (f.IsExtended)
                {
                    speedLevel++;
                }
            }

            if (speedLevel > 0) // add threshold per finger extended
            {
                Debug.Log("left index finger extended");
                //playerRig.transform.Translate()
                if (v_magnitude < max_speed * speedLevel / 5.0f)
                {
                    v_magnitude += speed_increment * speedLevel;
                }
                else if(v_magnitude > max_speed * speedLevel / 5.0f)
                {
                    v_magnitude -= speed_increment * 2.0f;
                }
            }
            else // Slow user down if cannot detect LHand or L index finger not extended
            {
                if (v_magnitude > 0)
                {
                    v_magnitude -= speed_increment * 2.0f;
                }
            }
        }



        if (rightHand != null && rightHand.Fingers[1].IsExtended)
        {
            Debug.Log("right index finger extended");
            //Debug.Log(rightHand.Rotation.angle);
            //rightHand.Rotation = (LeapQuaternion)(new Quaternion(0.0f, 0.0f, 0.0f, 1.0f));
            Quaternion temp = new Quaternion(rightHand.Rotation.x, rightHand.Rotation.y, rightHand.Rotation.z, rightHand.Rotation.w);
            //Debug.Log(temp.eulerAngles);

        }
        if(rightHand != null)
        {
            Vector dir = rightHand.Direction;
            v_dir = rightHandObject.transform.localToWorldMatrix * new Vector4(dir.x, dir.y, dir.z, 0.0f);
            Debug.Log(v_dir);

            
        }
        // Move player forward
        playerRig.transform.Translate(v_magnitude * v_dir, Space.World);
    }
}
