using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestionAffichageClassement : MonoBehaviour
{
    public TextMeshProUGUI txtNoms;
    public TextMeshProUGUI txtPointages;
    void Start()
    {
        txtNoms.text = "";
        txtPointages.text = "";

        string LesNoms = "";
        string LesScores = "";

        for (int posTableau = 0; posTableau < GererPointage.lstScores.Length-1; posTableau++) {
            if (GererPointage.lstScores[posTableau] == 0) {
                break;
            } else {
                LesNoms += GererPointage.lstNoms[posTableau] + "\n";
                LesScores += GererPointage.lstScores[posTableau] + "\n";
            }
        }

        txtNoms.text = LesNoms;
        txtPointages.text = LesScores;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
