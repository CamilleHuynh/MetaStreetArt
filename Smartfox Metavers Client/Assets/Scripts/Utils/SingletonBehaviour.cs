namespace m21cerutti.BUS.Runtime {
	using UnityEngine;

	/// <summary>
	///     Singleton base class; provides a static accessor to Components that are only ever in one instance.
	///     If marked as DontDestroyOnLoad in the inspector, the root gameObject is not destroyed when loading new scenes.
	/// </summary>
	public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {
		/// <summary>
		///     If true, the component's root Transform is marked as DontDestroyOnLoad().
		/// </summary>
		[SerializeField,
		 Tooltip(
			 "Tick this to call DontDestroyOnLoad on the root transform object of this component. Note that if this is ticked on any of the children of an object, that object becomes DontDestroyOnLoad.")]
		private bool m_dontDestroyOnLoad;

		/// <summary>
		///     Static singleton instance of the component.
		/// </summary>
		public static T instance { get; private set; }

		/// <summary>
		///     Singleton Awake callback sets up the static Instance of the component. Make sure to call base.Awake() when
		///     overriding to keep this behaviour.
		/// </summary>
		protected virtual void Awake() {
			// Set the static instance to this component.
			if (instance == null) {
				instance = this as T;
				if (m_dontDestroyOnLoad) {
					DontDestroyOnLoad(transform.root.gameObject);
				}
				if (!gameObject.name.Contains(" (Singleton)")) {
					gameObject.name += " (Singleton)";
				}
			}

			// If an instance of this singleton already exists, destroy this one before it causes ambiguity.
			else if (instance != this) {
				Debug.LogWarning(GetType().Name + " already exist in scene.");
				Destroy(transform.root.gameObject);
			}
		}

		protected virtual void OnDestroy() {
			// If the object is destroyed, detach it from the static instance.
			if (instance == this) {
				instance = null;
			}
		}
	}
}