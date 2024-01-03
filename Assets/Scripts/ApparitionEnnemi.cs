using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApparitionEnnemi : MonoBehaviour
{
    // Start is called before the first frame update

    // Variable publique du temps d'attente avant d'invoquer la fonction
    public float tempsAttente;
    void Start()
    {   
        // Invoke une fonction qui fait appraître l'ennemi après un certain temps décidé selon la valeur de tempsAttente
        Invoke("ApparaitreEnnemi", tempsAttente);
        // Désactive ensuite l'ennemi
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    // Active l'ennemi
    void ApparaitreEnnemi() {
        gameObject.SetActive(true);
    }
}
