using UnityEngine;

// Seta que flutua em cima do objeto com que o jogador precisa interagir agora.
public class SetaIndicadora : MonoBehaviour
{
    public float distanciaDoAlvo = 0.55f;
    public float amplitude = 0.15f;
    public float velocidade = 4f;

    private Transform alvo;

    public void ApontarPara(Transform novoAlvo)
    {
        alvo = novoAlvo;
        gameObject.SetActive(alvo != null);
    }

    private void LateUpdate()
    {
        if (alvo == null) return;

        float flutuacao = Mathf.Sin(Time.time * velocidade) * amplitude;
        Vector3 posicao = alvo.position + Vector3.up * (distanciaDoAlvo + flutuacao);
        posicao.z = transform.position.z;
        transform.position = posicao;
    }
}
