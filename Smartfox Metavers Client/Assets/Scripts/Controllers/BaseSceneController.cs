using UnityEngine;

/**
 * Parent class of the script attached to the Controller object in all scenes.
 */
public abstract class BaseSceneController : SingletonBehaviour<BaseSceneController>
{
    protected GlobalManager gm;

    /**
	 * On awake, set a reference to Global Manager.
	 */
    protected override void Awake()
    {
        base.Awake();
        // Get Global Manager instance
        gm = GlobalManager.Instance;
    }

    /**
	 * On update, hide modals if ESC button is pressed.
	 */
    protected virtual void Update()
    {
        // On ESC button press, hide modals
        if (Input.GetKeyDown("escape"))
            HideModals();
    }

    /**
	 * When a scene is destroyed, make sure all event listeners it added to the SmartFox class instance are removed.
	 */
    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Remove SFS2X listeners
        RemoveSmartFoxListeners();
    }

    /**
	 * Abstract method to be implementated by controller classes, to remove all SmartFox-related event listeners.
	 */
    protected abstract void RemoveSmartFoxListeners();

    /**
	 * Abstract method to be implementated by controller classes, to hide popup modals.
	 */
    protected abstract void HideModals();
}