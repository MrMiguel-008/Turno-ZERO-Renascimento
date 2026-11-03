using UnityEngine;

public enum Som
{
    EsfregarTapete,
    Bolhas,
    InteracaoItem,
    SoltarItem,
    CliqueBotao,
    MensagemSupervisora,
    ColetaItem,
    ColetaProducao,
    ColetaBiscoito,
    Lixo,
    EncaixeBancada,
    BipCaixa,
    Acerto,
    Erro,
    Vitoria
}

// Lista de todos os sons do jogo. Para colocar um som, selecione Assets/Resources/BibliotecaSons
// e arraste o arquivo de áudio para o campo dele. Campo vazio = fica sem som, sem dar erro.
[CreateAssetMenu(fileName = "BibliotecaSons", menuName = "Audio/Biblioteca de Sons")]
public class BibliotecaSons : ScriptableObject
{
    [System.Serializable]
    public class Entrada
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [System.Serializable]
    public class MusicaDeCena : Entrada
    {
        [Tooltip("Nome exato da cena.")]
        public string cena;
    }

    [Header("Volumes gerais")]
    [Range(0f, 1f)] public float volumeMusica = 0.35f;
    [Range(0f, 1f)] public float volumeAmbiente = 0.25f;
    [Range(0f, 1f)] public float volumeEfeitos = 0.9f;

    [Header("Música de fundo")]
    public Entrada musicaMenu = new Entrada();
    [Tooltip("Música das fases tranquilas. Só toca nas cenas da lista abaixo.")]
    public Entrada musicaJogo = new Entrada();
    [Tooltip("Cenas que tocam a Musica Jogo, com o nome exato da cena. As outras ficam sem música.")]
    public string[] cenasComMusica = new string[0];
    [Tooltip("Cenas com uma música só delas. Têm prioridade sobre a lista acima.")]
    public MusicaDeCena[] musicasDeCena = new MusicaDeCena[0];

    [Header("1. Movimentação e interação básica")]
    [Tooltip("Variações de um passo. Cada passo sorteia uma delas, sem repetir a anterior.")]
    public AudioClip[] passos;
    [Range(0f, 1f)] public float volumePassos = 0.45f;
    [Tooltip("Segundos entre um passo e outro enquanto o jogador anda.")]
    [Range(0.15f, 0.8f)] public float intervaloPassos = 0.32f;
    [Tooltip("Cada acerto de tecla no minigame do tapete.")]
    public Entrada esfregarTapete = new Entrada();
    [Tooltip("Bolhas do tapete e da estação de lavagem das mãos.")]
    public Entrada bolhas = new Entrada();
    [Tooltip("Clicar, pegar ou soltar um item e abrir uma estação com E.")]
    public Entrada interacaoItem = new Entrada();
    [Tooltip("Largar um item sem encaixá-lo em nenhum lugar: embalagem na esteira, scanner ou fita fora da caixa, papel-toalha.")]
    public Entrada soltarItem = new Entrada();
    [Tooltip("Botões do menu e da tela de fim de jogo.")]
    public Entrada cliqueBotao = new Entrada();
    [Tooltip("Cada fala da supervisora que aparece na tela.")]
    public Entrada mensagemSupervisora = new Entrada();

    [Header("2. Coleta e manipulação de itens")]
    [Tooltip("Coleta dos EPIs e demais itens do inventário.")]
    public Entrada coletaItem = new Entrada();
    [Tooltip("Coleta dos ingredientes: açúcar, água, farinha, gordura e gotas.")]
    public Entrada coletaProducao = new Entrada();
    [Tooltip("Pegar um biscoito na esteira do controle de qualidade.")]
    public Entrada coletaBiscoito = new Entrada();
    [Tooltip("Qualquer item jogado na lixeira.")]
    public Entrada lixo = new Entrada();

    [Header("3. Linha de produção e bancada")]
    [Tooltip("Opcional. Loop de fundo da fábrica em todas as cenas, menos no menu. Vazio = sem som de fundo.")]
    public Entrada ambienteIndustrial = new Entrada();
    [Tooltip("Toca uma vez inteira quando a produção é iniciada.")]
    public Entrada producaoFuncionando = new Entrada();
    [Tooltip("Item colocado no lugar certo na bancada, nos EPIs e na mesa.")]
    public Entrada encaixeBancada = new Entrada();
    [Tooltip("Loop: enquanto a esteira do controle de qualidade anda.")]
    public Entrada esteiraQualidade = new Entrada();
    [Tooltip("Scanner levado até a caixa na mesa de controle.")]
    public Entrada bipCaixa = new Entrada();

    [Header("4. Feedback de gameplay")]
    [Tooltip("Missão concluída e embalagem danificada descartada.")]
    public Entrada acerto = new Entrada();
    [Tooltip("Ação errada nos minigames.")]
    public Entrada erro = new Entrada();
    [Tooltip("Tela de fim de jogo.")]
    public Entrada vitoria = new Entrada();

    public Entrada Obter(Som som)
    {
        switch (som)
        {
            case Som.EsfregarTapete: return esfregarTapete;
            case Som.Bolhas: return bolhas;
            case Som.InteracaoItem: return interacaoItem;
            case Som.SoltarItem: return soltarItem;
            case Som.CliqueBotao: return cliqueBotao;
            case Som.MensagemSupervisora: return mensagemSupervisora;
            case Som.ColetaItem: return coletaItem;
            case Som.ColetaProducao: return coletaProducao;
            case Som.ColetaBiscoito: return coletaBiscoito;
            case Som.Lixo: return lixo;
            case Som.EncaixeBancada: return encaixeBancada;
            case Som.BipCaixa: return bipCaixa;
            case Som.Acerto: return acerto;
            case Som.Erro: return erro;
            case Som.Vitoria: return vitoria;
            default: return null;
        }
    }
}
