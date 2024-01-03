using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GererTir : MonoBehaviour
{
    // Déclare le prefab à cloner de la balle
    public GameObject ballePrefab;
    // Déclare un point d'apparition ou faire apparaître la balle
    public Transform ballePointApparition;
    // Détermine si le personnage a le droit de tirer ou pas
    public bool peutTirer = true;

    public AudioClip sonTir;
    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<GererJeu>().finPartie && GetComponent<ItemPickUp>().fusil.activeInHierarchy)
        {
            if (peutTirer)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    TirBalle();
                }
            }

        }

    }

    // À chaque appelle de cette fonction, clone un prefab, met la position et la rotation selon le point d'apparition
    // Active l'objet et lui donne une vélocité en avant (Z)
    void TirBalle()
    {

        GameObject balleClone = Instantiate(ballePrefab);
        balleClone.transform.position = ballePointApparition.transform.position;
        balleClone.transform.rotation = ballePointApparition.transform.rotation;
        balleClone.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(sonTir);
        balleClone.GetComponent<Rigidbody>().velocity = transform.forward * 20f;
    }
}
