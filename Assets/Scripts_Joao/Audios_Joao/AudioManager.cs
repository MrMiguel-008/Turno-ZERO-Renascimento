using UnityEngine;

// A música agora é tocada pelo SoundManager, em todas as cenas. Este componente continua no
// prefab TudoDoMenu só para o prefab não ficar com um script faltando, e não toca mais nada.
public class AudioManager : MonoBehaviour
{
    [Header("Componente de Áudio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Músicas por Cena")]
    [SerializeField] private AudioClip musicaCenaPrincipal;
    [SerializeField] private AudioClip musicaMinigame;
}
