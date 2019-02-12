using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrackController : MonoBehaviour
{
    public TextAsset trackFile;
    public GameObject checkPoint;

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
            int x = int.Parse(items[0]);
            int y = int.Parse(items[1]);
            int z = int.Parse(items[2]);

            GameObject obj = GameObject.Instantiate(checkPoint);
            obj.transform.Translate(x, y, z);
            checkpoints.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
