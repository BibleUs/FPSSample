using UnityEngine;

public class RayWidthAnimator : MonoBehaviour {
	public LineRenderer scalable;
	public float target;
	public float scalingTime = 1f;

	private float state = 0f;
	private float initialScale;

	private void Awake() {
		if (scalable == null)
			scalable = GetComponent<LineRenderer>();
	}

	private void OnEnable() {
		state = 0f;
		initialScale = scalable.widthMultiplier;
	}

	// Update is called once per frame
	private void Update() {
		state = Mathf.Clamp01(state + Time.deltaTime / scalingTime);
		scalable.widthMultiplier = initialScale * (1 - state) + target * state;
		if (state >= 1f)
			enabled = false;
	}
}
