using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    public static GameScoreManager Instance;

    private static float pontuaTotalAcumulada = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void AdicionarPontosMinigame(string nomeMinigame, float pontos)
    {
        pontuaTotalAcumulada += pontos;
        Debug.Log($"[GameScoreManager] Pontos adicionados de {nomeMinigame}: {pontos}. Total acumulado: {pontuaTotalAcumulada}");
    }

    public static float GetPontuacaoTotal()
    {
        return pontuaTotalAcumulada;
    }

    public static void ResetarPontuacao()
    {
        pontuaTotalAcumulada = 0f;
    }
}