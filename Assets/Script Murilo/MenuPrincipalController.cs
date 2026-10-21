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
        // Garante que o jogo comece na velocidade normal
        Time.timeScale = 1f;

        // Pega o modo salvo previamente (padrão é 0 - Normal)
        modoAtual = PlayerPrefs.GetInt("ModoDaltonismo", 0);

        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            AplicarModoDaltonismo(modoAtual);
        }
    }

    public void BotaoJogar()
    {
        PlayerPrefs.SetInt("CarregarUltimoSalvamento", 1);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene("JogoPrincipal");
    }

    public void AbrirConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);
    }

    public void AlternarDaltonismo()
    {
        // Avança o modo de 0 a 3 e volta para 0
        modoAtual = (modoAtual + 1) % 4;

        PlayerPrefs.SetInt("ModoDaltonismo", modoAtual);
        PlayerPrefs.Save();

        // Se por algum motivo o colorAdjustments não tiver sido pego no Start, tenta pegar agora
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