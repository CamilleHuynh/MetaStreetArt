namespace m21cerutti.BUS.Runtime {
	using System.Collections.Generic;

	using UnityEngine;

	/// <summary>
	///     Static class containing utilities functions for Layer management.
	/// </summary>
	public static class FuncLayer {
		/// <summary>
		///     LayerMask extension method for testing if a layer is inside the mask.
		/// </summary>
		/// <param name="layerMask"> The LayerMask in which to test.</param>
		/// <param name="layer">The layer to test.</param>
		/// <returns>True if the LayerMask have the layer inside.</returns>
		public static bool HaveLayer(this LayerMask layerMask, int layer) {
			return layerMask == (layerMask | (1 << layer));
		}

		/// <summary>
		///     LayerMask extension method for adding layer inside the mask.
		/// </summary>
		/// <param name="layerMask"> The original mask. </param>
		/// <param name="layer">The layer to add. </param>
		/// <returns> The new mask. </returns>
		public static LayerMask AddLayer(this LayerMask layerMask, int layer) { return layerMask | (1 << layer); }

		/// <summary>
		///     LayerMask extension method for removing layer inside the mask.
		/// </summary>
		/// <param name="layerMask"> The original mask. </param>
		/// <param name="layer">The layer to remove. </param>
		/// <returns> The new mask. </returns>
		public static LayerMask RemoveLayer(this LayerMask layerMask, int layer) { return layerMask & ~(1 << layer); }

		/// <summary>
		///     Transform extension method for setting this transform and children's one to a specific layer.
		///     WATCHOUT : It can be an expensive method depending on the tree.
		/// </summary>
		/// <param name="root">The root transform.</param>
		/// <param name="layer"> The layer o set gameObjects</param>
		public static void SetChildLayers(this Transform root, int layer) {
			Stack<Transform> move_targets = new Stack<Transform>();
			move_targets.Push(root);
			Transform current_target;
			while (move_targets.Count != 0) {
				current_target = move_targets.Pop();
				current_target.gameObject.layer = layer;
				foreach (Transform child in current_target) {
					move_targets.Push(child);
				}
			}
		}
	}
}