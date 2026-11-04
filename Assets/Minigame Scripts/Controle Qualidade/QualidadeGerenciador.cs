using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QualidadeGerenciador : MonoBehaviour
{
    public static QualidadeGerenciador Instancia;

    [Header("Configurações do Ciclo")]
    public int totalCiclos = 3;
    public float tempoMovendoEsteira = 4f;

    [Header("Prefabs e Spawner")]
    public List<Transform> pontosDeSpawn = new List<Transform>();
    public float intervaloSpawn = 0.8f;

    [Header("Categorias de Embalagens")]
    public List<GameObject> prefabsNormais = new List<GameObject>();
    public List<GameObject> prefabsDanificadas = new List<GameObject>();

    private int cicloAtual = 0;
    private bool esteiraAtiva = false;
    private List<Embalagem> embalagensNaCena = new List<Embalagem>();

    private bool danificadaGeradaNesteCiclo = false;

    void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (pontosDeSpawn.Count == 0 || prefabsNormais.Count == 0 || prefabsDanificadas.Count == 0)
        {
            Debug.LogError("Por favor, configure os Pontos de Spawn e as duas listas de Prefabs no Inspetor!");
            return;
        }

        StartCoroutine(FluxoDoMinigame());
    }

    IEnumerator FluxoDoMinigame()
    {
        for (cicloAtual = 1; cicloAtual <= totalCiclos; cicloAtual++)
        {
            Debug.Log($"--- INICIANDO CICLO {cicloAtual}/{totalCiclos} ---");

            danificadaGeradaNesteCiclo = false;
            esteiraAtiva = true;
            DefinirMovimentoDasEmbalagens(true);
            SoundManager.DefinirEsteira(true);

            Coroutine rotinaSpawn = StartCoroutine(SpawnarEmbalagens());

            yield return new WaitForSeconds(tempoMovendoEsteira);

            StopCoroutine(rotinaSpawn);

            esteiraAtiva = false;
            DefinirMovimentoDasEmbalagens(false);
            SoundManager.DefinirEsteira(false);
            Debug.Log("Esteira parada! Remova as embalagens danificadas.");

            yield return new WaitUntil(() => TodasDanificadasForamRemovidas());

            Debug.Log($"Ciclo {cicloAtual} finalizado!");
            yield return new WaitForSeconds(1.5f);
        }

        Debug.Log("Minigame Totalmente Concluído com Sucesso!");

        // No final do método FluxoDoMinigame() em QualidadeGerenciador.cs:
        if (QuestManager.Instance != null)
        {
            // Substitui exatamente pelo ID que está no teu Main Quest Database
            QuestManager.Instance.CheckMinigameQuest("qualidade");
        }
        else
        {
            Debug.LogWarning("[QualidadeGerenciador] QuestManager não encontrado na cena!");
        }

        Debug.Log("Minigame Totalmente Concluído com Sucesso!");

        // Informa o InteracaoMinigame que o objetivo foi concluído com sucesso e retorna à cena original
        InteracaoMinigame.FinalizarMinigame(true);
    }

    IEnumerator SpawnarEmbalagens()
    {
        int totalSpawnsNesteCiclo = 0;
        int maxSpawnsEstimados = Mathf.FloorToInt(tempoMovendoEsteira / intervaloSpawn);

        while (esteiraAtiva)
        {
            totalSpawnsNesteCiclo++;
            GameObject prefabEscolhido = null;

            if (!danificadaGeradaNesteCiclo && totalSpawnsNesteCiclo >= maxSpawnsEstimados)
            {
                int randomIndex = Random.Range(0, prefabsDanificadas.Count);
                prefabEscolhido = prefabsDanificadas[randomIndex];
                danificadaGeradaNesteCiclo = true;
            }
            else
            {
                bool sortearDanificada = Random.Range(0f, 1f) < 0.3f;

                if (sortearDanificada)
                {
                    int randomIndex = Random.Range(0, prefabsDanificadas.Count);
                    prefabEscolhido = prefabsDanificadas[randomIndex];
                    danificadaGeradaNesteCiclo = true;
                }
                else
                {
                    int randomIndex = Random.Range(0, prefabsNormais.Count);
                    prefabEscolhido = prefabsNormais[randomIndex];
                }
            }

            int randomPontoIndex = Random.Range(0, pontosDeSpawn.Count);
            Transform pontoEscolhido = pontosDeSpawn[randomPontoIndex];

            if (prefabEscolhido != null)
            {
                GameObject objInstanciado = Instantiate(prefabEscolhido, pontoEscolhido.position, pontoEscolhido.rotation);

                Embalagem scriptEmbalagem = objInstanciado.GetComponent<Embalagem>();
                if (scriptEmbalagem != null)
                {
                    embalagensNaCena.Add(scriptEmbalagem);
                    scriptEmbalagem.DefinirMovimento(esteiraAtiva);
                }
            }

            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    void DefinirMovimentoDasEmbalagens(bool mover)
    {
        embalagensNaCena.RemoveAll(item => item == null);
        foreach (var embalagem in embalagensNaCena)
        {
            if (embalagem != null)
            {
                embalagem.DefinirMovimento(mover);
            }
        }
    }

    bool TodasDanificadasForamRemovidas()
    {
        embalagensNaCena.RemoveAll(item => item == null);
        foreach (var embalagem in embalagensNaCena)
        {
            if (embalagem != null && embalagem.estaDanificada)
            {
                return false;
            }
        }
        return true;
    }

    public void RemoverDaLista(Embalagem emb)
    {
        if (embalagensNaCena.Contains(emb))
        {
            embalagensNaCena.Remove(emb);
        }
    }

    public bool IsEsteiraAtiva()
    {
        return esteiraAtiva;
    }


}