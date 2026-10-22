
using UnityEngine;
using System.Collections.Generic;

public class AreaSecagemMaos : MonoBehaviour
{
    public SecarMaos secarMaos;

    private HashSet<Collider2D> maosNaArea = new HashSet<Collider2D>();

    public bool DuasMaosNaArea()
    {
        return maosNaArea.Count >= 2;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (!outro.CompareTag("Mao"))
            return;

        maosNaArea.Add(outro);

        Debug.Log("Mãos na área de secagem: " + maosNaArea.Count);

        if (DuasMaosNaArea() && secarMaos != null)
        {
            secarMaos.MostrarLegendaSecagem();
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("Mao"))
        {
            maosNaArea.Remove(outro);

            Debug.Log("Mãos na área de secagem: " + maosNaArea.Count);
        }
    }
}
