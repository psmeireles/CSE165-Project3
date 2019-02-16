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
    float startTime;

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

        playerRig.transform.position = checkpoints[0].transform.position;
        playerRig.transform.LookAt(checkpoints[1].transform.position);

        nextCheckpoint = 0;
        waypointLine = this.GetComponent<LineRenderer>();
        movementEnabled = false;
        startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 nextCPCenter = checkpoints[nextCheckpoint].transform.position;
        // Checking next checkpoint
        if (Vector3.Distance(playerRig.transform.position, nextCPCenter) < radius) {
            checkpoints[nextCheckpoint].GetComponent<Renderer>().material.color = Color.green;
            nextCheckpoint++;
        }
        
        if(nextCheckpoint < checkpoints.Count - 1) {
            nextCPCenter = checkpoints[nextCheckpoint].transform.position;
            waypointLine.SetPosition(0, playerRig.transform.position);
            waypointLine.SetPosition(1, nextCPCenter);
            distanceText.GetComponent<Text>().text = "Distance: " + Vector3.Distance(playerRig.transform.position, nextCPCenter).ToString("0.00");
        }
        else {
            distanceText.GetComponent<Text>().text = "Finished!";
        }


        if (Time.time - startTime < 5) {
            countdown.text = Mathf.Floor(5 - (Time.time - startTime)).ToString();
            return;
        }
        else {
            movementEnabled = true;
            if(nextCheckpoint < checkpoints.Count - 1) {
                countdown.text = "Elapsed Time:\t" + (Time.time - startTime - 5).ToString("0.00");
            }
            else {
                movementEnabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Change the cube color to green.
        playerRig.transform.position = checkpoints[nextCheckpoint - 1].transform.position;
        movementEnabled = false;
        Debug.Log("Trigger");
    }

    private void OnCollisionEnter(Collider other) {
        // Change the cube color to green.
        playerRig.transform.position = checkpoints[nextCheckpoint - 1].transform.position;
        movementEnabled = false;
        Debug.Log("Collision");
    }
}
