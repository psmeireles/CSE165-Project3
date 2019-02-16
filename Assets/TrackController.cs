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
    public Text countdown;

    public List<GameObject> checkpoints;

    public int nextCheckpoint;
    float radius = 9.144f;
    LineRenderer waypointLine;
    LineRenderer trackLine;
    float startTime;
    float collideTime;

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

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 nextCPCenter = checkpoints[checkpoints.Count-1].transform.position;
        
        // Only assign new center if it is not the last one in the list
        if(nextCheckpoint < checkpoints.Count)
        {
            nextCPCenter = checkpoints[nextCheckpoint].transform.position;
        }
        // Checking next checkpoint
        if (nextCheckpoint < checkpoints.Count && Vector3.Distance(playerRig.transform.position, nextCPCenter) < radius) {
            checkpoints[nextCheckpoint].GetComponent<Renderer>().material.color = new Color(0,1,0,0.25f);
            nextCheckpoint++;
        }
        
        if(nextCheckpoint < checkpoints.Count) {
            nextCPCenter = checkpoints[nextCheckpoint].transform.position;
            waypointLine.SetPosition(0, playerRig.transform.position + new Vector3(0, -0.5f, 0.0f));
            waypointLine.SetPosition(1, nextCPCenter);
            distanceText.GetComponent<Text>().text = "Distance: " + Vector3.Distance(playerRig.transform.position, nextCPCenter).ToString("0.00") + "m";
        }
        else {
            distanceText.GetComponent<Text>().text = "Finished!";
        }

        if (PlayerColliderController.hasCollided)
        {
            collideTime = Time.time;
            playerRig.transform.position = checkpoints[nextCheckpoint - 1].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
            movementEnabled = false;
            PlayerColliderController.hasCollided = false;
        }

        if (Time.time - startTime < 5) {
            countdown.text = Mathf.Ceil(5 - (Time.time - startTime)).ToString();
            return;
        }
        else {

            // Check collision delay time
            if(Time.time - collideTime < 3)
            {
                countdown.text = Mathf.Ceil(3 - (Time.time - collideTime)).ToString();
                return;
            }
            else
            {
                movementEnabled = true;
                PlayerColliderController.hasCollided = false;
            }
            

            // Update stopwatch time as long as there are remaining checkpoints
            if(nextCheckpoint < checkpoints.Count) {
                countdown.text = "Elapsed Time:\t" + (Time.time - startTime - 5).ToString("0.00");
            }
            else { //Reached last checkpoint, do not update time anymore
                movementEnabled = false;
                waypointLine.enabled = false; // hide waypoint line
            }
        }

        
    }

}
