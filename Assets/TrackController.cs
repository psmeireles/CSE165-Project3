using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TrackController : MonoBehaviour
{
    public TextAsset trackFile;
    public GameObject checkPoint;
    public GameObject campus;
    public GameObject playerRig;
    public Transform distanceText;
    public Text countdownText;
    public Text stopwatchText;
    public AudioClip applause;
    public AudioClip countdownClip;
    public GameObject cameraIndicator_Horiz;
    public GameObject cameraIndicator_Vert;
    public GameObject mainCamera;
    public GameObject airplane;
    public List<GameObject> checkpoints;

    public int nextCheckpoint;
    float radius = 9.144f;
    LineRenderer waypointLine;
    LineRenderer trackLine;
    float startTime;
    float collideTime;
    bool hasFinished;

    public static bool movementEnabled;
    // Start is called before the first frame update
    void Start()
    {
        checkpoints = new List<GameObject>();
        StreamReader reader = File.OpenText("Assets/" + trackFile.name + ".txt");
        string line;
        while ((line = reader.ReadLine()) != null) {
            string[] items;
            if(line.Contains("\t")){
                items = line.Split('\t');
            }
            else {
                items = line.Split(' ');
            }
            float x = float.Parse(items[0]);
            float y = float.Parse(items[1]);
            float z = float.Parse(items[2]);

            GameObject obj = GameObject.Instantiate(checkPoint);
            Vector3 translation = new Vector3(x, y, z) * 0.0254f;
            obj.transform.Translate(translation);
            Destroy(obj.GetComponent<Collider>());
            checkpoints.Add(obj);
        }

        playerRig.transform.position = checkpoints[0].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        playerRig.transform.LookAt(checkpoints[1].transform.position);
        airplane.transform.position = playerRig.transform.position;
        MovementController.v_dir = playerRig.transform.forward; // updates the initial velocity direction after turning the player

        nextCheckpoint = 0;
        waypointLine = playerRig.GetComponent<LineRenderer>();
        trackLine = this.GetComponent<LineRenderer>();
        trackLine.positionCount = checkpoints.Count;
        trackLine.material.color = Color.blue;

        for (int i = 0; i < checkpoints.Count; i++)
        {
            trackLine.SetPosition(i, checkpoints[i].transform.position);
        }
       

        movementEnabled = false;
        startTime = Time.time;
        collideTime = 0.0f;
        hasFinished = false;

        distanceText.GetComponent<Text>().text = "Distance: -";
        stopwatchText.text = "Elapsed Time: 0.00";
        cameraIndicator_Horiz.SetActive(false);
        cameraIndicator_Vert.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AudioSource audio = this.GetComponent<AudioSource>();

        if (PlayerColliderController.hasCollided)
        {
            collideTime = Time.time;
            if(MovementController.usePlane)
            {
                airplane.transform.position = checkpoints[nextCheckpoint - 1].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
                playerRig.transform.position = airplane.transform.position + airplane.transform.forward * 2.0f;
                playerRig.transform.LookAt(airplane.transform.position);
            }
            else
            {
                playerRig.transform.position = checkpoints[nextCheckpoint - 1].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
            }
            movementEnabled = false;
            PlayerColliderController.hasCollided = false;
        }

        if (Time.time - startTime < 5)
        {
            if (!audio.isPlaying) {
                audio.PlayOneShot(countdownClip, 20);
            }

            countdownText.gameObject.SetActive(true);
            countdownText.text = Mathf.Ceil(5 - (Time.time - startTime)).ToString();
            return;
        }
        else
        {
            // Check collision delay time
            if (Time.time - collideTime < 3)
            {
                if (!audio.isPlaying) {
                    audio.PlayOneShot(countdownClip, 20);
                }

                countdownText.gameObject.SetActive(true);
                countdownText.text = Mathf.Ceil(3 - (Time.time - collideTime)).ToString();
                //return;
            }
            else
            {
                countdownText.gameObject.SetActive(false);
                movementEnabled = true;
                PlayerColliderController.hasCollided = false;
            }
        }

        // Update data if there are remaining checkpoints
        if (nextCheckpoint < checkpoints.Count)
        {
            Vector3 nextCPCenter = checkpoints[nextCheckpoint].transform.position;

            nextCPCenter = checkpoints[nextCheckpoint].transform.position;
            waypointLine.SetPosition(0, playerRig.transform.position + new Vector3(0, -0.5f, 0.0f));
            waypointLine.SetPosition(1, nextCPCenter);
            distanceText.GetComponent<Text>().text = "Distance: " + Vector3.Distance(playerRig.transform.position, nextCPCenter).ToString("0.00") + "m";

            // Update stopwatch time
            stopwatchText.text = "Elapsed Time:\t" + (Time.time - startTime - 5).ToString("0.00");

            // Checking next checkpoint
            if (Vector3.Distance(playerRig.transform.position, nextCPCenter) < radius)
            {
                checkpoints[nextCheckpoint].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.25f);
                nextCheckpoint++;
                this.GetComponent<AudioSource>().Play();
            }

            Vector3 nextCheckpoint_cam = mainCamera.GetComponent<Camera>().WorldToViewportPoint(nextCPCenter);
            
            //Debug.Log("HERE:" + nextCheckpoint_cam);
            if((nextCheckpoint_cam.x < 0.3 && nextCheckpoint_cam.z > 0) || (nextCheckpoint_cam.x > 0.5 && nextCheckpoint_cam.z < 0))
            {
                cameraIndicator_Horiz.SetActive(true);
                cameraIndicator_Horiz.transform.localPosition = new Vector3(-100, 0, 0);
                cameraIndicator_Horiz.transform.localRotation = Quaternion.Euler( new Vector3(0, 0, 90));
                //Debug.Log("look left!");
            }
            else if ((nextCheckpoint_cam.x > 0.7 && nextCheckpoint_cam.z > 0) || (nextCheckpoint_cam.x < 0.5 && nextCheckpoint_cam.z < 0))
            {
                cameraIndicator_Horiz.SetActive(true);
                cameraIndicator_Horiz.transform.localPosition = new Vector3(100, 0, 0);
                cameraIndicator_Horiz.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                //Debug.Log("look right!");
            }
            else
            {
                //Debug.Log("disable arrow");
                cameraIndicator_Horiz.SetActive(false);
            }

            if ((nextCheckpoint_cam.y < 0.3 && nextCheckpoint_cam.z > 0) || (nextCheckpoint_cam.y < 0.3 && nextCheckpoint_cam.z < 0))
            {
                cameraIndicator_Vert.SetActive(true);
                cameraIndicator_Vert.transform.localPosition = new Vector3(0, -100, 0);
                cameraIndicator_Vert.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                //Debug.Log("look down!");
            }
            else if ((nextCheckpoint_cam.y > 0.7 && nextCheckpoint_cam.z > 0) || (nextCheckpoint_cam.y > 0.7 && nextCheckpoint_cam.z < 0))
            {
                cameraIndicator_Vert.SetActive(true);
                cameraIndicator_Vert.transform.localPosition = new Vector3(0, 100, 0);
                cameraIndicator_Vert.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                //Debug.Log("look up!");
            }
            else
            {
                //Debug.Log("disable arrow");
                cameraIndicator_Vert.SetActive(false);
            }
        }
        else //Reached last checkpoint
        {
            if (!hasFinished) {
                audio.PlayOneShot(applause, 2);
                playerRig.GetComponent<AudioSource>().Pause();
            }
            movementEnabled = false;
            hasFinished = true;
            waypointLine.enabled = false; // hide waypoint line
            distanceText.GetComponent<Text>().text = "Finished!";
            cameraIndicator_Horiz.SetActive(false);
            cameraIndicator_Vert.SetActive(false);
        }


        //Debug.Log("mainCam Angle:"+mainCamera.transform.rotation.eulerAngles);
        
    }

}
