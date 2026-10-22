using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DefinirCorPadrao : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    // Função para redefinir para o modo Normal (0)
    public void ResetarParaCorPadrao()
    {
        // Se o volume não foi arrastado no Inspector, tenta encontrá-lo na cena
        if (globalVolume == null)
        {
            globalVolume = FindFirstObjectByType<Volume>();
        }

        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Define o modo no PlayerPrefs como 0 (Normal)
            PlayerPrefs.SetInt("ModoDaltonismo", 0);
            PlayerPrefs.Save();

            // Aplica os valores padrão imediatamente na tela
            colorAdjustments.hueShift.value = 0f;
            colorAdjustments.saturation.value = 0f;

            Debug.Log("Cor redefinida para o Padrão (Normal).");
        }
    }
}