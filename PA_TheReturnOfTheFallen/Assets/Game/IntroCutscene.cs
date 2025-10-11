using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroCutscene : MonoBehaviour
{
    public TMP_Text storyText;            
    [TextArea(5, 20)]
    public string[] storyPages;           
    public float letterDelay = 0.1f;     
    public string nextSceneName = "TestArea"; 

    public AudioSource audioSource;       
    public AudioClip typeSound;           

    private int currentPage = 0;
    private bool isTyping = false;

    void Start()
    {
        storyText.text = "";
        if (storyPages.Length > 0)
            StartCoroutine(TypePage(storyPages[currentPage]));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                storyText.text = storyPages[currentPage];
                isTyping = false;
            }
            else
            {
                currentPage++;
                if (currentPage < storyPages.Length)
                {
                    StartCoroutine(TypePage(storyPages[currentPage]));
                }
                else
                {
                    SceneManager.LoadScene(nextSceneName);
                }
            }
        }
    }

    private IEnumerator TypePage(string page)
    {
        isTyping = true;
        storyText.text = "";

        foreach (char c in page)
        {
            storyText.text += c;

            if (audioSource != null && typeSound != null && c != ' ')
            {
                audioSource.PlayOneShot(typeSound, 0.5f);
            }

            yield return new WaitForSeconds(letterDelay);
        }

        isTyping = false;
    }
}
