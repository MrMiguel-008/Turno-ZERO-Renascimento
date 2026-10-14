

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EtapaEnxague : MonoBehaviour
{
    public GameObject agua;
    public Slider barraEnxague;
    public SistemaHigiene sistemaHigiene;
    public LegendaHigiene legendaHigiene;

    private bool enxagueAtivo = false;
    private float contador = 0f;

    public float intervalo = 0.08f;

    public void IniciarEnxague()
    {
        if (enxagueAtivo)
            return;

        enxagueAtivo = true;

        barraEnxague.value = 0;
        barraEnxague.gameObject.SetActive(true);

        agua.SetActive(true);

        legendaHigiene.MudarLegenda(
            "Pressione ESPAÇO repetidamente para enxaguar as mãos."
        );

        Debug.Log("Enxágue iniciado!");
    }

    private void Update()
    {
        if (!enxagueAtivo)
            return;

        if (contador > 0)
            contador -= Time.deltaTime;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            contador <= 0)
        {
            barraEnxague.value += 1f;
            contador = intervalo;

            if (barraEnxague.value >= 100)
            {
                barraEnxague.value = 100;
                enxagueAtivo = false;

                barraEnxague.gameObject.SetActive(false);
                agua.SetActive(false);

                legendaHigiene.MudarLegenda(
                    "Leve as mãos para a área de secagem."
                );

                sistemaHigiene.LiberarMaosParaSecagem();

                Debug.Log("Enxágue concluído!");
            }
        }
    }
}

