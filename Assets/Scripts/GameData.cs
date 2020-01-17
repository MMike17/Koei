using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour, IDebugable
{
    [Header("Assign in Inspector")]
    public List<Card> cardList;

    [Header("Debug")]
    public PlayerData playerData;

    public IDebugable debugableInterface => (IDebugable) this;
    public string debugLabel => "<b>[GameData] : </b>";

    public static SubCategory CorrectSubCategory(SubCategory sub, Category parent)
    {
        List<string> listOfSubCategories = new List<string>();

        foreach (string line in Enum.GetNames(typeof(SubCategory)))
            listOfSubCategories.Add(line);

        string compare = parent.ToString();

        string selected = listOfSubCategories.Find(item => { return item.Contains(compare); });

        if(selected == null)
        {
            Debug.LogError("<b>[GameData] : </b>Sub-category " + sub.ToString() + " doesn't have corresponding category");
            return default(SubCategory);
        }

        int firstIndex = listOfSubCategories.IndexOf(selected);

        selected = listOfSubCategories.FindLast(item => { return item.Contains(compare); });

        if(selected == null)
        {
            Debug.LogError("<b>[GameData] : </b>Sub-category " + sub.ToString() + " doesn't have corresponding category");
            return default(SubCategory);
        }

        int lastIndex = listOfSubCategories.IndexOf(selected);

        if((int) sub > lastIndex || (int) sub < firstIndex)
        {
            Debug.LogWarning("<b>[GameData] : </b>Sub-category is part of wrong category ( sub is : " + sub.ToString().Split('_') [0] + " but category is : " + parent.ToString() + ")\nSub-category has been corrected to : " + listOfSubCategories[firstIndex]);
            return (SubCategory) firstIndex;
        }
        else
            return sub;
    }
}