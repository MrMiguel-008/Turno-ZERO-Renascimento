using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class AplicarFiltroJogo : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        StartCoroutine(AplicarFiltroComAtraso());
    }

    IEnumerator AplicarFiltroComAtraso()
    {
        yield return null; // Espera a cena carregar 100%

        if (globalVolume == null)
        {
            globalVolume = FindFirstObjectByType<Volume>();
        }

        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Força a ativação dos overrides por código para garantir que o Unity aceite a mudança
            colorAdjustments.hueShift.overrideState = true;
            colorAdjustments.saturation.overrideState = true;

            int modoSalvo = PlayerPrefs.GetInt("ModoDaltonismo", 0);
            AplicarModoDaltonismo(modoSalvo);

            Debug.Log("Filtro aplicado com sucesso! Modo: " + modoSalvo);
        }
        else
        {
            Debug.LogWarning("Global Volume ou Color Adjustments não encontrados nesta cena!");
        }
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