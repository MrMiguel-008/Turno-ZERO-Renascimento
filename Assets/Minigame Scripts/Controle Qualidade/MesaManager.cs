using UnityEngine;

public class MesaManager : MonoBehaviour
{
    public static MesaManager Instancia { get; private set; }

    [Header("Referências dos Sprites na Cena")]
    public SpriteRenderer rendererComputador;
    public SpriteRenderer rendererCaixa;
    public MesaArrastar scriptScanner;
    public MesaArrastar scriptFita;

    [Header("Novos Sprites de Estado")]
    public Sprite computadorAtualizado;
    public Sprite caixaAtualizada;

    private bool scannerJaFoi = false;
    private bool minigameConcluido = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void ProcessarEntrega(string tipoItem)
    {
        if (minigameConcluido) return;

        if (tipoItem == "Scanner" && !scannerJaFoi)
        {
            // Passo 1: Scanner levado até a caixa
            scannerJaFoi = true;
            if (computadorAtualizado != null) rendererComputador.sprite = computadorAtualizado;

            // Retorna o scanner para a posição inicial na mesa
            scriptScanner.RetornarOrigem();
            Debug.Log("Scanner validado! Agora leve a fita.");
        }
        else if (tipoItem == "Fita" && scannerJaFoi)
        {
            // Passo 2: Fita levada até a caixa (só funciona se o scanner já foi)
            minigameConcluido = true;
            if (caixaAtualizada != null) rendererCaixa.sprite = caixaAtualizada;

            // Retorna a fita para a posição inicial na mesa
            scriptFita.RetornarOrigem();
            Debug.Log("Minigame concluído!");

            FinalizarMinigame();
        }
        else
        {
            // Se tentar a fita antes do scanner, ou qualquer ação inválida, os itens voltam
            if (tipoItem == "Scanner") scriptScanner.RetornarOrigem();
            if (tipoItem == "Fita") scriptFita.RetornarOrigem();
        }
    }

    void FinalizarMinigame()
    {
        // Insira aqui o código para liberar a movimentação do jogador
    }
}
