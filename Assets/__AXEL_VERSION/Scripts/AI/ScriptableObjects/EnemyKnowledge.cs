using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "Koei/Enemy Configuration")]
public class EnemyKnowledge : ScriptableObject
{
    public List<Category> playerKnewWeaknessCategory;
    public List<SubCategory> playerKnewWeaknessSubcategory;
}
