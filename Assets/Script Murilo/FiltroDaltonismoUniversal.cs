using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FiltroDaltonismoUniversal : MonoBehaviour
{
    IEnumerator Start()
    {
        // Aguarda um frame para garantir que a cena e a câmera carregaram completamente
        yield return null;

        // Acha o Global Volume automaticamente na cena
        Volume volume = FindFirstObjectByType<Volume>();
        if (volume == null || volume.profile == null)
        {
            Debug.LogWarning("Nenhum Global Volume encontrado nesta cena!");
            yield break;
        }

        // Tenta pegar o Color Adjustments
        ColorAdjustments colorAdj;
        if (!volume.profile.TryGet(out colorAdj))
        {
            // Se não existir, adiciona automaticamente no perfil da cena!
            colorAdj = volume.profile.Add<ColorAdjustments>(true);
        }

        // Força a ativação das permissões de Hue e Saturação
        colorAdj.hueShift.overrideState = true;
        colorAdj.saturation.overrideState = true;

        // Pega o modo salvo no menu (0 = Normal, 1 = Protanopia, 2 = Deuteranopia, 3 = Tritanopia)
        int modo = PlayerPrefs.GetInt("ModoDaltonismo", 0);

        // Aplica os valores com força total
        switch (modo)
        {
            case 0:
                colorAdj.hueShift.value = 0f;
                colorAdj.saturation.value = 0f;
                break;
            case 1: // Protanopia
                colorAdj.hueShift.value = -90f;
                colorAdj.saturation.value = 30f;
                break;
            case 2: // Deuteranopia
                colorAdj.hueShift.value = 90f;
                colorAdj.saturation.value = 30f;
                break;
            case 3: // Tritanopia
                colorAdj.hueShift.value = -50f;
                colorAdj.saturation.value = 20f;
                break;
        }

        Debug.Log("Filtro Universal aplicado com sucesso para o modo: " + modo);
    }
}