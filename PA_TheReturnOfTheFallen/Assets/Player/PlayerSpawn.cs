using UnityEngine;

public class PlayerSpawnFade : MonoBehaviour
{
    public float fadeDuration = 1f; 
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        Color c = rend.material.color;
        c.a = 0f; 
        rend.material.color = c;

        StartCoroutine(FadeIn());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = rend.material.color;

        while (elapsed < fadeDuration)
        {
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            rend.material.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }


        c.a = 1f;
        rend.material.color = c;
    }
}
