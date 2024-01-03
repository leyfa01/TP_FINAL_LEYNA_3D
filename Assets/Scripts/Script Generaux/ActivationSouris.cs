using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationSouris : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
        // Rend le curseur visible et retire le LockState afin de pouvoir le bouger
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
