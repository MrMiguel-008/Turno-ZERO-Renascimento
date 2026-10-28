using UnityEngine;

public class Lixeira : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o que entrou na lixeira é uma embalagem
        Embalagem embalagem = other.GetComponent<Embalagem>();

        if (embalagem != null)
        {
            if (embalagem.estaDanificada)
            {
                // Sucesso: Destrói o objeto
                Destroy(other.gameObject);
                Debug.Log("Embalagem danificada reciclada!");
            }
            else
            {
                // Erro: Jogador jogou uma embalagem boa na lixeira
                Debug.Log("Você destruiu uma embalagem boa! Perdeu vida.");
                Destroy(other.gameObject);
            }
        }
    }
}
