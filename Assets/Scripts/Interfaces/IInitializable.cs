public interface IInitializable
{
	bool initialized { get; }
	bool initializedInternal { get; set; }

	void InitInternal();
}