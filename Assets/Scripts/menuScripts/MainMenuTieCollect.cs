using UnityEngine;

public class MainMenuTieCollect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (!LevelsBeatSave.IsMainMenuTieCollected())
        {
            LevelsBeatSave.SaveMainMenuTieCollected();
            RefreshAllCollectionStatsUI();
        }

    }

    private void RefreshAllCollectionStatsUI()
    {
        CollectionStatsUI[] statsUIs = FindObjectsOfType<CollectionStatsUI>(true);

        foreach (CollectionStatsUI statsUI in statsUIs)
        {
            statsUI.Refresh();
        }
    }
}