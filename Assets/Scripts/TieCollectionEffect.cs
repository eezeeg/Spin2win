using UnityEngine;

public class TieCollectionEffect : MonoBehaviour
{
    [SerializeField] private GameObject sf;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sf != null)
            {
                sf.SetActive(true);
            }
        }
    }
}