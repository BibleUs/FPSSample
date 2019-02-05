using UnityEngine;

public class MouseCur : MonoBehaviour {
	public Texture cursorImage;
	public int curWidth = 128;
	public int curHeigh = 128;
	private Vector3 mousePos;
	
	void Start()
    {
		Cursor.visible = false;
	}

	void OnGUI() {
		mousePos = Input.mousePosition;
		Rect pos = new Rect(mousePos.x - curWidth / 2, Screen.height - mousePos.y - curHeigh / 2, curWidth, curHeigh);
		GUI.Label(pos, cursorImage);
	}
}
