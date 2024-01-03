using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AfficherPointage : MonoBehaviour
{
    // Start is called before the first frame update

    // Variable TMP UI de pointage
    public TextMeshProUGUI Pointage;
    // Variable float du temps qui est passé depuis le début de la fonction
    float pointageTempsPasser = 0f;
    // Pointage a montrer selon le calcul à chaque frame
    float pointageAMontrer = 0f;
    // Tant que c'est true, la fonction sera appelé
    bool affichagePoints = true;
    // Le temps total que ça doit prendre pour arriver au nombre de points finaux
    float pointageTempsTotal = 2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        // Tant que c'est true, appelle la fonction

        if (affichagePoints)
        {
            // Tant que le temps passé est moins que le temps total continue ici
            if (pointageTempsPasser < pointageTempsTotal)
            {   
                // Ajoute au temps passé, le temps qui est passé entre temps
                pointageTempsPasser += Time.deltaTime;
                // Le pointage a montrer est un pourcentage des points finaux, créé à partir du rapport entre le temps passé
                // et le temps total
                pointageAMontrer = Mathf.Lerp(0, GererJeu.pointsFinaux, pointageTempsPasser / pointageTempsTotal);
                // Met ensuite cette partie des points en texte
                Pointage.text = Mathf.RoundToInt(pointageAMontrer).ToString() + " Points";
            }
            else
            {
                // Une fois que le temps passé n'est plus inférieur au temps total, met le nombre final de point en texte, 
                // et met l'affichage de point à false afin d'éviter que cette fonction continue
                Pointage.text = Mathf.RoundToInt(GererJeu.pointsFinaux).ToString() + " Points";
                affichagePoints = false;
            }
        }
    }
}
