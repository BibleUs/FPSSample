using UnityEngine;

public class LaserGun : MonoBehaviour, IShooter
{
	public Transform shootSource;
	public GameObject laserPrefab;
	public float maxDistance;
	public float fireRate = 0.2f;
	public float reloadTime = 2f;
	public int clipSize = 15;
	public float damage = 30f;

	public bool isShooting;
	private ShootState shootState;
	private float lastActionTime;
	private float waitTime;
	private int bulletsLeft;
	private GameObject lastBullet;
	private Vector3 lastTarget;
	private RaycastHit laserHit;
	private LineRenderer lineRenderer;
	private Vector3[] trajectory = new Vector3[2];
	private ObjectsContainer objectsContainer;
	private IDamagable[] damaged;

	private void Start() {
		bulletsLeft = clipSize;
	}

	public void Shoot(bool _state) {
		isShooting = _state;
	}

	private void Update() {
		if (!isShooting) return;
		if (Time.time < lastActionTime + waitTime) return;

		lastActionTime = Time.time;

		if (shootState == ShootState.Shooting) {
			MakeLaserBeam();
			Damage();
			--bulletsLeft;
			if (bulletsLeft == 0) {
				shootState = ShootState.Reloading;
				waitTime = reloadTime;
			} else
				waitTime = fireRate;
			return;
		}

		if (shootState == ShootState.Reloading) {
			bulletsLeft = clipSize;
			waitTime = 0;
			shootState = ShootState.Shooting;
			isShooting = false;
		}
	}

	private void MakeLaserBeam() {
		lastBullet = Instantiate(laserPrefab, shootSource.position, shootSource.rotation);
		trajectory[0] = shootSource.position;

		objectsContainer = lastBullet.GetComponent<ObjectsContainer>();

		if (Physics.Raycast(trajectory[0], (transform.forward*maxDistance), out laserHit, maxDistance)) {
			trajectory[1] = laserHit.point;
			objectsContainer.objects[0].GetComponent<ScaleAnimator>().scalingTime *= 10;
		} else {
			trajectory[1] = trajectory[0] + (transform.forward*maxDistance);
		}

		objectsContainer.objects[0].transform.position = trajectory[1];

		lineRenderer = lastBullet.GetComponentInChildren<LineRenderer>();
		lineRenderer.SetPositions(trajectory);
	}


	public void MakeLaser(Vector3 start, Vector3 end) {
		var laser = Instantiate(laserPrefab);
		var renderer = laser.GetComponentInChildren<LineRenderer>();
		renderer.SetPositions(new []{start, end});
	}

	private void Damage() {
		if (laserHit.collider == null) return;

		damaged = laserHit.collider.GetComponents<IDamagable>();
		foreach (IDamagable damagable in damaged)
			damagable.Damage(damage);
	}
}
