using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerTesteMovimento : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 3f;

    [Header("Configuração de Reinício")]
    [Tooltip("Marque se quiser que o player SEMPRE volte com a roupa 'Antes' ao iniciar/reiniciar a cena.")]
    [SerializeField] private bool forcarRoupaInicialNoStart = false;

    [Header("Customização Pós-Minigame (Animações)")]
    [Tooltip("Arraste aqui o Animator Controller com o visual ANTES do minigame.")]
    [SerializeField] private RuntimeAnimatorController controllerAntes;

    [Tooltip("Arraste aqui o Animator Controller com o visual DEPOIS do minigame.")]
    [SerializeField] private RuntimeAnimatorController controllerDepois;

    [Header("Customização Pós-Minigame (Sprites Base)")]
    [Tooltip("O sprite padrão do player com a roupa antiga (usado como fallback).")]
    [SerializeField] private Sprite spriteBaseAntes;

    [Tooltip("O sprite inicial do player com a ROUPA NOVA. Evita que o player pisque com a roupa antiga.")]
    [SerializeField] private Sprite spriteBaseDepois;

    [Header("Vínculo com o Minigame")]
    [Tooltip("O ID exato (idObjetivo) configurado no InteracaoMinigame.")]
    [SerializeField] private string idMinigameVisual = "objetivo_01";

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        AtualizarControladorVisual();
    }

    private void Start()
    {
        // Se ativado no Inspector, limpa o estado do minigame ao iniciar a cena para forçar a skin antiga
        if (forcarRoupaInicialNoStart)
        {
            // Se você estiver salvando o progresso usando PlayerPrefs, podemos apagar a chave dele aqui:
            // PlayerPrefs.DeleteKey(idMinigameVisual);

            // Força o visual de "Antes" diretamente
            AplicarVisualAntes();
        }
        else
        {
            AtualizarControladorVisual();
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movimento = new Vector2(horizontal, vertical).normalized;

        transform.position += (Vector3)(movimento * velocidade * Time.deltaTime);

        if (animator != null)
        {
            bool estaSeMovendo = (movimento != Vector2.zero);
            animator.SetBool("IsWalking", estaSeMovendo);
        }

        if (horizontal > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (horizontal < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public void AtualizarControladorVisual()
    {
        if (animator == null || spriteRenderer == null) return;

        bool oMinigameFoiConcluido = InteracaoMinigame.ObjetivoFoiConcluido(idMinigameVisual);

        if (oMinigameFoiConcluido)
        {
            AplicarVisualDepois();
        }
        else
        {
            AplicarVisualAntes();
        }
    }

    private void AplicarVisualAntes()
    {
        if (spriteBaseAntes != null)
        {
            spriteRenderer.sprite = spriteBaseAntes;
        }

        if (controllerAntes != null && animator.runtimeAnimatorController != controllerAntes)
        {
            animator.runtimeAnimatorController = controllerAntes;
            animator.Rebind();
            animator.Update(0f);
        }
    }

    private void AplicarVisualDepois()
    {
        if (spriteBaseDepois != null)
        {
            spriteRenderer.sprite = spriteBaseDepois;
        }

        if (controllerDepois != null && animator.runtimeAnimatorController != controllerDepois)
        {
            animator.runtimeAnimatorController = controllerDepois;
            animator.Rebind();
            animator.Update(0f);
        }

        Debug.Log($"[PlayerVisual] Roupa Nova Aplicada! Sprite Base e Animator atualizados.");
    }
}