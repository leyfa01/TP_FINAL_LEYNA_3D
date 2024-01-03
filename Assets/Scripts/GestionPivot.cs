using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionPivot : MonoBehaviour
{

    // La cible dont on enregistre la position pour la suivre
    public GameObject cible;
    // La camera du Pivot
    public GameObject cameraPerso;
    // La hauteur du pivot
    float hautPivot = 1.6f;
    // La position du RayCast afin d'éviter les objets qui obstruent la vision
    public GameObject positionRayCastCamera;
    // Distance de la caméra si rien bloque
    public float distanceCameraLoin = -4f;
    // Distance de la caméra si quelque chose bloque
    public float distanceCameraPres = 0.5f;
    // Vitesse de la rotation de la caméra
    public float vitesseRotation = 2f;
    // Start is called before the first frame update
    void Start()
    {
        // Met la position du pivot à la position de la cible + la hauteur
        transform.position = cible.transform.position + new Vector3(0, hautPivot, 0);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Met la position du pivot à la tête du personnage
        transform.position = cible.transform.position + new Vector3(0, hautPivot, 0);
        // Quand il appuie sur Ctrl gauche, le curseur devient visible, et il peut être utilisé
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Rotate le pivot selon le déplacement de la souris en X et en Y
            transform.Rotate(-Input.GetAxis("Mouse Y") * vitesseRotation, Input.GetAxis("Mouse X") * vitesseRotation, 0);

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            // Cache le curseur et le 'lock' pour l'empêcher de bouger, ça rend le gameplay meilleur
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // Prend l'angle actuel en X
            float currentPitch = transform.localEulerAngles.x;
            // Si l'angle est plus grand que que 180, lui enlève 360, pour rester autour de -180 et 180
            // (Plus simple à utilise à ce qu'il paraît)
            if (currentPitch > 180f)
            {
                currentPitch -= 360f;
            }
            // BLoque l'angle entre -17 et 50 afin de l'empêcher d'aller en dessous du sol
            currentPitch = Mathf.Clamp(currentPitch, -17f, 50f);
            // Corrige l'angle si y'a lieu
            transform.localEulerAngles = new Vector3(currentPitch, transform.localEulerAngles.y, 0);

        }


        // Regarde la direction du personnage
        cameraPerso.transform.LookAt(transform);
    }
}
