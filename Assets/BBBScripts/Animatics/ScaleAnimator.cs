using UnityEngine;

public class ScaleAnimator : MonoBehaviour {
	public Transform scalable;
	public Vector3 target;
	public float scalingTime = 1f;

	private float state = 0f;
	private Vector3 initialScale;

	private void Awake() {
		if (scalable == null)
			scalable = transform;
	}

	private void OnEnable() {
		state = 0f;
		initialScale = scalable.localScale;
	}

	// Update is called once per frame
	private void Update() {
		state = Mathf.Clamp01(state + Time.deltaTime / scalingTime);
		scalable.localScale = initialScale * (1 - state) + target * state;
		if (state >= 1f)
			enabled = false;
	}
}
