using System;
using UnityEngine;

public enum Category
{
	WAR,
	FAMILY,
	RELIGION,
	MONEY,
	EMPTY
}

// subcategories HAVE to start with the categories they come from followed with a "_"
public enum SubCategory
{
	EMPTY,
	WAR_COURAGE,
	WAR_TALENT,
	WAR_LOYALTY,
	WAR_PLUNDER,
	WAR_PROTECTION,
	FAMILY_CHILDREN,
	FAMILY_PARENTS,
	FAMILY_SPOUSE,
	FAMILY_SEXUALITY,
	FAMILY_FIDELITY,
	RELIGION_ATHEISM,
	RELIGION_OFFERING,
	RELIGION_DEVOTION,
	RELIGION_RESPECT,
	RELIGION_OCCULTISM,
	MONEY_DEBTS,
	MONEY_MANAGING,
	MONEY_EXPENSES,
	MONEY_TAXES,
	MONEY_CORRUPTION
}

[Serializable]
public class CategoryColor
{
	public Category category;
	public Color color;
}