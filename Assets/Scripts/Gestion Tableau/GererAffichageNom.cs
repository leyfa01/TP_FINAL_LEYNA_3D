using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GererAffichageNom : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI nomText;

    void Start()
    {
        nomText.text = GestionTexteInput.nomJoueur;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
