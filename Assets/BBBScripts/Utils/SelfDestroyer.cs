using UnityEngine;

public class SelfDestroyer : MonoBehaviour {
	public bool auto;
	public float delay = 1f;

	private void Start() {
		if (auto)
			Destroy(gameObject, delay);
	}

	public void DestroyNow() {
		Destroy(gameObject);
	}

	public void DestroyDelayed() {
		Destroy(gameObject, delay);
	}
}
