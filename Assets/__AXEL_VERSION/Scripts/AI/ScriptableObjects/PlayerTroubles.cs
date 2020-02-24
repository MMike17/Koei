using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Configuration", menuName = "Koei/Player Configuration")]
public class PlayerTroubles : ScriptableObject
{
    public List<Category> categoriesWeakness;
    public List<SubCategory> subCategoriesWeakness;
}
