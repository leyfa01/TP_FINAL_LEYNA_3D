using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CliqueBouton : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioClip sonClique;
    public int SceneACharger;
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Lance le son si il y'a lieu, ensuite enlève le son pour ne pas avoir de dédoublement de sib
    // Ensuite charge la scène peu de temps après
    public void chargerScene() {
        if (sonClique != null) {
            GetComponent<AudioSource>().PlayOneShot(sonClique);
        }
        sonClique = null;
        Invoke("lancementScence", 0.3f);
    }

    void lancementScence() {
        SceneManager.LoadScene(SceneACharger);
    }
}
