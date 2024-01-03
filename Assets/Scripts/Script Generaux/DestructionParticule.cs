using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionParticule : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        // Quand le système de particule arrête de jouer, détruit la particule
        if (!GetComponent<ParticleSystem>().isPlaying) {
            Destroy(gameObject);
        }
    }
}
