using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralPunchlines", menuName = "Koei/GeneralPunchlines")]
public class GeneralPunchlines : ScriptableObject
{
	public List<CategoryPunchlines> allPunchlines;
	[Space]
	[TextArea]
	public List<string> enemyDamageReaction;
	[Space]
	[TextArea]
	public List<string> enemyFailReaction;

	public int lastDamageIndex { get; set; }
	public int lastFailIndex { get; set; }

	public void Init()
	{
		lastDamageIndex = UnityEngine.Random.Range(0, enemyDamageReaction.Count - 1);
		lastFailIndex = UnityEngine.Random.Range(0, enemyFailReaction.Count - 1);
	}

	public string GetRandomDamageReaction()
	{
		Debug.LogWarning("<b>Last damage index : " + lastDamageIndex + "</b>");
		lastDamageIndex = GetDamageIndex();
		Debug.LogWarning("<b>New damage index : " + lastDamageIndex + "</b>");

		return enemyDamageReaction[lastDamageIndex];
	}

	int GetDamageIndex()
	{
		int index = UnityEngine.Random.Range(0, enemyDamageReaction.Count - 1);

		if(enemyDamageReaction.Count > 1 && index == lastDamageIndex)
			return GetDamageIndex();

		Debug.LogWarning("Last damage index : " + lastDamageIndex + " / new index : " + index);

		return index;
	}

	public string GetRandomFailReaction()
	{
		Debug.LogWarning("<b>Last fail index : " + lastFailIndex + "</b>");
		lastFailIndex = GetFailIndex();
		Debug.LogWarning("<b>New fail index : " + lastFailIndex + "</b>");

		return enemyFailReaction[lastFailIndex];
	}

	int GetFailIndex()
	{
		int index = UnityEngine.Random.Range(0, enemyFailReaction.Count - 1);

		if(enemyFailReaction.Count > 1 && index == lastFailIndex)
			return GetFailIndex();

		Debug.LogWarning("Last fail index : " + lastFailIndex + " / new index : " + index);

		return index;
	}

	public bool IsValid()
	{
		if(allPunchlines == null)
			return false;

		bool categoryGood = allPunchlines.Count == Enum.GetValues(typeof(Category)).Length;
		bool subCategoryGood = true;

		foreach (CategoryPunchlines categoryPunchlines in allPunchlines)
		{
			if(categoryPunchlines.punchlines == null)
				return false;

			foreach (Punchline punchline in categoryPunchlines.punchlines)
			{
				if(!punchline.subCategory.ToString().Contains(categoryPunchlines.category.ToString()))
					return false;
			}

			List<SubCategory> subCategories = new List<SubCategory>(Enum.GetValues(typeof(SubCategory)) as SubCategory[]);
			List<SubCategory> selected = subCategories.FindAll(item => { return item.ToString().Contains(categoryPunchlines.category.ToString()); });

			if(categoryPunchlines.punchlines.Count != selected.Count)
				subCategoryGood = false;
		}

		return categoryGood && subCategoryGood;
	}

	public void FixPunchlines()
	{
		allPunchlines = new List<CategoryPunchlines>();

		foreach (Category category in Enum.GetValues(typeof(Category)))
		{
			CategoryPunchlines pristine = new CategoryPunchlines(category);

			foreach (SubCategory subCategory in Enum.GetValues(typeof(SubCategory)))
			{
				if(subCategory.ToString().Contains(category.ToString()))
					pristine.punchlines.Add(new Punchline(subCategory));
			}

			allPunchlines.Add(pristine);
		}
	}

	[Serializable]
	public class CategoryPunchlines
	{
		public Category category;
		public List<Punchline> punchlines;

		public CategoryPunchlines(Category category)
		{
			this.category = category;
			punchlines = new List<Punchline>();
		}
	}

	[Serializable]
	public class Punchline
	{
		public SubCategory subCategory;
		[TextArea]
		public string line;

		public Punchline(SubCategory subCategory)
		{
			this.subCategory = subCategory;
			line = string.Empty;
		}
	}
}