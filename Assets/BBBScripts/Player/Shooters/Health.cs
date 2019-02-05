using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable {
	public UnityEvent OnDeath;
	public float health = 100f;

	public void Damage(float _damage) {
		health -= _damage;
		if (health < 0f) {
			Destroy(this);
			OnDeath.Invoke();
		}
	}
}
