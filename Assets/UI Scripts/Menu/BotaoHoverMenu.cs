
using UnityEngine;
using UnityEngine.EventSystems;

public class BotaoHoverMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Escala")]
    [SerializeField] private float multiplicadorEscala = 1.12f;
    [SerializeField] private float velocidadeEscala = 10f;

    [Header("Flutuacao")]
    [SerializeField] private float alturaFlutuacao = 5f;
    [SerializeField] private float velocidadeFlutuacao = 3f;
    [SerializeField] private float suavidadeFlutuacao = 10f;

    private RectTransform rectTransform;

    private Vector3 escalaOriginal;
    private Vector2 posicaoOriginal;

    private bool mouseSobre = false;
    private float tempoFlutuacao = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        escalaOriginal = rectTransform.localScale;
        posicaoOriginal = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float delta = Time.unscaledDeltaTime;

        // Aumenta ou restaura o tamanho do botão.
        Vector3 escalaAlvo = mouseSobre
            ? escalaOriginal * multiplicadorEscala
            : escalaOriginal;

        float suavidadeEscala = 1f - Mathf.Exp(-velocidadeEscala * delta);

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            escalaAlvo,
            suavidadeEscala
        );

        // Movimento vertical enquanto o mouse estiver sobre o botão.
        if (mouseSobre)
        {
            tempoFlutuacao += delta * velocidadeFlutuacao;

            float deslocamento = Mathf.Sin(tempoFlutuacao) * alturaFlutuacao;

            Vector2 posicaoAlvo = posicaoOriginal + Vector2.up * deslocamento;

            float suavidade = 1f - Mathf.Exp(-suavidadeFlutuacao * delta);

            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                posicaoAlvo,
                suavidade
            );
        }
        else
        {
            // Retorna à posição original quando o mouse sai.
            float suavidade = 1f - Mathf.Exp(-suavidadeFlutuacao * delta);

            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                posicaoOriginal,
                suavidade
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseSobre = true;
        tempoFlutuacao = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseSobre = false;
    }
}
