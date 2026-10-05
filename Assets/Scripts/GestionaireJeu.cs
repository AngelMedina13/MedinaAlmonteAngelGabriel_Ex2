using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public enum EtatJeu
{
    Debut,
    Initialisation,
    Jeu,
    Fin,
}
public class GestionaireJeu : MonoBehaviour
{
    public static GestionaireJeu instance;
    public EtatJeu etat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        etat = EtatJeu.Jeu;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FinJeu()
    {
        etat = EtatJeu.Fin;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Intro");
    }
}
