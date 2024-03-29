using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCirclePattern : BossBaseShot
{
    private void Update() {
        Shoot();
    }

    private void Shoot() {
        if (isShooting == false) {
            return;
        }

        // divides circle so projectiles are fired evenly
        float shiftAngle = 360f / projectileAmount;
        for(int i = 0; i < projectileAmount; i++) {
            BossProjectile bossProjectile = GetBossProjectile(transform.position);
            if (bossProjectile == null) {
                //goes to next iteration of the loop
                break;
            }

            float angle = shiftAngle * i;
            ShootProjectile(bossProjectile, projectileSpeed, angle, projectileAcceleration);
        }

        DisableShooting();
    }

    // shoot again method - used as start of shooting pattern
    public void EnableProjectile() {
        isShooting = true;
    }
}
