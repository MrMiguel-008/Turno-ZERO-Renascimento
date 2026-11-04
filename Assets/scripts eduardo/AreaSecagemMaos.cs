using System.Collections.Generic;
using UnityEngine;

// O HashSet impede contar a mesma mão duas vezes, e a saída tira a mão da contagem.
public class AreaSecagemMaos : MonoBehaviour
{
    public SistemaHigiene sistemaHigiene;

    private readonly HashSet<Collider2D> maosNaArea = new HashSet<Collider2D>();

    public bool DuasMaosNaArea()
    {
        return maosNaArea.Count >= 2;
    }

    public void VerificarMaos()
    {
        if (DuasMaosNaArea())
        {
            sistemaHigiene.MaosChegaramNaSecagem();
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Mao") && maosNaArea.Add(outro))
        {
            VerificarMaos();
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("Mao"))
        {
            maosNaArea.Remove(outro);
        }
    }
}
