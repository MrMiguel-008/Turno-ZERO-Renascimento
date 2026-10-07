using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EtapaEnxague : MonoBehaviour
{
    public GameObject agua;
    public Slider barraEnxague;
    public SistemaHigiene sistemaHigiene;

    private bool enxagueAtivo = false;
    private float contador = 0f;

    public float intervalo = 0.08f;

    public void IniciarEnxague()
    {
        enxagueAtivo = true;

        // Mostra a barra de enxágue
        barraEnxague.gameObject.SetActive(true);

        // Liga a água
        agua.SetActive(true);

        Debug.Log("Enxágue iniciado!");
    }

    private void Update()
    {
        if (!enxagueAtivo)
        {
            return;
        }

        if (contador > 0)
        {
            contador -= Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && contador <= 0)
        {
            barraEnxague.value += 1f;
            contador = intervalo;

            if (barraEnxague.value >= 100)
            {
                barraEnxague.value = 100;

                enxagueAtivo = false;

                // Esconde a barra
                barraEnxague.gameObject.SetActive(false);

                // Desliga a água
                agua.SetActive(false);

                Debug.Log("Enxágue concluído!");

                // Libera as mãos para serem arrastadas novamente
                sistemaHigiene.LiberarMaosParaSecagem();
            }
        }
    }
}