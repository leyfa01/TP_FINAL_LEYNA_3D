using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestionEnregistrement : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI nomText;
    void Start()
    {
        nomText.text = GestionTexteInput.nomJoueur;

        if (GererPointage.VerificationScore()) {
            GererPointage.EnregistrementPointageListes();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
