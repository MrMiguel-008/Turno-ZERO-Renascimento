using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Monta a cena da higienização com as artes da pasta minigameHigienização e deixa todas as referências preenchidas.
// O que ainda não tem arte (sabonete, gotas, seta, borda, água caindo) continua como protótipo.
// Rodar de novo substitui a cena inteira, inclusive o que tiver sido ajustado à mão.
public static class MontarCenaHigiene
{
    private const string CAMINHO_CENA = "Assets/Scenes/Eduardo.unity";
    private const string PASTA_ARTE = "Assets/Artes/minigameHigienização/";
    private const string CAMINHO_LIXEIRA = "Assets/Artes/Bancada Sprites/lixeira.png";
    private const string PASTA_FORMAS = "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/";

    // A profundidade decide qual objeto recebe o clique quando dois ficam um em cima do outro:
    // quem está mais perto da câmera ganha, então as áreas de detecção nunca roubam o clique das mãos e do papel.
    private const float Z_CENARIO = 0f;
    private const float Z_PEDAL = -0.5f;
    private const float Z_MAOS = -1f;
    private const float Z_PAPEL = -2f;
    private const float Z_SETA = -3f;

    // Escalas que deixam cada arte do tamanho certo em relação às outras.
    private const float ESCALA_FUNDO = 2.8f;
    private const float ESCALA_MAOS_E_PAPEL = 0.28f;
    private const float ESCALA_PEDAL = 0.51f;
    private const float ESCALA_SUPORTE = 0.25f;
    // A lixeira vem de outra pasta, importada com outra quantidade de pixels por unidade.
    private const float ESCALA_LIXEIRA = 0.3f;

    private const float DISTANCIA_DAS_MAOS_AO_CENTRO = 1.45f;

    private static readonly Vector2[] POSICOES_GOTAS =
    {
        new Vector2(-0.42f, 0.72f),
        new Vector2(-0.14f, 1.05f),
        new Vector2(0.2f, 0.9f),
        new Vector2(0.3f, 0.35f),
        new Vector2(-0.28f, 0.2f),
        new Vector2(0.05f, 0.02f),
        new Vector2(0.42f, -0.18f),
        new Vector2(-0.3f, -0.32f),
        new Vector2(0.12f, -0.5f),
        new Vector2(-0.12f, -0.85f),
        new Vector2(0.16f, -1.08f)
    };

    private static Sprite quadrado;
    private static Sprite capsula;
    private static Sprite circulo;
    private static Material materialSemLuz;

    [MenuItem("Turno Zero/Montar Cena de Higienização")]
    private static void MontarPeloMenu()
    {
        bool confirmou = EditorUtility.DisplayDialog(
            "Montar cena de higienização",
            "Isso substitui " + CAMINHO_CENA + " inteira, inclusive o que tiver sido ajustado à mão.",
            "Montar",
            "Cancelar");

        if (confirmou && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Montar();
        }
    }

    public static void Montar()
    {
        GarantirTag("Mao");
        GarantirTag("AreaMaos");
        GarantirTag("AreaSecagem");
        GarantirTag("AreaLixo");

        quadrado = CarregarForma("Square.png");
        capsula = CarregarForma("Capsule.png");
        circulo = CarregarForma("Circle.png");
        Sprite triangulo = CarregarForma("Triangle.png");
        materialSemLuz = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");

        Sprite[] quadrosAgua = CarregarQuadros("ligando torneira agua.png");
        Sprite[] quadrosSabao = CarregarQuadros("ligando torneira sabao.png");

        Scene cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CriarCamera();

        // Pia: o fundo é a própria animação da torneira, com a pia, a parede e o piso.
        Transform pia = new GameObject("Pia").transform;
        GameObject fundo = CriarArte("Fundo", pia, new Vector3(0f, 0f, Z_CENARIO), quadrosAgua[0], ESCALA_FUNDO, 0);
        AnimacaoTorneira torneiraAnimada = fundo.AddComponent<AnimacaoTorneira>();
        torneiraAnimada.fundo = fundo.GetComponent<SpriteRenderer>();
        torneiraAnimada.quadrosAgua = quadrosAgua;
        torneiraAnimada.quadrosSabao = quadrosSabao;

        // A água e a torneira ficam na frente das mãos, para a água cair em cima delas.
        GameObject agua = CriarBloco("Agua", pia, new Vector3(0f, -0.17f, Z_CENARIO), 0.22f, 0.92f, new Color(0.25f, 0.62f, 0.95f, 0.85f), 8);
        agua.SetActive(false);
        Transform pontoDaAgua = CriarAlvo("PontoDaAgua", pia, new Vector3(0f, -0.2f, Z_CENARIO));
        CriarArte("Torneira", pia, new Vector3(0f, 1.316f, Z_CENARIO), CarregarArte("torneira recortada.png"), ESCALA_FUNDO, 9);
        CriarBloco("Sabonete", pia, new Vector3(-5.2f, 1.75f, Z_CENARIO), 0.9f, 0.42f, new Color(1f, 0.72f, 0.82f), 3, capsula);

        // Pedal e pé: o pé fica fora da tela e sobe até o pedal a cada pisada. Sem colisor, ele nunca recebe clique.
        GameObject pedal = CriarArte("Pedal", pia, new Vector3(0f, -3.47f, Z_PEDAL), CarregarArte("pedal pia.png"), ESCALA_PEDAL, 3);
        AdicionarColisor(pedal, false);
        Vector3 posicaoPeDescanso = new Vector3(0f, -6.2f, Z_PEDAL);
        Vector3 posicaoPePisando = new Vector3(0f, -3.957f, Z_PEDAL);
        GameObject pe = CriarArte("Pe", pia, posicaoPeDescanso, CarregarArte("pe recortado.png"), ESCALA_PEDAL, 4);

        // Áreas de detecção
        Transform areas = new GameObject("Areas").transform;
        Vector3 centroSecagem = new Vector3(6.4f, 0.45f, Z_CENARIO);
        CriarArea("AreaPia", areas, new Vector3(0f, 0f, Z_CENARIO), 7f, 2f, "AreaMaos");
        GameObject areaSecagem = CriarArea("AreaSecagem", areas, centroSecagem, 4.6f, 3.2f, "AreaSecagem");
        GameObject bordaSecagem = CriarBordas("BordaAreaSecagem", areas, centroSecagem, 4.6f, 3.2f, 0.07f, new Color(0.1f, 0.8f, 0.4f));
        bordaSecagem.SetActive(false);

        GameObject lixeira = CriarArte("Lixeira", areas, new Vector3(6.6f, -3.6f, Z_CENARIO), CarregarSprite(CAMINHO_LIXEIRA), ESCALA_LIXEIRA, 2);
        lixeira.tag = "AreaLixo";
        AdicionarColisor(lixeira, true);

        // Objetos que o jogador arrasta. As mãos começam lado a lado na frente da pia, junto do pedal,
        // como se a pessoa estivesse de pé diante dela. Longe das áreas, para nenhuma entrar sozinha.
        Transform interativos = new GameObject("Interativos").transform;
        ArrastarMao maoEsquerda = CriarMao("MaoEsquerda", interativos, new Vector3(-1.75f, -4.75f, Z_MAOS), "esquerda", -DISTANCIA_DAS_MAOS_AO_CENTRO);
        ArrastarMao maoDireita = CriarMao("MaoDireita", interativos, new Vector3(1.75f, -4.75f, Z_MAOS), "direita", DISTANCIA_DAS_MAOS_AO_CENTRO);

        // O papel começa escondido atrás do suporte, que fica no canto de cima da tela.
        Vector3 papelNoSuporte = new Vector3(6.9f, 4.95f, Z_PAPEL);
        Vector3 papelForaDoSuporte = new Vector3(6.9f, 3.2f, Z_PAPEL);
        GameObject papel = CriarArte("PapelToalha", interativos, papelNoSuporte, CarregarArte("Papel.png"), ESCALA_MAOS_E_PAPEL, 12);
        AdicionarColisor(papel, false);
        AdicionarCorpoCinematico(papel);
        CriarArte("SuportePapel", pia, new Vector3(7.15f, 4.23f, Z_CENARIO), CarregarArte("Suporte papel.png"), ESCALA_SUPORTE, 14);

        // Pontos para onde a seta aponta.
        Transform alvoPia = CriarAlvo("AlvoSetaPia", areas, new Vector3(-DISTANCIA_DAS_MAOS_AO_CENTRO, 1.69f, 0f));
        Transform alvoPedal = CriarAlvo("AlvoSetaPedal", areas, new Vector3(0f, -2.94f, 0f));
        Transform alvoSecagem = CriarAlvo("AlvoSetaSecagem", areas, centroSecagem + new Vector3(0f, 1.6f, 0f));
        Transform alvoLixeira = CriarAlvo("AlvoSetaLixeira", areas, new Vector3(6.6f, -2.85f, 0f));
        Transform alvoPapel = CriarAlvo("AlvoSetaPapel", papel.transform, papelNoSuporte + new Vector3(0f, -0.75f, 0f));

        // Seta
        GameObject objetoSeta = new GameObject("Seta");
        objetoSeta.transform.position = new Vector3(0f, 0f, Z_SETA);
        objetoSeta.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        objetoSeta.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        SpriteRenderer rendererSeta = objetoSeta.AddComponent<SpriteRenderer>();
        rendererSeta.sprite = triangulo;
        rendererSeta.sharedMaterial = materialSemLuz;
        rendererSeta.color = new Color(1f, 0.3f, 0.15f);
        rendererSeta.sortingOrder = 20;
        SetaIndicadora seta = objetoSeta.AddComponent<SetaIndicadora>();

        // Bolhas e respingos
        GameObject objetoBolhas = new GameObject("BolhasDeSabao");
        objetoBolhas.transform.position = new Vector3(0f, 0f, Z_SETA);
        BolhasDeSabao bolhas = objetoBolhas.AddComponent<BolhasDeSabao>();
        bolhas.spriteBolha = circulo;
        bolhas.material = materialSemLuz;

        // Interface
        Transform canvas = CriarCanvas();
        LegendaHigiene legenda = CriarLegenda(canvas);
        Slider barraHigiene = CriarBarra(canvas, "BarraHigiene", "Higiene", new Color(0.3f, 0.75f, 0.35f));
        Slider barraEnxague = CriarBarra(canvas, "BarraEnxague", "Enxágue", new Color(0.2f, 0.55f, 0.95f));
        Slider barraSecagem = CriarBarra(canvas, "BarraSecagem", "Secagem", new Color(0.95f, 0.6f, 0.15f));
        barraEnxague.gameObject.SetActive(false);
        barraSecagem.gameObject.SetActive(false);

        // Scripts e referências
        GameObject objetoSistema = new GameObject("SistemaHigiene");
        SistemaHigiene sistema = objetoSistema.AddComponent<SistemaHigiene>();
        EtapaEnxague enxague = objetoSistema.AddComponent<EtapaEnxague>();
        AnimacaoMaos animacao = objetoSistema.AddComponent<AnimacaoMaos>();

        animacao.sistemaHigiene = sistema;
        animacao.maoEsquerda = maoEsquerda;
        animacao.maoDireita = maoDireita;
        animacao.bolhas = bolhas;
        animacao.pontoDaAgua = pontoDaAgua;
        animacao.aproximacao = 1.25f;
        animacao.aproximacaoEnxague = 0.4f;

        AreaSecagemMaos scriptAreaSecagem = areaSecagem.AddComponent<AreaSecagemMaos>();
        scriptAreaSecagem.sistemaHigiene = sistema;

        ArrastarPapel arrastarPapel = papel.AddComponent<ArrastarPapel>();
        arrastarPapel.sistemaHigiene = sistema;

        SecarMaos secar = papel.AddComponent<SecarMaos>();
        arrastarPapel.secarMaos = secar;
        secar.barraSecagem = barraSecagem;
        secar.sistemaHigiene = sistema;
        secar.arrastarPapel = arrastarPapel;
        secar.alvoDoPapel = alvoPapel;
        secar.spriteAmassado = CarregarArte("papel amassado.png");
        secar.posicaoNoSuporte = papelNoSuporte;
        secar.posicaoForaDoSuporte = papelForaDoSuporte;

        PedalAgua scriptPedal = pedal.AddComponent<PedalAgua>();
        scriptPedal.sistemaHigiene = sistema;
        scriptPedal.etapaEnxague = enxague;
        scriptPedal.pe = pe.transform;
        scriptPedal.posicaoPeDescanso = posicaoPeDescanso;
        scriptPedal.posicaoPePisando = posicaoPePisando;
        scriptPedal.achatamento = 0.92f;

        enxague.barraEnxague = barraEnxague;
        enxague.sistemaHigiene = sistema;

        maoEsquerda.sistemaHigiene = sistema;
        maoDireita.sistemaHigiene = sistema;

        sistema.barraHigiene = barraHigiene;
        sistema.agua = agua;
        sistema.animacaoTorneira = torneiraAnimada;
        sistema.etapaEnxague = enxague;
        sistema.areaSecagem = scriptAreaSecagem;
        sistema.secarMaos = secar;
        sistema.maos = new[] { maoEsquerda, maoDireita };
        sistema.animacaoMaos = animacao;
        sistema.bordaAreaSecagem = bordaSecagem;
        sistema.corAguaLimpa = agua.GetComponent<SpriteRenderer>().color;
        sistema.legendaHigiene = legenda;
        sistema.seta = seta;
        sistema.alvoPia = alvoPia;
        sistema.alvoPedal = alvoPedal;
        sistema.alvoSecagem = alvoSecagem;
        sistema.alvoLixeira = alvoLixeira;

        EditorSceneManager.SaveScene(cena, CAMINHO_CENA);
        Debug.Log("[MontarCenaHigiene] Cena montada em " + CAMINHO_CENA);
    }

    private static void GarantirTag(string tag)
    {
        foreach (string existente in InternalEditorUtility.tags)
        {
            if (existente == tag) return;
        }

        InternalEditorUtility.AddTag(tag);
    }

    private static Sprite CarregarForma(string arquivo)
    {
        return CarregarSprite(PASTA_FORMAS + arquivo);
    }

    private static Sprite CarregarArte(string arquivo)
    {
        return CarregarSprite(PASTA_ARTE + arquivo);
    }

    private static Sprite CarregarSprite(string caminho)
    {
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(caminho).OfType<Sprite>().FirstOrDefault();
        if (sprite == null)
        {
            throw new System.IO.FileNotFoundException("[MontarCenaHigiene] Sprite não encontrado: " + caminho);
        }

        return sprite;
    }

    private static Sprite[] CarregarQuadros(string arquivo)
    {
        Sprite[] quadros = AssetDatabase.LoadAllAssetsAtPath(PASTA_ARTE + arquivo).OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
        if (quadros.Length == 0)
        {
            throw new System.IO.FileNotFoundException("[MontarCenaHigiene] Quadros não encontrados: " + arquivo);
        }

        return quadros;
    }

    private static void CriarCamera()
    {
        GameObject objeto = new GameObject("Main Camera");
        objeto.tag = "MainCamera";
        objeto.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = objeto.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.17f, 0.2f, 0.24f);

        objeto.AddComponent<AudioListener>();
        objeto.AddComponent<UniversalAdditionalCameraData>();
    }

    private static GameObject CriarArte(string nome, Transform pai, Vector3 posicao, Sprite sprite, float escala, int ordem)
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.SetParent(pai, false);
        objeto.transform.position = posicao;
        objeto.transform.localScale = new Vector3(escala, escala, 1f);

        SpriteRenderer renderer = objeto.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sharedMaterial = materialSemLuz;
        renderer.sortingOrder = ordem;

        return objeto;
    }

    private static GameObject CriarBloco(string nome, Transform pai, Vector3 posicao, float largura, float altura, Color cor, int ordem, Sprite sprite = null)
    {
        Sprite spriteUsado = sprite != null ? sprite : quadrado;

        GameObject objeto = CriarArte(nome, pai, posicao, spriteUsado, 1f, ordem);
        objeto.transform.localScale = new Vector3(largura / spriteUsado.bounds.size.x, altura / spriteUsado.bounds.size.y, 1f);
        objeto.GetComponent<SpriteRenderer>().color = cor;

        return objeto;
    }

    private static GameObject CriarArea(string nome, Transform pai, Vector3 posicao, float largura, float altura, string tag)
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.SetParent(pai, false);
        objeto.transform.position = posicao;
        objeto.tag = tag;

        BoxCollider2D colisor = objeto.AddComponent<BoxCollider2D>();
        colisor.size = new Vector2(largura, altura);
        colisor.isTrigger = true;

        return objeto;
    }

    private static void AdicionarColisor(GameObject objeto, bool gatilho)
    {
        BoxCollider2D colisor = objeto.AddComponent<BoxCollider2D>();
        SpriteRenderer renderer = objeto.GetComponent<SpriteRenderer>();
        colisor.size = renderer.sprite.bounds.size;
        colisor.offset = renderer.sprite.bounds.center;
        colisor.isTrigger = gatilho;
    }

    private static void AdicionarCorpoCinematico(GameObject objeto)
    {
        Rigidbody2D corpo = objeto.AddComponent<Rigidbody2D>();
        corpo.bodyType = RigidbodyType2D.Kinematic;
        corpo.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    private static ArrastarMao CriarMao(string nome, Transform pai, Vector3 posicao, string lado, float deslocamentoX)
    {
        GameObject objeto = CriarArte(nome, pai, posicao, CarregarArte("mão " + lado + ".png"), ESCALA_MAOS_E_PAPEL, 5);
        objeto.tag = "Mao";
        AdicionarColisor(objeto, false);
        AdicionarCorpoCinematico(objeto);

        ArrastarMao mao = objeto.AddComponent<ArrastarMao>();
        mao.deslocamentoNoEncaixe = new Vector2(deslocamentoX, 0f);
        mao.visual = objeto.GetComponent<SpriteRenderer>();
        mao.spriteAberta = CarregarArte("mão " + lado + ".png");
        mao.spriteEsfregando = CarregarArte("mao para esfregar " + lado + ".png");
        mao.corAberta = Color.white;
        mao.corEsfregando = Color.white;

        // A espuma é a arte da mão com sabão, por cima da mão limpa. Ela aparece e some pela transparência.
        GameObject espuma = new GameObject("Espuma");
        espuma.transform.SetParent(objeto.transform, false);
        SpriteRenderer rendererEspuma = espuma.AddComponent<SpriteRenderer>();
        rendererEspuma.sprite = CarregarArte("mão " + lado + " sabao.png");
        rendererEspuma.sharedMaterial = materialSemLuz;
        rendererEspuma.color = new Color(1f, 1f, 1f, 0f);
        rendererEspuma.sortingOrder = 6;
        mao.camadasEspuma = new[] { rendererEspuma };

        mao.camadasMolhada = CriarGotas(objeto.transform);
        return mao;
    }

    // Ainda não há arte da mão molhada: as gotas são protótipos, e depois basta colocar o sprite oficial em uma camada só.
    private static SpriteRenderer[] CriarGotas(Transform mao)
    {
        SpriteRenderer[] gotas = new SpriteRenderer[POSICOES_GOTAS.Length];
        Vector3 escalaMao = mao.lossyScale;

        for (int i = 0; i < POSICOES_GOTAS.Length; i++)
        {
            GameObject objeto = new GameObject("Gota");
            objeto.transform.SetParent(mao, false);
            objeto.transform.localPosition = new Vector3(POSICOES_GOTAS[i].x / escalaMao.x, POSICOES_GOTAS[i].y / escalaMao.y, 0f);
            objeto.transform.localScale = new Vector3(0.09f / circulo.bounds.size.x / escalaMao.x, 0.14f / circulo.bounds.size.y / escalaMao.y, 1f);

            SpriteRenderer renderer = objeto.AddComponent<SpriteRenderer>();
            renderer.sprite = circulo;
            renderer.sharedMaterial = materialSemLuz;
            renderer.color = new Color(0.62f, 0.86f, 1f, 0f);
            renderer.sortingOrder = 7;
            gotas[i] = renderer;
        }

        return gotas;
    }

    private static GameObject CriarBordas(string nome, Transform pai, Vector3 centro, float largura, float altura, float espessura, Color cor)
    {
        GameObject grupo = new GameObject(nome);
        grupo.transform.SetParent(pai, false);
        grupo.transform.position = centro;

        float meiaLargura = largura / 2f;
        float meiaAltura = altura / 2f;

        CriarBloco("Cima", grupo.transform, centro + new Vector3(0f, meiaAltura, 0f), largura + espessura, espessura, cor, 2);
        CriarBloco("Baixo", grupo.transform, centro + new Vector3(0f, -meiaAltura, 0f), largura + espessura, espessura, cor, 2);
        CriarBloco("Esquerda", grupo.transform, centro + new Vector3(-meiaLargura, 0f, 0f), espessura, altura + espessura, cor, 2);
        CriarBloco("Direita", grupo.transform, centro + new Vector3(meiaLargura, 0f, 0f), espessura, altura + espessura, cor, 2);

        return grupo;
    }

    private static Transform CriarAlvo(string nome, Transform pai, Vector3 posicaoNoMundo)
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.SetParent(pai, false);
        objeto.transform.position = posicaoNoMundo;
        return objeto.transform;
    }

    private static Transform CriarCanvas()
    {
        GameObject objeto = new GameObject("Canvas");
        objeto.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = objeto.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler escala = objeto.AddComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(1920f, 1080f);
        escala.matchWidthOrHeight = 0.5f;

        return objeto.transform;
    }

    private static LegendaHigiene CriarLegenda(Transform canvas)
    {
        GameObject objeto = new GameObject("Legenda", typeof(RectTransform));
        objeto.layer = canvas.gameObject.layer;
        objeto.transform.SetParent(canvas, false);
        LegendaHigiene legenda = objeto.AddComponent<LegendaHigiene>();

        GameObject objetoTexto = new GameObject("TextoLegenda", typeof(RectTransform));
        objetoTexto.layer = canvas.gameObject.layer;
        objetoTexto.transform.SetParent(canvas, false);

        // A largura deixa o texto longe do suporte do papel, no canto de cima.
        RectTransform retangulo = objetoTexto.GetComponent<RectTransform>();
        retangulo.anchorMin = new Vector2(0.5f, 1f);
        retangulo.anchorMax = new Vector2(0.5f, 1f);
        retangulo.pivot = new Vector2(0.5f, 1f);
        retangulo.anchoredPosition = new Vector2(0f, -18f);
        retangulo.sizeDelta = new Vector2(1100f, 110f);

        TextMeshProUGUI texto = objetoTexto.AddComponent<TextMeshProUGUI>();
        texto.text = "";
        texto.fontSize = 40f;
        texto.fontStyle = FontStyles.Bold;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color(0.12f, 0.14f, 0.18f);
        texto.raycastTarget = false;

        legenda.textoLegenda = texto;
        return legenda;
    }

    private static Slider CriarBarra(Transform canvas, string nome, string rotulo, Color cor)
    {
        DefaultControls.Resources recursos = new DefaultControls.Resources
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
            background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
            knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd")
        };

        GameObject objeto = DefaultControls.CreateSlider(recursos);
        objeto.name = nome;
        objeto.transform.SetParent(canvas, false);

        // Fica no alto, logo abaixo da legenda, porque as mãos começam na parte de baixo da tela.
        RectTransform retangulo = objeto.GetComponent<RectTransform>();
        retangulo.anchorMin = new Vector2(0.5f, 1f);
        retangulo.anchorMax = new Vector2(0.5f, 1f);
        retangulo.pivot = new Vector2(0.5f, 1f);
        retangulo.anchoredPosition = new Vector2(0f, -135f);
        retangulo.sizeDelta = new Vector2(700f, 36f);

        Slider slider = objeto.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 0f;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;

        // A barra só mostra o progresso: o jogador não pode puxá-la, então ela não precisa do pegador.
        Object.DestroyImmediate(slider.handleRect.parent.gameObject);
        slider.handleRect = null;

        slider.fillRect.GetComponent<Image>().color = cor;

        foreach (Graphic grafico in objeto.GetComponentsInChildren<Graphic>(true))
        {
            grafico.raycastTarget = false;
        }

        GameObject objetoRotulo = new GameObject("Rotulo", typeof(RectTransform));
        objetoRotulo.layer = canvas.gameObject.layer;
        objetoRotulo.transform.SetParent(objeto.transform, false);

        RectTransform retanguloRotulo = objetoRotulo.GetComponent<RectTransform>();
        retanguloRotulo.anchorMin = new Vector2(0f, 0.5f);
        retanguloRotulo.anchorMax = new Vector2(0f, 0.5f);
        retanguloRotulo.pivot = new Vector2(1f, 0.5f);
        retanguloRotulo.anchoredPosition = new Vector2(-16f, 0f);
        retanguloRotulo.sizeDelta = new Vector2(240f, 44f);

        TextMeshProUGUI texto = objetoRotulo.AddComponent<TextMeshProUGUI>();
        texto.text = rotulo;
        texto.fontSize = 30f;
        texto.fontStyle = FontStyles.Bold;
        texto.alignment = TextAlignmentOptions.Right;
        texto.color = new Color(0.12f, 0.14f, 0.18f);
        texto.raycastTarget = false;

        return slider;
    }
}
