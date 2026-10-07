using UnityEngine;
using UnityEngine.UI;

public class SecarMaos : MonoBehaviour
{
    public Slider barraSecagem;

    private bool estaNaArea = false;
    private Vector3 ultimaPosicao;
    private float distanciaAcumulada = 0f;

    private void Start()
    {
        ultimaPosicao = transform.position;
    }

    private void Update()
    {
        // Calcula quanto o papel se movimentou
        float distancia = Vector3.Distance(transform.position, ultimaPosicao);

        if (estaNaArea)
        {
            distanciaAcumulada += distancia;

            // A cada determinada distância, aumenta a secagem
            if (distanciaAcumulada >= 0.05f)
            {
                barraSecagem.value += 1f;
                distanciaAcumulada = 0f;

                if (barraSecagem.value >= 100)
                {
                    barraSecagem.value = 100;

                    Debug.Log("Mãos secas!");
                }
            }
        }

        ultimaPosicao = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            estaNaArea = true;

            Debug.Log("Papel chegou nas mãos!");
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            estaNaArea = false;

            Debug.Log("Papel saiu das mãos!");
        }
    }
}