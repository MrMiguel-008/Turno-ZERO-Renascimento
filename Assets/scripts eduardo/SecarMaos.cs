using UnityEngine;
using UnityEngine.UI;

public class SecarMaos : MonoBehaviour
{
    public Slider barraSecagem;
    public AreaSecagemMaos areaSecagem;

    public float velocidadeSecagem = 10f;

    private bool arrastando = false;
    private bool papelNaArea = false;
    private bool secagemConcluida = false;

    private void Start()
    {
        barraSecagem.value = 0;
        barraSecagem.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        // Se já terminou, não faz mais nada
        if (secagemConcluida)
        {
            return;
        }

        arrastando = true;
    }

    private void OnMouseUp()
    {
        arrastando = false;
    }

    private void Update()
    {
        if (secagemConcluida)
        {
            return;
        }

        if (!arrastando)
        {
            return;
        }

        // As duas mãos precisam estar na área
        if (!areaSecagem.DuasMaosNaArea())
        {
            return;
        }

        // O papel também precisa estar na área
        if (!papelNaArea)
        {
            return;
        }

        // Mostra a barra
        if (!barraSecagem.gameObject.activeSelf)
        {
            barraSecagem.gameObject.SetActive(true);
        }

        // Aumenta a secagem
        barraSecagem.value += velocidadeSecagem * Time.deltaTime;

        if (barraSecagem.value >= 100)
        {
            barraSecagem.value = 100;

            secagemConcluida = true;
            arrastando = false;

            // Esconde a barra
            barraSecagem.gameObject.SetActive(false);

            Debug.Log("Higiene concluída!");
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = true;

            Debug.Log("Papel entrou na área de secagem!");
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = false;

            Debug.Log("Papel saiu da área de secagem!");
        }
    }
}