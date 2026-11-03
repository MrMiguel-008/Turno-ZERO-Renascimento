using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MenuPrincipalController : MonoBehaviour
{
    [SerializeField] private GameObject painelConfiguracoes;
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private int modoAtual = 0;

    void Start()
    {
        Time.timeScale = 1f;
        modoAtual = PlayerPrefs.GetInt("ModoDaltonismo", 0);

        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            AplicarModoDaltonismo(modoAtual);
        }
    }

    public void BotaoJogar()
    {
        SoundManager.Tocar(Som.CliqueBotao);

        // Retoma o tempo do jogo caso estivesse pausado
        Time.timeScale = 1f;

        // Apenas carrega a cena do jogo sem apagar nenhuma coordenada do jogador
        SceneManager.LoadScene("JogoPrincipal");
    }

    public void AbrirConfiguracoes()
    {
        SoundManager.Tocar(Som.CliqueBotao);

        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        SoundManager.Tocar(Som.CliqueBotao);

        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);
    }

    public void AlternarDaltonismo()
    {
        SoundManager.Tocar(Som.CliqueBotao);

        modoAtual = (modoAtual + 1) % 4;
        PlayerPrefs.SetInt("ModoDaltonismo", modoAtual);
        PlayerPrefs.Save();

        if (colorAdjustments == null && globalVolume != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
        }

        AplicarModoDaltonismo(modoAtual);
    }

    private void AplicarModoDaltonismo(int tipoModo)
    {
        if (colorAdjustments == null) return;

        switch (tipoModo)
        {
            case 0: // Normal
                colorAdjustments.hueShift.value = 0f;
                colorAdjustments.saturation.value = 0f;
                break;
            case 1: // Protanopia
                colorAdjustments.hueShift.value = -20f;
                colorAdjustments.saturation.value = 10f;
                break;
            case 2: // Deuteranopia
                colorAdjustments.hueShift.value = 20f;
                colorAdjustments.saturation.value = 15f;
                break;
            case 3: // Tritanopia
                colorAdjustments.hueShift.value = -50f;
                colorAdjustments.saturation.value = 5f;
                break;
        }
    }
}