using UnityEngine;

public class SkillTreeToggle : MonoBehaviour
{
    [SerializeField] private GameObject skillTreeUI; // o GameObject que tem SkillTreeUI
    [SerializeField] private KeyCode toggleKey = KeyCode.P;
    [SerializeField] private bool pauseOnOpen = false; // opcional

    private void Start()
    {
        if (skillTreeUI != null)
            skillTreeUI.SetActive(false); // garante que começa oculto
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (!skillTreeUI) return;

            bool newState = !skillTreeUI.activeSelf;
            skillTreeUI.SetActive(newState);

            if (pauseOnOpen)
                Time.timeScale = newState ? 0f : 1f;
        }
    }
}
