using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GererJeu : MonoBehaviour
{

    // Nombre de points finaux
    public static int pointsFinaux;
    // Nbr d'ennemis que le joueur a tué
    public static float nbrEnnemisTuer = 0f;
    // La durée d'une partie
    public float tempsTotal = 600f;
    // Gestion de la fin de partie
    public bool finPartie = false;
    // Quantité de point de vie
    public int PV = 150;
    // La valeur de base du labyrinthe en point
    float valeurMap = 500f;
    // Vérifie si le calcul du pointage a déjà été fait    
    bool pointageFait;
    // Deux éléments du UI de type slider pour le minuteur et la vie
    public Slider minuteurCanvas;
    public Slider barrePV;

    public AudioClip sonMort;
    public AudioClip sonVictoire;

    void Start()

    {
        // Remet le nbr d'ennemi à 0, et lance le minuteur
        nbrEnnemisTuer = 0f;
        InvokeRepeating("Minuteur", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Met à jour le slider selon le nombre de PV à chaque frame
        barrePV.value = PV;
        // Si les points de vie ou le temps atteint 0, la partie est finie
        if (PV <= 0f || tempsTotal <= 0f)
        {
            finPartie = true;
        }
        // Si la partie est finie, déclenche la mort si le temps ou les PV sont à 0, sinon, déclenche la victoire
        if (finPartie && !pointageFait)
        {
            // Met les points à 0, active l'animation de la mort, et charge la scène de défaite après 2 secondes
            if (tempsTotal <= 0)
            {
                pointsFinaux = 0;
                GetComponent<Animator>().SetBool("Mort", true);
                GetComponent<AudioSource>().PlayOneShot(sonMort);
                Invoke("chargerDefaiteTemps", 2f);
            }
            // Met les points à 0, active l'animation de la mort, et charge la scène de défaite après 2 secondes
            else if (PV <= 0)
            {
                pointsFinaux = 0;
                GetComponent<Animator>().SetBool("Mort", true);
                GetComponent<AudioSource>().PlayOneShot(sonMort);
                Invoke("chargerDefaitePV", 2f);
            }
            // Calcul des points totaux en se basant sur le temps restant, le nombre d'ennemis tués ainsi que la valeur de la carte
            // Active l'animation de victoire pour ensuite charger la scène de victoire après 2 secondes
            else
            {
                pointsFinaux = Mathf.RoundToInt(valeurMap * tempsTotal * (1 + (nbrEnnemisTuer / 20)));
                GetComponent<Animator>().SetBool("Victoire", true);
                GetComponent<AudioSource>().PlayOneShot(sonVictoire);
                Invoke("chargerVictoire", 3.5f);
            }
            // Confirme que le pointage a été fait pour que ça ne recommence pas
            pointageFait = true;
        }

    }

    // Tant que la partie n'est pas fini enlève 1 seconde à chaque appel de la fonction
    void Minuteur()
    {
        if (!finPartie)
        {
            tempsTotal--;
            minuteurCanvas.value = tempsTotal;
        }
    }

    // Charge la scène en question
    void chargerDefaiteTemps()
    {
        SceneManager.LoadScene(7);
    }
    void chargerDefaitePV()
    {
        SceneManager.LoadScene(8);
    }
    void chargerVictoire() {
        SceneManager.LoadScene(9);
    }

}
