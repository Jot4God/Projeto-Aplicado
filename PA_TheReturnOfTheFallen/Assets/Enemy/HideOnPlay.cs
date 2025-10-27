using UnityEngine;

// Garante que só o "template" da cena é desativado, nunca os clones.
public class HideOnPlay : MonoBehaviour
{
    void Awake()
    {
        if (!Application.isPlaying) return;

        // Se for um clone, o nome contém "(Clone)" -> não escondas
        if (name.Contains("(Clone)")) return;

        gameObject.SetActive(false);
    }
}
