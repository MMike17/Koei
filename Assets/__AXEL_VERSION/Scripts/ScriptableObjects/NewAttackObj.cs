using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Object", menuName = "Koei/Battle/AttackObj")]
public class NewAttackObj : ScriptableObject
{
    public List<SentencesByCategory> sentencesByCategories = new List<SentencesByCategory>();

    [System.Serializable]
    public class SentencesByCategory
    {
        public Category category;
        [TextArea]
        public string sentence;
        public List<SubCategoryBySentence> subCategoryBySentences = new List<SubCategoryBySentence>();
    }

    [System.Serializable]
    public class SubCategoryBySentence
    {
        public SubCategory subcategory;
    }
}
