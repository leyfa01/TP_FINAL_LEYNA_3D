using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionnaireCamera : MonoBehaviour

{

    // Déclare un tableau de GameObject (Camera dans ce cas)
    public GameObject[] lesCams;


    // Start is called before the first frame update
    void Start()
    {
        ActiveCam(0);

    }

    // Update is called once per frame
    void Update()
    {
        // Active la cam 0
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveCam(0);
        }
        // Active la cam 1
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiveCam(1);
        }
    }
    // Désactive toutes les cam puis active celle choisie
    public void ActiveCam(int indexCamActive)
    {
        foreach (GameObject cam in lesCams)
        {
            cam.SetActive(false);
        }

        lesCams[indexCamActive].SetActive(true);
    }
}
