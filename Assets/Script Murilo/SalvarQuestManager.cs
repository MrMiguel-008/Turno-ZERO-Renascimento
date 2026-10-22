using UnityEngine;

public class SalvarQuestManager : MonoBehaviour
{
    void Start()
    {
        // Quando a cena do jogo carrega, recupera o ID da missão ativa e aplica-o ao QuestManager
        if (PlayerPrefs.HasKey("QuestAtivaID"))
        {
            string questSalva = PlayerPrefs.GetString("QuestAtivaID");

            // Atribui diretamente à variável do QuestManager
            if (GetComponent<QuestManager>() != null)
            {
                GetComponent<QuestManager>().activeQuestID = questSalva;
            }
        }
    }

    void OnDisable()
    {
        // Quando o jogo vai para o menu (mudança de cena), guarda a missão ativa atual
        if (GetComponent<QuestManager>() != null)
        {
            string questAtual = GetComponent<QuestManager>().activeQuestID;
            if (!string.IsNullOrEmpty(questAtual))
            {
                PlayerPrefs.SetString("QuestAtivaID", questAtual);
                PlayerPrefs.Save();
            }
        }
    }
}