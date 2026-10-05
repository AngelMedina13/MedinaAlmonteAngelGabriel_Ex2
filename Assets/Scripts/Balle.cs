using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Balle : MonoBehaviour
{

    [Header("État de jeu")]
    Vector3 positionBalle;
    int points = 0;
    [SerializeField] bool peutJouer;


    [Header("Paramètres de tir")]
    [SerializeField] float angleTir;
    [SerializeField] float vitesseRotation;
    [SerializeField] float forceTir;
    Rigidbody rigidbodyBalle;
    LineRenderer lineRendererBalle;

    [Header("Gauge de force")]
    [SerializeField] Slider jaugeForce;


    [Header("Input Actions")]
    [SerializeField] InputAction tirAction;
    [SerializeField] InputAction tournerAction;


    [Header("Sons")]
    AudioSource audioSourceBalle;
    [SerializeField] AudioClip sonFin;
    [SerializeField] AudioClip sonErreur;
    [SerializeField] AudioClip sonTir;
    [SerializeField] AudioClip backgroundBgm;


    [Header("UI")]
    [SerializeField] TMP_Text textePoints;


    void Start()
    {
        rigidbodyBalle = GetComponent<Rigidbody>();
        lineRendererBalle = GetComponent<LineRenderer>();
        audioSourceBalle = GetComponent<AudioSource>();
        points = 0;
        textePoints.text = $"Coups: {points}";
        peutJouer = true;
        string positionJson = PlayerPrefs.GetString("positionBalle");
        audioSourceBalle.clip = backgroundBgm;
        audioSourceBalle.loop = true;
        audioSourceBalle.Play();
    }

    void Update()
    {
        if (peutJouer == true && GestionaireJeu.instance.etat == EtatJeu.Jeu)
        {
            if (rigidbodyBalle.linearVelocity.magnitude > 0.1f)
            {
                lineRendererBalle.enabled = false;
            }
            else
            {
                lineRendererBalle.enabled = true;
            }

            float inputRotation = tournerAction.ReadValue<float>();

            angleTir += inputRotation;

            Vector3 direction = Quaternion.Euler(0, angleTir, 0) * Vector3.forward;
            lineRendererBalle.SetPosition(0, transform.position);
            lineRendererBalle.SetPosition(1, transform.position + direction);
            if (tirAction.WasPressedThisFrame())
            {
                forceTir = 0;
                MettreAJourUI();
            }


            if (tirAction.IsPressed())
            {
                forceTir += 1;
                forceTir = Mathf.Clamp(forceTir, jaugeForce.minValue, jaugeForce.maxValue);
                MettreAJourUI();
            }

            if (tirAction.WasReleasedThisFrame())
            {
                points++;
                textePoints.text = $"Coups: {points}";
                audioSourceBalle.PlayOneShot(sonTir);
                positionBalle = transform.position;
                string positionJson = JsonUtility.ToJson(positionBalle);
                PlayerPrefs.SetString("positionBalle", positionJson);
                rigidbodyBalle.AddForce(direction * forceTir * Time.deltaTime, ForceMode.Impulse);
                StartCoroutine(VerififerApresTir());
                forceTir = 0;
                MettreAJourUI();
            }

        }
    }

    IEnumerator VerififerApresTir()
    {
        peutJouer = false;
        lineRendererBalle.enabled = false;

        yield return new WaitForFixedUpdate();

        while (rigidbodyBalle.linearVelocity.magnitude > 0.1f)
        {
            yield return null;
        }

        peutJouer = true;
        Vector3 direction = Quaternion.Euler(0, angleTir, 0) * Vector3.forward;
        lineRendererBalle.SetPosition(0, transform.position);
        lineRendererBalle.SetPosition(1, transform.position + direction);
        lineRendererBalle.enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "horsParcours")
        {
            Debug.Log("opsd");

            //replace balle là où il était
            rigidbodyBalle.linearVelocity = Vector3.zero;
            transform.position = positionBalle;

            audioSourceBalle.PlayOneShot(sonErreur);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "trou")
        {
            rigidbodyBalle.linearVelocity = Vector3.zero;
            rigidbodyBalle.useGravity = false;
            transform.position = collision.transform.position;
            PlayerPrefs.DeleteKey("positionBalle");
            audioSourceBalle.PlayOneShot(sonFin);
            PlayerPrefs.SetInt("NbCoups", points);
            StartCoroutine(GestionaireJeu.instance.FinJeu());
        }
    }

    // ===================
    void FrapperBalle()
    {

    }

    void MettreAJourUI()
    {
        jaugeForce.value = forceTir;
    }

    // IEnumerator FinJeu()
    // {

    // }

    void SauvegarderScore()
    {

    }

    //=================================
    // Gestion des inputs actions
    void OnEnable()
    {
        tirAction.Enable();
        tournerAction.Enable();
    }

    void OnDisable()
    {
        tirAction.Disable();
        tournerAction.Disable();
    }
}
