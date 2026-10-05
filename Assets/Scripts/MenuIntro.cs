using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuIntro : MonoBehaviour
{

    AudioSource audioSourceIntro;
    [SerializeField] AudioClip sonFond;


    [SerializeField] TMP_Text textecoups;
    public void Start()
    {
        audioSourceIntro = GetComponent<AudioSource>();
        int nbCoups = PlayerPrefs.GetInt("NbCoups",0);
        textecoups.text = $"Dernier score: {nbCoups} coups";
        audioSourceIntro.clip = sonFond;
        audioSourceIntro.loop = true;
        audioSourceIntro.Play();

    }

    public void DemarrerJeu()
    {
        SceneManager.LoadScene("Jeu");
    }
}
