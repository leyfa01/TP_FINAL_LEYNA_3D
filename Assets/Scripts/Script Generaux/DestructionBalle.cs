using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionBalle : MonoBehaviour
{
    // Start is called before the first frame update

    // Enregistre les particule d'explosion de la balle
    public GameObject explosionBalleParticules;

    void Start()
    {
        // Détruit la balle après 1.5 secondes
        Invoke("DestroyBullet", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        // Quand y'a une collision entre la balle et un collider, clone les particule, les met à la position de la balle
        // Active les particules et détruit la balle
        GameObject explosionBalle = Instantiate(explosionBalleParticules);
        explosionBalle.transform.position = gameObject.transform.position;
        explosionBalle.SetActive(true);
        Destroy(gameObject);
    }

    void DestroyBullet() {
        // Quand la fonction est appelée clone les particule, les met à la position de la balle
        // Active les particules et détruit la balle
        GameObject explosionBalle = Instantiate(explosionBalleParticules);
        explosionBalle.transform.position = gameObject.transform.position;
        explosionBalle.SetActive(true);
        Destroy(gameObject);
    }
}
