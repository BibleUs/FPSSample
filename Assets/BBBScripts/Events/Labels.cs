using UnityEngine;

public class Labels : MonoBehaviour {
	public string[] labels;

	public bool Check(string _label) {
		foreach (string label in labels) {
			if (label == _label)
				return true;
		}
		return false;
	}

	public bool CheckAny(string[] _labels) {
		foreach (string _label in _labels)
			foreach (string label in labels)
				if (label == _label)
					return true;
		return false;
	}

	public bool CheckAll(string[] _labels) {
		foreach (string _label in _labels) {
			foreach (string label in labels)
				if (label == _label)
					goto outer;
			return false;
			outer:;
		}
		return true;
	}
}
