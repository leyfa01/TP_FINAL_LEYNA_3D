using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GererPointage : MonoBehaviour
{

    public static string[] lstNoms = new string[6] { "", "", "", "", "", "" };
    public static int[] lstScores = new int[6];
    void Start()
    {
        if(PlayerPrefs.HasKey("LabyrintheDeniNoms")) {
            lstNoms = PlayerPrefsX.GetStringArray("LabyrintheDeniNoms");
            lstScores = PlayerPrefsX.GetIntArray("LabyrintheDeniPoints");
        } else {
            print("sav");
            Sauvegarde();
        }
    }

    public static void Sauvegarde() {
        PlayerPrefsX.SetStringArray("LabyrintheDeniNoms", lstNoms);
        PlayerPrefsX.SetIntArray("LabyrintheDeniPoints", lstScores);
    }

    public static void EnregistrementPointageListes() {
        int LePointage = GererJeu.pointsFinaux;
        string LeNom = GestionTexteInput.nomJoueur;

        lstScores[5] = LePointage;
        lstNoms[5] = LeNom;

        System.Array.Sort(lstScores, lstNoms);

        System.Array.Reverse(lstScores);
        System.Array.Reverse(lstNoms);

        Sauvegarde();
    }

    public static bool VerificationScore() {
        int LePointage = GererJeu.pointsFinaux;

        if (LePointage > lstScores[4]) {
            return true;
        }
        return false;
    }
}
