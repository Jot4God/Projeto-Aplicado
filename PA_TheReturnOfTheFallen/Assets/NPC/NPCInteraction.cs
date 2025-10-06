using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogueUI;   
    [HideInInspector] public bool PlayerPerto = false; 

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;              
        rb.constraints = RigidbodyConstraints.FreezeAll;

        if (dialogueUI != null)
            dialogueUI.SetActive(false);     
    }

    void Update()
    {
        if (PlayerPerto && Input.GetKeyDown(KeyCode.E))
        {
            dialogueUI.SetActive(!dialogueUI.activeSelf);
            Debug.Log("E pressionado! Abrindo diálogo");
        }
    }
}
