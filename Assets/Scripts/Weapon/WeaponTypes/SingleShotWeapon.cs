using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotWeapon : Weapon {
    [SerializeField] private Vector3 projectileSpawnPosition;

    public Vector3 ProjectileSpawnPosition { get; set; }

    private Vector3 projectileSpawnValue;

    void Start() {
        projectileSpawnValue = projectileSpawnPosition;
        projectileSpawnValue.y = -projectileSpawnPosition.y;
    }

    protected override void Update() {
        base.Update();
    }

    protected override void RequestShot() {
        base.RequestShot();
    }

    private void SpawnProjectile(Vector2 spawnPosition) {

    }

    private void EvaluateProjetileSpawnPosition() {
        if (WeaponOwner.GetComponent<CharacterFlip>().FacingRight) {
            ProjectileSpawnPosition = transform.position + transform.rotation * projectileSpawnPosition;
        } else {
            ProjectileSpawnPosition = transform.position - transform.rotation * projectileSpawnValue;
        }
    }

    private void OnDrawGizmosSelected() {
        EvaluateProjetileSpawnPosition();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(ProjectileSpawnPosition, 0.1f);
    }
}
