using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/**
 * Script attached to Settings Panel prefab.
 */
public class SettingsPanel : BasePanel
{
	private const int CUBE_MODEL = 0;
	private const int SPHERE_MODEL = 1;
	private const int CAPSULE_MODEL = 2;

	private const int BLUE_MATERIAL = 0;
	private const int GREEN_MATERIAL = 1;
	private const int RED_MATERIAL = 2;
	private const int YELLOW_MATERIAL = 3;

	public Toggle aoiToggle;

	public Toggle cubeToggle;
	public Toggle sphereToggle;
	public Toggle capsuleToggle;

	public Toggle blueToggle;
	public Toggle greenToggle;
	public Toggle redToggle;
	public Toggle yellowToggle;

	public UnityEvent<int> onModelToggleChange;
	public UnityEvent<int> onMaterialToggleChange;
	public UnityEvent<bool> onAoiToggleChange;

	public void SetModelSelection(int numModel)
    {
		// Update settings panel with the selected model
		switch (numModel)
		{
			case CUBE_MODEL:
				cubeToggle.SetIsOnWithoutNotify(true);
				break;
			case SPHERE_MODEL:
				sphereToggle.SetIsOnWithoutNotify(true);
				break;
			case CAPSULE_MODEL:
				capsuleToggle.SetIsOnWithoutNotify(true);
				break;
		}
	}

	public void SetMaterialSelection(int numMaterial)
    {
		// Update settings panel with the selected material
		switch (numMaterial)
		{
			case BLUE_MATERIAL:
				blueToggle.SetIsOnWithoutNotify(true);
				break;
			case GREEN_MATERIAL:
				greenToggle.SetIsOnWithoutNotify(true);
				break;
			case RED_MATERIAL:
				redToggle.SetIsOnWithoutNotify(true);
				break;
			case YELLOW_MATERIAL:
				yellowToggle.SetIsOnWithoutNotify(true);
				break;
		}
	}

	public void OnModelToggleChange()
	{
		if (cubeToggle.isOn)
			onModelToggleChange.Invoke(CUBE_MODEL);
		else if (sphereToggle.isOn)
			onModelToggleChange.Invoke(SPHERE_MODEL);
		else if (capsuleToggle.isOn)
			onModelToggleChange.Invoke(CAPSULE_MODEL);
	}

	public void OnMaterialToggleChange()
	{
		if (blueToggle.isOn)
			onMaterialToggleChange.Invoke(BLUE_MATERIAL);
		else if (greenToggle.isOn)
			onMaterialToggleChange.Invoke(GREEN_MATERIAL);
		else if (redToggle.isOn)
			onMaterialToggleChange.Invoke(RED_MATERIAL);
		else if (yellowToggle.isOn)
			onMaterialToggleChange.Invoke(YELLOW_MATERIAL);
	}

	public void OnAoiToggleChange()
    {
		onAoiToggleChange.Invoke(aoiToggle.isOn);
    }
}
