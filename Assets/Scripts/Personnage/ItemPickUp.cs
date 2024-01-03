using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{

    // Général pour faire fonctionner le Pick Up
    // La caméra à partir duquel le RayCast part
    public Camera CameraFPS;
    // La longueur du RayCast
    public float ramasserDistance = 5f;
    // L'objet qui est en focus par le Ray Cast
    GameObject objetEnFocus = null;
    // Gestionnaire de camera, sert à changer de Camera selon les situatios
    public GameObject GestionCam;
    // Raccourci pour GetComponent Animator
    Animator AnimPerso;
    // Raccourci pour GetComponent ScriptPe
    ScriptPe ScriptPerso;
    // Stocke le prochain Enigme du joueur
    string prochainEnigme;
    // Les instructions qui apparaissent selon les actions du joueur
    public GameObject[] Instructions;
    // Torche et fusil à prendre par terre
    public GameObject torche;
    public GameObject fusil;
    // Différentes portes à ouvrir selon les situations
    public GameObject porteTombe;
    public GameObject porteBarils;
    public GameObject porteSortie;
    // Matériel à changer selon les actions du joueur
    public Material trouCleNoirMateriel;
    public Material trouCleBlancMateriel;
    // Valide si le levier est baissé ou pas encore
    bool levierBaissee = false;
    // Sert à enregistrer la rotation du levier afin de l'arrêter à 90
    Vector3 rotationActuelleLevier;
    // Enregistre le gameObject pour continuer de l'utiliser même si le RayCast ne le touche plus
    GameObject levierHit;
    // Confirme si tel ou tel clé a été récupéré afin de débloquer le prochain énigme
    bool cleBaril = false;
    bool cleCoffre = false;
    // Différente clé à animer selon les situations
    public GameObject cleDansBaril;
    public GameObject cleSortie;
    // Particule à faire apparaître quand un baril se fait détruire
    public GameObject explosionBarilParticules;
    // Différents point d'apparition à utiliser pour la potion surprise
    public GameObject pointApparitionTombe;
    public GameObject pointApparitionBaril;
    public GameObject pointApparitionCoffre;
    public GameObject pointApparitionSortie;
    // Détermine si le coffre a été ouvert ou pas
    bool coffreOuvert = false;

    public AudioClip sonPortePierre;
    public AudioClip sonPorteBois;
    public AudioClip sonPotion;
    // Enregistre le coffre pour savoir quand son animation fini
    GameObject coffreObjet;

    void Start()
    {
        // Raccourcis
        AnimPerso = GetComponent<Animator>();
        ScriptPerso = GetComponent<ScriptPe>();

    }

    // Update is called once per frame
    void Update()
    {
        // Si ces animations jouent ou que ces instructions sont affichés, la vitesse se met à 0 à la fois en vélocité et en animation
        // Et empêche le personnage de bouger en mettant la variable peutBouger à false
        // Si rien ne joue, et rien n'est affiché, alors le perso peut bouger
        if (AnimPerso.GetCurrentAnimatorStateInfo(0).IsName("Store Item") ||
            AnimPerso.GetCurrentAnimatorStateInfo(0).IsName("Equip Rifle") ||
            AnimPerso.GetCurrentAnimatorStateInfo(0).IsName("Unequip Rifle") ||
            AnimPerso.GetCurrentAnimatorStateInfo(0).IsName("Equip Torch") ||
            AnimPerso.GetCurrentAnimatorStateInfo(0).IsName("Unequip Torch") ||
            AnimPerso.GetCurrentAnimatorStateInfo(0).IsName("Pulling Lever") ||
            Instructions[0].activeInHierarchy || Instructions[1].activeInHierarchy || Instructions[2].activeInHierarchy)
        {
            ScriptPerso.peutBouger = false;
            AnimPerso.SetFloat("Speed", 0);
            GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, 0f);
        }
        else
        {
            ScriptPerso.peutBouger = true;
        }

        // Lance la fonction RayCast à chaque frame
        RayCast();
        // Tant que levierBaisse est à true, et que la rotation en Z n'est toujours pas à 90, rajoute 1, jusqu'à 90
        // Une fois à 90, lance l'animation de la porte
        if (levierBaissee && rotationActuelleLevier.z < 90)
        {
            rotationActuelleLevier.z += 1f;
            levierHit.transform.eulerAngles = rotationActuelleLevier;
            if (rotationActuelleLevier.z >= 90f)
            {
                porteTombe.GetComponent<Animator>().enabled = true;
                GetComponent<AudioSource>().PlayOneShot(sonPortePierre);

            }
        }
        // Si le coffre a été ouvert, et que l'animation ne joue plus, active la clé qui sort du coffre
        if (coffreOuvert && !coffreObjet.GetComponent<Animation>().isPlaying)
        {

            if (cleSortie != null && !cleSortie.activeSelf)
            {
                cleSortie.SetActive(true);
            }

        }
    }
    void RayCast()
    {

        // Le ray du RayCast est la position de la caméra jusqu'à la direction de l'input de la souris
        // (Marche bien avec cursor.visible = none et cursor.lock)
        Ray ray = CameraFPS.ScreenPointToRay(Input.mousePosition);
        // Variable de type Raycast
        RaycastHit hit; // C'est en anglais, mais elle me semble vraiment + intuitive comme nom de variable

        // Dessine une ligne de debugage afin de mieux voir
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * ramasserDistance, Color.red);

        // Utilise la fonction Raycast à partir de la caméra FPS, direction en avant jusqu'à la position de la souris avec la distance de 
        // ramasserDistance, met ensuite les informations dans la variable hit (l'objet que le Cast a hit dans mon cas)

        if (Physics.Raycast(ray, out hit, ramasserDistance))
        {

            // Longue explication à cause de la répétition, on stocke d'abord l'objet touché dans une variable gameObject
            // On vérifie si l'objetEnFocus est le même que l'objet touché, si c'est le même on fait rien pour ne pas le lancer à chaque frame
            // Si ce n'est pas le même, on vérifie si l'objetEnFocus est autre chose que null (et en même temps pas celui qu'on touche), 
            // si c'est les cas, retire le OutLine de cet objet après avoir vérifié qu'elle existe, on attribue l'objet touché à l'objet en focus
            // On active sa bordure si elle existe
        

            if (hit.collider.CompareTag("torche"))
            {
                // Stocke l'objet dans une variable gameObject
                GameObject objetHit = hit.collider.gameObject;

                // Est-ce que l'objet en focus(décidé après) est le même que celui-ci, donc torche
                // cela évite de lancer le code à chaque frame
                if (objetEnFocus != objetHit)
                {

                    // Est-ce que l'objetEnFocus existe, et en même temps n'est pas torche, si c'est le cas
                    // Lui retire son script d'Outline, importé pour souligner les items
                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }
                    // Enregistre l'objetHit dans objetEnFocus afin de pouvoir l'utiliser partout
                    // Puis enregistre son script dans une variable qu'on active si le script existe
                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }


                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    // Détruit l'objet en question, donc la torche  
                    Destroy(hit.collider.gameObject);
                    // Met la variable torch du script à true (Permet d'activer la fonction de torche)
                    ScriptPerso.torch = true;
                    // Active l'objet Torche lié à la main
                    torche.SetActive(true);
                    // Le désactive 1.2 secondes après
                    Invoke("disableTorch", 1.2f);
                    // Montre les instructions après 
                    Invoke("montrerInstructions1", 1.5f);


                }
            }
            else if (hit.collider.CompareTag("Gun")) {

                // Stocke l'objet dans une variable gameObject
                GameObject objetHit = hit.collider.gameObject;

                // Est-ce que l'objet en focus(décidé après) est le même que celui-ci, donc torche
                // cela évite de lancer le code à chaque frame
                if (objetEnFocus != objetHit)
                {

                    // Est-ce que l'objetEnFocus existe, et en même temps n'est pas torche, si c'est le cas
                    // Lui retire son script d'Outline, importé pour souligner les items
                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }
                    // Enregistre l'objetHit dans objetEnFocus afin de pouvoir l'utiliser partout
                    // Puis enregistre son script dans une variable qu'on active si le script existe
                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }

                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    // Détruit l'objet en question, donc la torche  
                    Destroy(hit.collider.gameObject);
                    // Met la variable fusil du script à true (Permet d'activer la fonction de fusil)
                    ScriptPerso.fusil = true;
                    // Active l'objet Fusil lié à la main
                    fusil.SetActive(true);
                    // Le désactive 1.2 secondes après
                    Invoke("disableFusil", 1.2f);
                }
            }
            else if (hit.collider.CompareTag("levier"))
            {
                // Stocke l'objet dans une variable gameObject
                GameObject objetHit = hit.collider.gameObject;

                // Est-ce que l'objet en focus(décidé après) est le même que celui-ci, donc torche
                // cela évite de lancer le code à chaque frame
                if (objetEnFocus != objetHit)
                {
                    // Est-ce que l'objetEnFocus existe, et en même temps n'est pas torche, si c'est le cas
                    // Lui retire son script d'Outline, importé pour souligner les items
                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }
                    // Enregistre l'objetHit dans objetEnFocus afin de pouvoir l'utiliser partout
                    // Puis enregistre son script dans une variable qu'on active si le script existe
                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }
                // Si le joueur appuie sur E et que levierBaissee est false
                if (Input.GetKeyDown(KeyCode.E) && !levierBaissee)
                {
                    // On active l'animation du levier
                    AnimPerso.SetBool("Lever", true);
                    // Enregistre l'objet (le levier) dans une variable gameObject initialisé plus haut
                    levierHit = hit.collider.gameObject;
                    // Enregesitre la rotation du levier
                    rotationActuelleLevier = levierHit.transform.eulerAngles;
                    // Met levierBaissee à true pour éviter de relancer le code
                    levierBaissee = true;
                }

            }
            else if (hit.collider.CompareTag("cleBaril"))
            {

                // Meme chose que plus haut

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }

                if (Input.GetKey(KeyCode.E))
                {
                    // Lance l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    // Détruit l'objet en question
                    Destroy(hit.collider.gameObject);
                    // Met la clé du baril à true afin de pouvoir ouvrir une poarte plus tard
                    cleBaril = true;
                    // Montre les instructions pour le prochain énigme
                    Instructions[1].SetActive(true);
                    // Retire les instructions après un certain temps
                    Invoke("cacherInstruction2", 3f);
                }
            }
            else if (hit.collider.CompareTag("trouCle"))
            {

                // Même chose

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        // Cette fois, si l'objet existe déjà, on lui remet le materiel de la serrureNoir
                        Renderer imagePrecedente = objetEnFocus.GetComponent<Renderer>();
                        if (imagePrecedente != null)
                        {
                            imagePrecedente.material = trouCleNoirMateriel;
                        }
                    }

                    objetEnFocus = objetHit;
                    // Ici on stocke le renderer de l'objet, et si il existe, on lui met le matiriel de la serrureBlanche
                    Renderer imageActuelle = objetHit.GetComponent<Renderer>();
                    if (imageActuelle != null)
                    {
                        imageActuelle.material = trouCleBlancMateriel;
                    }
                }
                // Si le joueur appuie sur E et qu'il possède la clé baril
                if (Input.GetKeyDown(KeyCode.E) && cleBaril)
                {
                    // Active l'animator de la porte
                    porteBarils.GetComponent<Animator>().enabled = true;
                    // Active la fonction du script de gesion de caméra pour activer la caméra
                    // qui permet de voir l'ouverture de la porte
                    GestionCam.GetComponent<GestionnaireCamera>().ActiveCam(2);
                    GetComponent<AudioSource>().PlayOneShot(sonPorteBois);
                    // Remet automatiquement la caméra normale après 5 secondes
                    Invoke("cameraNormale", 5f);
                }
            }
            else if (hit.collider.CompareTag("baril"))
            {
                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }


                // Quand il appuie sur E
                if (Input.GetKey(KeyCode.E))
                {   
                    // Clone les particules d'explosion de baril
                    GameObject explosionBaril = Instantiate(explosionBarilParticules);
                    // Positionne le clone à la position du baril
                    explosionBaril.transform.position = hit.collider.gameObject.transform.position;

                    // Si le baril touché est nommé BarilCle active la clé du coffre afin de pouvoir la récupérer
                    if (hit.collider.gameObject.name == "BarilCle")
                    {
                        Invoke("ActiveCleCoffre", 0.8f);
                    }

                    // Active les particule puis détruit l'objet
                    explosionBaril.SetActive(true);
                    Destroy(hit.collider.gameObject);


                }
            }
            else if (hit.collider.CompareTag("coffre"))
            {

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }

                // Si il appuie sur E et qu'il possède la clé du coffre
                if (Input.GetKey(KeyCode.E) && cleCoffre)
                {
                    // Et que le coffre a jamais été ouvert
                    if (!coffreOuvert)
                    {
                        // Active l'animation du coffre
                        hit.collider.gameObject.GetComponent<Animation>().Play();
                        // Met coffreOuvert à true pour ne pas répéter cette condition
                        coffreOuvert = true;
                        // Enregistre le coffre dans une variable
                        coffreObjet = hit.collider.gameObject;
                        // Enlève le tag du coffre et change son layer pour ignorer le RayCast
                        hit.collider.gameObject.tag = "Untagged";
                        hit.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }


                }

            }
            else if (hit.collider.CompareTag("cleSortie"))
            {

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }

                // Si le joueur appuie sur E
                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de la porte de sortie
                    porteSortie.GetComponent<Animator>().enabled = true;
                    // Active la caméra de la porte de sortie afin que le joueur le voit
                    GestionCam.GetComponent<GestionnaireCamera>().ActiveCam(3);
                    GetComponent<AudioSource>().PlayOneShot(sonPortePierre);
                    // Détruit la clé
                    Destroy(hit.collider.gameObject);
                    // Remet la caméra normale après 2 secondes
                    Invoke("cameraNormale", 2f);
                    // Met à jour le prochain énigme
                    prochainEnigme = "sortie";
                }

            }
            else if (hit.collider.CompareTag("cleCoffre"))
            {


                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }

                // Si le joueur appuie sur E
                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    // Détruit l'objet en question
                    Destroy(hit.collider.gameObject);
                    // Met la clé du coffre à true afin que le joueur puisse ouvrir le coffre
                    cleCoffre = true;
                    // Montre les instructions
                    Instructions[2].SetActive(true);
                    // Les enlève 3 secondes après
                    Invoke("cacherInstruction3", 3f);
                }
            }
            else if (hit.collider.CompareTag("potionVitesse"))
            {

                // Même chose

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }
                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    GetComponent<AudioSource>().PlayOneShot(sonPotion);
                    // Détruit l'objet en question
                    Destroy(hit.collider.gameObject);
                    // Met à true la variable de potion vitesse ce qui permet au personnage de courir + vite
                    ScriptPerso.potionVitesse = true;
                    // Enlève la potion vitesse
                    Invoke("desactiverPotionVitesse", 25f);
                }
            }
            else if (hit.collider.CompareTag("potionPV"))
            {

                // Même chose

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }
                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    GetComponent<AudioSource>().PlayOneShot(sonPotion);
                    // Détruit l'objet en question
                    Destroy(hit.collider.gameObject);
                    // Ajoute 50 PV aux PV du joueur
                    GetComponent<GererJeu>().PV += 50;
                    // Si les PV dépasse 150 les remet à 150 car c'est le maximum
                    if (GetComponent<GererJeu>().PV > 150)
                    {
                        GetComponent<GererJeu>().PV = 150;
                    }

                }
            }
            else if (hit.collider.CompareTag("potionHasard"))
            {

                GameObject objetHit = hit.collider.gameObject;

                if (objetEnFocus != objetHit)
                {

                    if (objetEnFocus != null)
                    {
                        Outline bordurePrecedente = objetEnFocus.GetComponent<Outline>();
                        if (bordurePrecedente != null)
                        {
                            bordurePrecedente.enabled = false;
                        }
                    }

                    objetEnFocus = objetHit;
                    Outline bordureActuelle = objetHit.GetComponent<Outline>();
                    if (bordureActuelle != null)
                    {
                        bordureActuelle.enabled = true;
                    }
                }

                if (Input.GetKey(KeyCode.E))
                {
                    // Active l'animation de Store(Stocker)
                    AnimPerso.SetBool("Store", true);
                    GetComponent<AudioSource>().PlayOneShot(sonPotion);
                    // Détruit l'objet en question
                    Destroy(hit.collider.gameObject);
                    // Choisis une valeur float au hasard entre 0 et 1
                    float valeurAuHasard = Random.Range(0f, 1f);
                    // Si la valeur est plus petite que 0.1 (10% des possibilités)
                    if (valeurAuHasard < 0.1f)
                    {
                        // Téléporte le personnage à la prochaine énigme
                        if (prochainEnigme == "tombe")
                        {
                            gameObject.transform.position = pointApparitionTombe.transform.position;
                        }
                        else if (prochainEnigme == "baril")
                        {
                            gameObject.transform.position = pointApparitionBaril.transform.position;
                        }
                        else if (prochainEnigme == "coffre")
                        {
                            gameObject.transform.position = pointApparitionCoffre.transform.position;
                        }
                        else if (prochainEnigme == "sortie")
                        {
                            gameObject.transform.position = pointApparitionSortie.transform.position;
                        }
                        // Si il n'a toujours pas commencé d'énigme, un malus est appliqué, réduction de vitesse
                        else
                        {
                            ScriptPerso.vitesseNormale /= 2;
                            Invoke("RetoureVitesseNormal", 25f);
                        }
                    }
                    // S'il n'est pas chanceux le malus est appliqué, réduction de vitesse
                    else
                    {
                        ScriptPerso.vitesseNormale /= 2;
                        Invoke("RetoureVitesseNormal", 25f);
                    }


                }

            }
            else
            {

                // Si aucun des tag mentionné n'est hit, reset l'objetEnFocus, en désactivant le script d'Outline
                // ou en remettant le materiel de la serrure noir si c'est 'trouCle', puis met l'objet à null

                if (objetEnFocus != null)
                {
                    Outline focusedOutline = objetEnFocus.GetComponent<Outline>();
                    Renderer imageEnFocus = objetEnFocus.GetComponent<Renderer>();

                    if (focusedOutline != null)
                    {
                        focusedOutline.enabled = false;
                    }

                    if (imageEnFocus != null && imageEnFocus.tag == "trouCle")
                    {
                        imageEnFocus.material = trouCleNoirMateriel;
                    }

                    objetEnFocus = null;
                }
            }
        }


    }
    // Reactive la caméra normal, position 0 du gesitonnaire de camera
    void cameraNormale()
    {
        GestionCam.GetComponent<GestionnaireCamera>().ActiveCam(0);
    }
    // Désactive la torche et cache les instructions après 3 secondes
    public void disableTorch()
    {
        torche.SetActive(false);

        Invoke("cacherInstruction1", 3f);
    }
    // Désactive le fusil
    void disableFusil() {
        fusil.SetActive(false);
    }
    // Montre les instructions
    void montrerInstructions1()
    {
        Instructions[0].SetActive(true);
    }
    // Desactive la potion de vitesse
    void desactiverPotionVitesse()
    {
        ScriptPerso.potionVitesse = false;
    }
    // Remet la vitesse a la normale (Divisé par deux avant)
    void RetoureVitesseNormal()
    {
        ScriptPerso.vitesseNormale *= 2;
    }
    // Active la clé dans le baril
    void ActiveCleCoffre()
    {
        cleDansBaril.SetActive(true);
    }
    // Fonction qui cache les instructions selon leur position dans le tableau
    void cacherInstructions(int Nombre)
    {
        Instructions[Nombre].SetActive(false);
    }
    // Cache les instructions adéquatement
    void cacherInstruction1()
    {
        cacherInstructions(0);
    }
    void cacherInstruction2()
    {
        cacherInstructions(1);
    }
    void cacherInstruction3()
    {
        cacherInstructions(2);
    }
}