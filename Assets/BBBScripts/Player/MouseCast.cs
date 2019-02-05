using UnityEngine;

public class MouseCast : MonoBehaviour {
	public float lenght;
	public static Vector3 mousePos;
//	public static Vector3 mouseDir;
	private Camera caster;
//	private Transform from;
	private RaycastHit mouseHit;
	private Ray mouseRay;

	private void Start() {
		caster = GetComponent<Camera>();
	}

	private void Update() {
		mouseRay = caster.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(mouseRay, out mouseHit)) {
			mousePos = mouseHit.point;
//			mouseDir = mousePos - from.position;
		} else {
			mousePos = mouseRay.origin + mouseRay.direction * lenght;
		}
	}
}
