
using UnityEngine;

public class PedalAgua : MonoBehaviour
{
    public GameObject agua;
    public SistemaHigiene sistemaHigiene;
    public EtapaEnxague etapaEnxague;
    public LegendaHigiene legendaHigiene;

    private bool aguaLigada = false;

    private void OnMouseDown()
    {
        if (!sistemaHigiene.MaosEstaoNaPia())
        {
            return;
        }

        // Antes de terminar a lavagem, o pedal liga/desliga a água.
        if (!sistemaHigiene.LavagemEstaConcluida())
        {
            aguaLigada = !aguaLigada;

            agua.SetActive(aguaLigada);
            sistemaHigiene.DefinirAgua(aguaLigada);

            return;
        }

        // Depois da lavagem, o pedal inicia o enxágue.
        if (!etapaEnxague.enabled)
        {
            return;
        }

        etapaEnxague.IniciarEnxague();

        Debug.Log("Enxágue iniciado!");
    }
}
