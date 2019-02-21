using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{

    public static bool hasCollided;
    public AudioClip explosion;

    private AudioSource explosionSound;
    // Start is called before the first frame update
    void Start()
    {
        hasCollided = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(explosion, 2);
    }
}
