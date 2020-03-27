using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpicBattleManager : MonoBehaviour
{
    public NewAttackObj allAttacks;
    public Transform parent;
    public GameObject attackBtt;

    private List<GameObject> bttAttacks = new List<GameObject>();

    private void Start()
    {
        MakeSpawnButtons();
    }

    public void MakeSpawnButtons()
    {
        if (FightSettings.currentHpEnemy > 0)
        {
            bttAttacks.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));

            if (parent.childCount != 0)
            {
                for (int btt = 0; btt < parent.childCount; btt++)
                {
                    Destroy(bttAttacks[btt]);
                    Debug.Log(btt);
                }
            }

            #region Spawn Buttons
            

            for (int i = 0; i < allAttacks.sentencesByCategories.Count; i++)
            {
                SpawnButton(i);
            }

            #endregion


            bttAttacks.Clear();
        }
    }

    private void SpawnButton(int iteration)
    {
        GameObject button = Instantiate(attackBtt);
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;
        button.GetComponentInChildren<TextMeshProUGUI>().text = allAttacks.sentencesByCategories[iteration].sentence;
    }
}
