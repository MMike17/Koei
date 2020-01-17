// interface for classes that need to be initialized before being used
public interface IInitializable
{
	// represents initialized state to the outside
	// this should return the "initializedInternal" variable
	bool initialized { get; }
	// represents initialized state to the inside
	bool initializedInternal { get; set; }

	// this should set the "initializedInternal" variable
	void InitInternal();
}