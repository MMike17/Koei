using UnityEngine;

public static class CustomHelper
{
	/// <summary>"More Or Less" computes if the two numbers are close enough</summary>
	/// <param name="first_value">first value to compare</param>
	/// <param name="second_value">second value to compare</param>
	/// <param name="step">distance between the two numbers</param>
	/// <returns>returns true if numbers are close enough, false otherwise</returns>
	public static bool MOL(float first_value, float second_value, float step)
	{
		return first_value <= second_value + step?first_value >= second_value - step?true : false : false;
	}

	/// <summary>"More Or Less" computes if the two 3D positions are close enough</summary>
	/// <param name="first_position">first position to compare</param>
	/// <param name="second_position">second position to compare</param>
	/// <param name="step">distance between the two position</param>
	/// <returns>returns true if positions are close enough, false otherwise</returns>
	public static bool MOL(Vector3 first_position, Vector3 second_position, float step)
	{
		bool x = first_position.x <= second_position.x + step?x = first_position.x >= second_position.x - step?true : false : false, y = first_position.y <= second_position.y + step?y = first_position.y >= second_position.y - step?true : false : false, z = first_position.z <= second_position.z + step?z = first_position.z >= second_position.z - step?true : false : false;

		return x?y?z?true : false : false : false;
	}

	/// <summary>"More Or Less" computes if the two quaternions are close enough</summary>
	/// <param name="first_rot">first rotation to compare</param>
	/// <param name="second_rot">second rotation to compare</param>
	/// <param name="step">distance between the two rotations</param>
	/// <returns>returns true if rotations are close enough, false otherwise</returns>
	public static bool MOL(Quaternion first_rot, Quaternion second_rot, float step)
	{
		return CustomHelper.MOL(first_rot.eulerAngles, second_rot.eulerAngles, step);
	}
}