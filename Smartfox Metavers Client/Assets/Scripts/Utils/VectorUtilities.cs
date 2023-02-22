namespace m21cerutti.BUS.Runtime {
	using UnityEngine;

	public static class VectorUtilities {
		public static Vector2 Random(this Vector2 myVector, Vector2 min, Vector2 max) {
			return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
		}

		public static Vector3 Random(this Vector3 myVector, Vector3 min, Vector3 max) {
			return new Vector3(UnityEngine.Random.Range(min.x, max.x),
							   UnityEngine.Random.Range(min.y, max.y),
							   UnityEngine.Random.Range(min.z, max.z));
		}

		public static Vector2 Random(this Vector2 myVector, float size = 1.0f, bool center = false) {
			return (new Vector2().Random(Vector2.zero, Vector2.one) * size) -
				   (center ? Vector2.one * size / 2.0f : Vector2.zero);
		}

		public static Vector3 Random(this Vector3 myVector, float size = 1.0f, bool center = false) {
			return (new Vector3().Random(Vector3.zero, Vector3.one) * size) -
				   (center ? Vector3.one * size / 2.0f : Vector3.zero);
		}
	}
}