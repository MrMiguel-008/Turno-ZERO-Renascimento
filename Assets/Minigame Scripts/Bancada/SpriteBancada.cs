using UnityEngine;

public class SpriteBancada : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Tooltip("Imagem normal/inicial do objeto.")]
    [SerializeField] private Sprite spriteNormal;

    [Tooltip("Imagem modificada após iniciar o minigame.")]
    [SerializeField] private Sprite spriteModificado;

    [Tooltip("O mesmo nome de chave que você vai usar no script do minigame.")]
    [SerializeField] private string chaveMinigame = "MinigameIniciado_01";

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Toda vez que a cena carregar, ele checa se o minigame já foi iniciado
        if (PlayerPrefs.GetInt(chaveMinigame, 0) == 1)
        {
            spriteRenderer.sprite = spriteModificado;
        }
        else
        {
            spriteRenderer.sprite = spriteNormal;
        }
    }
}
