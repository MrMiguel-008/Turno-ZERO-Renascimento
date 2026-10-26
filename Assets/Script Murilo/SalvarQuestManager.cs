using UnityEngine;
using System.Collections;

public class SalvarQuestManager : MonoBehaviour
{
    private QuestManager questManagerRef;

    void Awake()
    {
        questManagerRef = GetComponent<QuestManager>();
    }

    void Start()
    {
        // Usa uma corrotina para garantir que aplicamos a missão salva 
        // APÓS todos os Start() originais do jogo correrem
        StartCoroutine(RestaurarQuestAtrasada());
    }

    IEnumerator RestaurarQuestAtrasada()
    {
        // Espera um frame para garantir que o QuestManager já inicializou tudo dele
        yield return null;

        if (PlayerPrefs.HasKey("QuestAtivaID") && questManagerRef != null)
        {
            string questSalva = PlayerPrefs.GetString("QuestAtivaID");
            questManagerRef.activeQuestID = questSalva;
            
            // Se o teu QuestManager tiver alguma função para atualizar a UI da missão, 
            // podes chamá-la aqui se souberes o nome, por exemplo:
            // questManagerRef.AtualizarInterfaceMissao();
        }
    }

    void OnDisable()
    {
        // Guarda a missão ativa imediatamente antes de sair para o menu
        if (questManagerRef != null && !string.IsNullOrEmpty(questManagerRef.activeQuestID))
        {
            PlayerPrefs.SetString("QuestAtivaID", questManagerRef.activeQuestID);
            PlayerPrefs.Save();
        }
    }
}