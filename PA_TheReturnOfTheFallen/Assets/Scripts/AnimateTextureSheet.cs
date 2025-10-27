using UnityEngine;

public class AnimateTextureSheet : MonoBehaviour
{
    // Configure isto no Inspector
    public int columns = 4; // Número de colunas na sua folha de sprites
    public int rows = 5;    // Número de linhas na sua folha de sprites
    public float framesPerSecond = 15f;

    private Renderer objectRenderer;
    private Material portalMaterial;

    void Start()
    {
        // Pega no Renderer do objeto
        objectRenderer = GetComponent<Renderer>();

        // Pega numa instância do material para não alterar o asset original
        portalMaterial = objectRenderer.material;

        // Calcula e define o tamanho (tiling) para mostrar apenas UM frame
        Vector2 tiling = new Vector2(1f / columns, 1f / rows);
        portalMaterial.mainTextureScale = tiling;
    }

    void Update()
    {
        // Calcula o índice total de frames
        int totalFrames = columns * rows;
        if (totalFrames == 0) return;

        // Calcula o frame atual baseado no tempo
        int index = (int)(Time.time * framesPerSecond);
        index = index % totalFrames;

        // Calcula a posição (offset) do frame na folha de sprites
        int u = index % columns; // Coluna atual
        int v = index / columns; // Linha atual

        // O offset em Y precisa de ser invertido porque as coordenadas UV começam em baixo
        Vector2 offset = new Vector2(u / (float)columns, (rows - 1 - v) / (float)rows);

        // Aplica o offset ao material para mostrar o frame correto
        portalMaterial.mainTextureOffset = offset;
    }
}