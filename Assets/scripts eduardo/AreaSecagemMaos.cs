using UnityEngine;

public class AreaSecagemMaos : MonoBehaviour
{
    private int maosNaArea = 0;

    public bool DuasMaosNaArea()
    {
        return maosNaArea >= 2;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Mao"))
        {
            maosNaArea++;

            if (maosNaArea > 2)
            {
                maosNaArea = 2;
            }

            Debug.Log("Mãos na área de secagem: " + maosNaArea);
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("Mao"))
        {
            maosNaArea--;

            if (maosNaArea < 0)
            {
                maosNaArea = 0;
            }

            Debug.Log("Mãos na área de secagem: " + maosNaArea);
        }
    }
}