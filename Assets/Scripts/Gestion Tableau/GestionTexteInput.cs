using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GestionTexteInput : MonoBehaviour
{

    public AudioClip sonClique;
    public TextMeshProUGUI txtInput;
    public TextMeshProUGUI message;

    public static string nomJoueur;

    public void ValidationNom() {
        string leNom = txtInput.text;

        GetComponent<AudioSource>().PlayOneShot(sonClique);

        if (leNom.Length <= 1f) {
            message.text = "Il faut entrer un nom !";
        } else {
            nomJoueur = leNom;
            message.text = "Chargement en cours..";
            Invoke("ChargementJeu", 2f);
        }
    }

    void ChargementJeu() {
        SceneManager.LoadScene("Map 1");
    }
}
