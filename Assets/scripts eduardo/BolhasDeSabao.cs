using System.Collections;
using UnityEngine;

// Cria bolhas de sabão e respingos de água que se movem, desbotam e somem sozinhos.
public class BolhasDeSabao : MonoBehaviour
{
    public Sprite spriteBolha;
    public Material material;
    public int ordemDeDesenho = 11;

    // Deslocamento vertical positivo sobe, como uma bolha. Negativo cai, como um respingo.
    public void Soltar(Vector3 origem, int quantidade, float espalhamento, Color cor, float tamanho, float deslocamentoVertical, float duracao)
    {
        for (int i = 0; i < quantidade; i++)
        {
            GameObject objeto = new GameObject("Bolha");
            objeto.transform.SetParent(transform, false);

            SpriteRenderer visual = objeto.AddComponent<SpriteRenderer>();
            visual.sprite = spriteBolha;
            visual.sharedMaterial = material;
            visual.color = cor;
            visual.sortingOrder = ordemDeDesenho;

            Vector3 inicio = origem + new Vector3(Random.Range(-espalhamento, espalhamento), Random.Range(-espalhamento, espalhamento) * 0.5f, 0f);
            inicio.z = transform.position.z;

            float tamanhoFinal = tamanho * Random.Range(0.7f, 1.3f);
            StartCoroutine(Animar(objeto.transform, visual, inicio, tamanhoFinal, deslocamentoVertical, duracao * Random.Range(0.8f, 1.2f)));
        }
    }

    private IEnumerator Animar(Transform bolha, SpriteRenderer visual, Vector3 inicio, float tamanho, float deslocamentoVertical, float duracao)
    {
        Color cor = visual.color;
        float alfaInicial = cor.a;
        float balanco = Random.Range(0f, 6.28f);
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / duracao);

            float lado = Mathf.Sin(balanco + tempo * 7f) * 0.08f;
            bolha.position = inicio + new Vector3(lado, deslocamentoVertical * progresso, 0f);
            bolha.localScale = Vector3.one * Mathf.Lerp(tamanho * 0.5f, tamanho, progresso);

            cor.a = Mathf.Lerp(alfaInicial, 0f, progresso * progresso);
            visual.color = cor;

            yield return null;
        }

        Destroy(bolha.gameObject);
    }
}
