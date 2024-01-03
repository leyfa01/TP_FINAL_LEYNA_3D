using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScriptEnnemi : MonoBehaviour
{
    // Start is called before the first frame update

    // Le personnage à suivre
    public GameObject PersonnageAkali;
    // Raccourci pour le GetComponent navAgent
    NavMeshAgent navAgent ;
    // Nombre de point de vie de l'ennemi
    int PV = 3;
    // Détermine si il est mort ou pas
    bool mort = false;
    public AudioClip sonPas;
    float dernierSonDePas;
    void Start()
    {
        // Initialise la variable
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Met le float d'animator selon la vélocité du navAgent
        GetComponent<Animator>().SetFloat("Vitesse", navAgent.velocity.magnitude);

        if (GetComponent<Animator>().GetFloat("Vitesse") > 0.1f && Time.time - dernierSonDePas > 0.23f) {
            GetComponent<AudioSource>().PlayOneShot(sonPas);
            dernierSonDePas = Time.time;
        } 

        // Si les PV sont à 0 et qu'il n'est toujour pas mort
        if (PV <= 0 && !mort) {
            // Active l'animation de mort
            GetComponent<Animator>().SetBool("Mort", true);
            // Met la variable mort à true pour éviter de relancer cette fonction
            mort = true;
            // Désactive le navAgente
            navAgent.enabled = false;
            // Désactive tous ses capsuleCollider
            foreach (CapsuleCollider collider in GetComponents<CapsuleCollider>()) {
                collider.enabled = false;
            }
            // Augmente le nbr d'ennemi tué par le joueur
            GererJeu.nbrEnnemisTuer++;
        } 
        // Si ses PV sont supérieur à 0, et que le navAgente est activé, il suit continuellement le joueur
        else if (PV > 0 && navAgent.enabled) 
        {
            navAgent.SetDestination(PersonnageAkali.transform.position);
        }
    }

    // Quand l'ennemi touche une balle, il perd 1 PV
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Balle") {
            PV--;
        }
    }
    // Si le perso rentre dans le trigger de l'ennemi, le navAgent est désactivé, et il commence son animation de frappe
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Perso") {
            GetComponent<Animator>().SetBool("Frappe", true);
            navAgent.enabled = false;
        }
    }
    // Pendant que le perso est dans le trigger de l'ennemi, il perd continuellement des PV
    void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "Perso") {
            PersonnageAkali.GetComponent<GererJeu>().PV--;
        }
    }
    // Si le perso sort du trigger de l'ennemi, le navAgent est activé, et il arrête son animation de frappe
    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag == "Perso") {
            GetComponent<Animator>().SetBool("Frappe", false);
            navAgent.enabled = true;
        }
    }
}
