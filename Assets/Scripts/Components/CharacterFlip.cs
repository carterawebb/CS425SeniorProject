using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlip : CharacterAbilities {
    public enum FlipMode {
        MovementDirection,
        WeaponDirection
    }

    [SerializeField] private FlipMode flipMode = FlipMode.MovementDirection;
    [SerializeField] private float flipThreshold = 0.1f;

    // used for recoil for guns
    public bool FacingRight { get; set; }

    private void Awake()
    {
        //FacingRight = true;
        FacingRight = transform.root.transform.localScale.x == 1;
    }

    protected override void HandleAbility() {
        base.HandleAbility();

        if (flipMode == FlipMode.MovementDirection) {
            FlipToMoveDirection();
        } else if (flipMode == FlipMode.WeaponDirection) {
            FlipToWeaponDirection();
        }
    }

    private void FlipToMoveDirection () {
        if (controller.CurrentMovement.normalized.magnitude > flipThreshold) {
            if (controller.CurrentMovement.normalized.x > 0) {
                FaceDirection(1);
            } else if (controller.CurrentMovement.normalized.x < 0) {
                FaceDirection(-1);
            }
        }
    }

    private void FlipToWeaponDirection ()
    {
        if (characterWeapon != null)
        {
            float weaponAngle = characterWeapon.WeaponAim.CurrentAimAngleAbsolute;
            if (weaponAngle > 90 || weaponAngle < -90)
            {
                FaceDirection(-1);
            }
            else
            {
                FaceDirection(1);
            }
        }
    }

    private void FaceDirection (int newDirection) {
        // 1 = right, -1 = left
        if (newDirection == 1)
        {
            // Changes scale on tranform of object to flip it
            character.CharacterSprite.transform.localScale = new Vector3(1, 1, 1);
            FacingRight = true;
        }
        else if (newDirection == -1)
        {
            character.CharacterSprite.transform.localScale = new Vector3(-1, 1, 1);
            FacingRight = false;
        }
    }
}
