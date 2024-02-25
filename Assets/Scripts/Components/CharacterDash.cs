using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillMenu;

public class CharacterDash : CharacterAbilities {
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField ]private float dashCooldownTimer = 0.75f;

    public bool isDashing;
    private float dashTimer;
    private Vector2 dashOrigin;
    private Vector2 dashDestination;
    private Vector2 newPlayerPosition;

    private SkillMenu skillMenu;
    private bool isSkilMenuDashUnlockedOnce;

    [SerializeField] private SoundManager soundManager;

    private void Awake() {
        skillMenu = FindObjectOfType<SkillMenu>();

        soundManager = FindObjectOfType<SoundManager>();
    }

    protected override void Update() {
        base.Update();

        if (!isSkilMenuDashUnlockedOnce && skillMenu.skillLevels[(int)SkillMenu.SkillEnum.IncreaseDash] > 0) {
            isSkilMenuDashUnlockedOnce = true;
            dashDistance = 6f;
        }

        if (soundManager == null) {
            soundManager = FindObjectOfType<SoundManager>();
        }
    }

    protected override void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownTimer <= 0f) {
            Dash();
            dashCooldownTimer = 1f;
        }
        else
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    protected override void HandleAbility() {
        base.HandleAbility();

        if (isDashing) {
            if (dashTimer < dashDuration) {
                newPlayerPosition = Vector2.Lerp(dashOrigin, dashDestination, dashTimer / dashDuration);
                controller.MovePosition(newPlayerPosition);
                dashTimer += Time.deltaTime;
            } else {
                StopDash();
            }
        }
    }

    private void Dash() {
        isDashing = true;
        dashTimer = 0f;
        dashOrigin = transform.position;

        controller.NormalMovement = false;

        dashDestination = transform.position + (Vector3)controller.CurrentMovement.normalized * dashDistance;

        soundManager.DashSound();
    }

    private void StopDash() {
        isDashing = false;
        controller.NormalMovement = true;
    }

    public bool GetIsDashing() {
        return isDashing;
    }
}
