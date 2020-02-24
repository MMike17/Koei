using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Troubles", menuName = "Koei/Player Troubles")]
public class PlayerTroubles : ScriptableObject
{
    public List<Category> categoriesWeakness;
    public List<SubCategory> subCategoriesWeakness;
}
