public enum Category
{
	WAR,
	FAMILY,
	RELIGION,
	MONEY
}

// subcategories HAVE to start with the categories they come from followed with a "_"
public enum SubCategory
{
	EMPTY,
	WAR_COURAGE,
	WAR_TALENT,
	FAMILY_CHILDREN,
	FAMILY_PARENTS,
	RELIGION_ATHEISM,
	RELIGION_OFFERING,
	MONEY_DEBTS,
	MONEY_MANAGING
}