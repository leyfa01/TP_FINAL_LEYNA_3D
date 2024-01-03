using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPe : MonoBehaviour
{

    // Déclaration des différentes vitesses
    public float vitesseNormale = 8f;
    public float vitesseSprint = 14f;
    public float vitesseSaut = 6f;
    // Vitesse actuel
    float vitesseDeplacement = 0f;
    // Vitesse voulue
    float vitesseVoulue = 8f;
    // Rigidbody du perso 
    Rigidbody RigidbodyPerso;
    // Animator du perso
    Animator AnimPerso;
    // Camera 3e personne
    public GameObject camera3e;
    // Si le perso est au sol ou pas
    bool auSol = true;
    // Si le perso est en saut ou pas
    bool saut = false;
    // Est-ce que le joueur a la torche en sa possession
    public bool torch = false;
    // Est-ce que le joueur a la potion de vitesse
    public bool potionVitesse = false;
    // Est-ce que le joueur a le fusil en sa possession
    public bool fusil = false;
    // Lequel des deux est actif, afin d'éviter d'activer les deux en même temps
    bool torcheActive = false;
    bool fusilActif = false;
    // Décide si le joueur peut bouger ou non
    public bool peutBouger = true;
    float dernierSonDePas;

    public AudioClip sonPas;
    public AudioClip sonSaut;
    public AudioClip sonEquipFusil;
    // Start is called before the first frame update
    void Start()
    {
        // Initialise les variables
        RigidbodyPerso = GetComponent<Rigidbody>();
        AnimPerso = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<GererJeu>().finPartie && peutBouger)
        {

            // Quand le joueur est au sol et que son animation de course est activé
            // Joue l'audio dès que la différence de temps (prend le temps a un moment puis a un autre, et soustrait les 2)
            // entre chaque son est de 0.27 secondes
            if (auSol && AnimPerso.GetFloat("Speed") > 0.1f && Time.time - dernierSonDePas > 0.27f)
        {
            GetComponent<AudioSource>().PlayOneShot(sonPas);
            dernierSonDePas = Time.time;
        }

            float hDeplacement = Input.GetAxisRaw("Horizontal");
            float vDeplacement = Input.GetAxisRaw("Vertical");

            // Met la variable speed de l'animator égale à la vitesse actuel du personnage
            AnimPerso.SetFloat("Speed", vitesseDeplacement);
            // Change la variable turn selon l'axe Horizontale (Pas utile pour le moment)
            AnimPerso.SetFloat("Turn", Input.GetAxis("Horizontal"));
            // Met la direction du perso dans une variable, la direction est décidé par la direction de la camera
            // Et les touches appuyés par le perso
            Vector3 directionDep = camera3e.transform.forward * vDeplacement + camera3e.transform.right * hDeplacement;
            // Met la direction dans l'axe des Y à 0
            directionDep.y = 0;
            

            // Si fusil est true (Donc le perso a le fusil), et qu'il appuie sur Z, change l'état actuel
            // Anime l'équipement de la fusil si elle est déséquipé et vice versa
            if (fusil && Input.GetKeyDown(KeyCode.Z) && !torcheActive)
            {
                bool etat = AnimPerso.GetBool("Fusil");
                AnimPerso.SetBool("Fusil", !etat);
                if (etat == true)
                {
                    Invoke("disableFusil", 0.8f);
                    fusilActif = false;
                }
                else
                {
                    Invoke("enableFusil", 0f);
                    fusilActif = true;
                    GetComponent<AudioSource>().PlayOneShot(sonEquipFusil);
                }
            }
            // Si torch est true (Donc le perso a la torche), et qu'il appuie sur G, change l'état actuel
            // Anime l'équipement de la torche si elle est déséquipé et vice versa
            if (torch && Input.GetKeyDown(KeyCode.G) && !fusilActif)
            {
                bool etat = AnimPerso.GetBool("Torche");
                AnimPerso.SetBool("Torche", !etat);
                if (etat == true)
                {
                    Invoke("disableTorch", 1.2f);
                    torcheActive = false;
                }
                else
                {
                    Invoke("enableTorch", 0.6f);
                    torcheActive = true;
                }
            }
   
            // Si le joueur appuie sur espace et qu'il est au sol, lance l'animation de saut
            // Selon la vitesse de deplacement du personnage, l'animation de saut n'est pas la même
            if (Input.GetButton("Jump") && auSol)
            {
                AnimPerso.SetBool("Jump", true);
                auSol = false;
                saut = true;
                // Empêche le personnage de tirer
                GetComponent<GererTir>().peutTirer = false;
                // Joue le son du saut
                GetComponent<AudioSource>().PlayOneShot(sonSaut);
            }


            // Si saut est true (Saut en cours), met la vélocité y du perso à vitesseSaut puis remet saut à false
            if (saut)
            {
                RigidbodyPerso.velocity = new Vector3(RigidbodyPerso.velocity.x, vitesseSaut, RigidbodyPerso.velocity.z);
                saut = false;
            }

            // Si directionDep est égale à autre chose que 0, (Si aucune touche n'est appuyé, le raw donne 0)
            // Donc si une touche de mouvement est appuyé, lance le code
            if (directionDep != Vector3.zero)
            {
                // Tourne le personnage vers la direction calculée selon la caméra plus haut
                transform.forward = directionDep;
                // Change la vélocité du personnage en lui donnant de la vitesse vers cette direction
                // Et garde la vélocité du rigidbody (Gravité) en y
                RigidbodyPerso.velocity = (transform.forward * vitesseDeplacement) + new Vector3(0, RigidbodyPerso.velocity.y, 0);


                // La vitesseDeplacement augmente petit à petit jusqu'à la vitesse voulue grâce à Lerp
                // Si ils sont égals, cela fait rien, ça permet de profiter du blendTree
                vitesseDeplacement = Mathf.Lerp(vitesseDeplacement, vitesseVoulue, Time.deltaTime * 10);


                // Si le joueur a pas de potion de vitesse, sa vitesse est la vitesse normale
                if (!potionVitesse)
                {
                    vitesseVoulue = vitesseNormale;
                } // Si il a la postion de vitesse, c'Est la vitesseSprint qui est activé
                else 
                {
                    vitesseVoulue = vitesseSprint;
                }



            }
            // Si rien n'est appuyé, met la variable Speed à 0, ce qui joue l'animation de repos,
            // et met la vélocité du personnage en x et z à 0
            else
            {
                AnimPerso.SetFloat("Speed", 0);
                RigidbodyPerso.velocity = new Vector3(0, RigidbodyPerso.velocity.y, 0);
            }
        }

    }
    // Si y'a une collision entre le perso et le sol
    void OnCollisionEnter(Collision collision)
    {   
    
        if (collision.gameObject.tag == "sol")
        {   
            // Met la variable auSol à true
            auSol = true;
            // Met le bool animator Jump à false
            AnimPerso.SetBool("Jump", false);
            // Permet au joueur de tirer
            GetComponent<GererTir>().peutTirer = true;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        // Si le joueur rentre en collision trigger avec l'objet sortie
        if (collision.gameObject.tag == "sortie")
        {
            // Active la fin de partie et met le float d'animator speed à 0
            GetComponent<GererJeu>().finPartie = true;
            GetComponent<Animator>().SetFloat("Speed", 0);
        }
    }
    // Désactive la torche par l'autre script
    void disableTorch()
    {
        GetComponent<ItemPickUp>().disableTorch();
    }
    // Active la torche par l'autre script
    void enableTorch()
    {
        GetComponent<ItemPickUp>().torche.SetActive(true);
    }
    // Désactive le fusil par l'autre script
    void disableFusil()
    {
        GetComponent<ItemPickUp>().fusil.SetActive(false);
    }
    // Active le fusil par l'autre script
    void enableFusil()
    {
        GetComponent<ItemPickUp>().fusil.SetActive(true);
    }
}
