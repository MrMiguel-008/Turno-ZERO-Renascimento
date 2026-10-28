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

    // ALTERADO: Separamos os prefabs em duas listas para o gerenciador poder escolher de forma inteligente
    [Header("Categorias de Embalagens")]
    public List<GameObject> prefabsNormais = new List<GameObject>();
    public List<GameObject> prefabsDanificadas = new List<GameObject>();

    private int cicloAtual = 0;
    private bool esteiraAtiva = false;
    private List<Embalagem> embalagensNaCena = new List<Embalagem>();

    // Controle interno do requisito do ciclo
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
            Debug.LogError("Por favor, configure os Pontos de Spawn e as duas listas de Prefabs (Normais e Danificadas) no Inspetor!");
            return;
        }

        StartCoroutine(FluxoDoMinigame());
    }

    // 1. CONTROLADOR DOS TURNOS / CICLOS
    IEnumerator FluxoDoMinigame()
    {
        for (cicloAtual = 1; cicloAtual <= totalCiclos; cicloAtual++)
        {
            Debug.Log($"--- INICIANDO CICLO {cicloAtual}/{totalCiclos} ---");

            // Reseta a checagem para o novo ciclo
            danificadaGeradaNesteCiclo = false;

            esteiraAtiva = true;
            DefinirMovimentoDasEmbalagens(true);

            Coroutine rotinaSpawn = StartCoroutine(SpawnarEmbalagens());

            yield return new WaitForSeconds(tempoMovendoEsteira);

            StopCoroutine(rotinaSpawn);

            esteiraAtiva = false;
            DefinirMovimentoDasEmbalagens(false);
            Debug.Log("Esteira parada! Remova as embalagens danificadas.");

            yield return new WaitUntil(() => TodasDanificadasForamRemovidas());

            Debug.Log($"Ciclo {cicloAtual} finalizado!");
            yield return new WaitForSeconds(1.5f);
        }

        Debug.Log("Minigame Totalmente Concluído com Sucesso!");
        SceneManager.LoadScene(1);
    }

    // 2. ROTINA DE SPAWN ALEATÓRIO E INTELIGENTE
    IEnumerator SpawnarEmbalagens()
    {
        int totalSpawnsNesteCiclo = 0;
        // Calcula aproximadamente quantos itens vão spawnar baseado no tempo total e no intervalo
        int maxSpawnsEstimados = Mathf.FloorToInt(tempoMovendoEsteira / intervaloSpawn);

        while (esteiraAtiva)
        {
            totalSpawnsNesteCiclo++;
            GameObject prefabEscolhido = null;

            // REGRA DA EMBALAGEM DANIFICADA:
            // Se for o ÚLTIMO spawn possível do ciclo e nenhuma danificada nasceu ainda, FORÇA uma danificada.
            if (!danificadaGeradaNesteCiclo && totalSpawnsNesteCiclo >= maxSpawnsEstimados)
            {
                int randomIndex = Random.Range(0, prefabsDanificadas.Count);
                prefabEscolhido = prefabsDanificadas[randomIndex];
                danificadaGeradaNesteCiclo = true;
                Debug.Log("Sorteio Forçado: Garantindo a embalagem danificada do ciclo!");
            }
            else
            {
                // Caso contrário, deixa o jogo sortear aleatoriamente entre normal ou danificada (chance de 30% de vir danificada naturalmente)
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

            // Seleção de um dos 5 pontos
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

    // 3. ATIVA OU DESATIVA O MOVIMENTO DE TODAS AS EMBALAGENS PRESENTES
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

    // 4. CHECA SE AINDA EXISTE ALGUMA EMBALAGEM DANIFICADA NA TELA
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

    // 5. REMOVE A REFERÊNCIA DA CAIXA QUANDO ELA É DESTRUÍDA
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
