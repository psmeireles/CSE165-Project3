using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrackController : MonoBehaviour
{
    public TextAsset trackFile;
    public GameObject checkPoint;
    public GameObject campus;

    List<GameObject> checkpoints;
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
            Vector3 translation = new Vector3(x, y, z);
            obj.transform.Translate(translation + campus.transform.position);
            checkpoints.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
