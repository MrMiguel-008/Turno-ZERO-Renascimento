using UnityEngine;

// Troca o fundo da pia pelos quadros da arte: ao ligar a água eles avançam e ficam no último,
// ao desligar eles voltam até a pia seca.
public class AnimacaoTorneira : MonoBehaviour
{
    public SpriteRenderer fundo;
    public Sprite[] quadrosAgua;
    public Sprite[] quadrosSabao;
    public float quadrosPorSegundo = 14f;

    private Sprite[] quadrosAtuais;
    private float quadroAtual = 0f;
    private bool ligada = false;

    public void Ligar(bool comSabao)
    {
        quadrosAtuais = comSabao ? quadrosSabao : quadrosAgua;
        ligada = true;
    }

    public void Desligar()
    {
        ligada = false;
    }

    private void Update()
    {
        if (quadrosAtuais == null || quadrosAtuais.Length == 0) return;

        float alvo = ligada ? quadrosAtuais.Length - 1 : 0f;
        quadroAtual = Mathf.MoveTowards(quadroAtual, alvo, quadrosPorSegundo * Time.deltaTime);
        fundo.sprite = quadrosAtuais[Mathf.RoundToInt(quadroAtual)];
    }
}
