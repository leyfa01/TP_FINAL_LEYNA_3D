using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spots : MonoBehaviour
{   
    // Cible à suivre (Le personnage dans ce cas)
    public GameObject cible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        // Position selon la position de la cible
        transform.position = cible.transform.position + new Vector3(0, 1.4f, 0);
    }
}
