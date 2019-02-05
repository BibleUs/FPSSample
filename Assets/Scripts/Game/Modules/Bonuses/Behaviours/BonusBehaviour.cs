using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[ServerOnlyComponent]
[RequireComponent(typeof(BoxCollider))]
public class BonusBehaviour : MonoBehaviour {

    public GameObject visual;
    public Entity character;
    public BoxCollider collider;
    public BonusSpawnPoint spawner;

    private void Awake() {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other) {
        var hitCollision = other.gameObject.GetComponent<HitCollision>();
        if (hitCollision == null)
            return;
        character = hitCollision.owner.GetComponent<GameObjectEntity>().Entity;
    }

    public void Despawn() {
        Destroy(gameObject);
    }
}



