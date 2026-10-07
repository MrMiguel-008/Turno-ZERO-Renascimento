using UnityEngine;

public class PedalAgua : MonoBehaviour
{
    public GameObject agua;
    public SistemaHigiene sistemaHigiene;
    public EtapaEnxague etapaEnxague;

    private bool aguaLigada = false;

    private void OnMouseDown()
    {
        // Só permite usar o pedal quando as duas mãos estiverem na pia
        if (!sistemaHigiene.MaosEstaoNaPia())
        {
            return;
        }

        // Se a lavagem ainda não terminou,
        // funciona normalmente para ligar/desligar a água
        if (sistemaHigiene.LavagemEstaConcluida() == false)
        {
            aguaLigada = !aguaLigada;

            agua.SetActive(aguaLigada);

            sistemaHigiene.DefinirAgua(aguaLigada);

            return;
        }

        // Se a lavagem terminou, inicia o enxágue
        etapaEnxague.IniciarEnxague();

        Debug.Log("Enxágue iniciado!");
    }
}