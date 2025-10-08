using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogueUI;   
    public GameObject shopUI;       
    [HideInInspector] public bool PlayerPerto = false; 

    private enum InteractionState { Nenhum, Dialogo, Loja }
    private InteractionState estadoAtual = InteractionState.Nenhum;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;              
        rb.constraints = RigidbodyConstraints.FreezeAll;

        if (dialogueUI != null)
            dialogueUI.SetActive(false);  

        if (shopUI != null)
            shopUI.SetActive(false);     
    }

    void Update()
    {
        if (!PlayerPerto) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (estadoAtual)
            {
                case InteractionState.Nenhum:
                    
                    if (dialogueUI != null)
                        dialogueUI.SetActive(true);
                    estadoAtual = InteractionState.Dialogo;
                    Debug.Log("Diálogo aberto!");
                    break;

                case InteractionState.Dialogo:
                    
                    if (dialogueUI != null)
                        dialogueUI.SetActive(false);

                    if (shopUI != null)
                        shopUI.SetActive(true);

                    estadoAtual = InteractionState.Loja;
                    Debug.Log("Diálogo fechado, loja aberta!");
                    break;

                case InteractionState.Loja:
                    
                    if (shopUI != null)
                        shopUI.SetActive(false);

                    estadoAtual = InteractionState.Nenhum;
                    Debug.Log("Loja fechada, estado inicial!");
                    break;
            }
        }
    }
}
