using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    [Header("Field of View")]
    [SerializeField] private FieldOfView fieldOfView;

    [Header("Shield")]
    [SerializeField] private float initialShield = 5f;
    [SerializeField] protected float maxShield = 5f;
    [SerializeField] protected float delayAfterDmgToBeginShieldRegen = 1.0f; // seconds
    [SerializeField] protected float shieldRegenRate = 0.25f; // shield / second

    [Header("Health")]
    [SerializeField] private float initialHealthPlayer = 10f;
    [SerializeField] protected float maxHealthPlayer = 10f;
    
    private float timeToStartRegen = 0.0f;
    private bool needRegen = false;

    //private CharacterSkills characterSkills;
    private SkillMenu skillMenu;
    private int lastCheckHealth;
    private int lastCheckShield;
    
    private Character character;
    private PlayerController controller;
    private Collider2D collide2D;
    private SpriteRenderer[] spriteRenderers;
    private CharacterWeapon characterWeapon;
    private CharacterFlip characterFlip;
    private CharacterDash characterDash;
    private bool shieldBroken;
    public float CurrentShield { get; set; }


    // Awake is called before start
    protected override void Awake()
    {
        character = GetComponent<Character>(); // this should be a player
        controller = GetComponent<PlayerController>();
        collide2D = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); // allow for any expansion of hierarchy
        characterWeapon = GetComponent<CharacterWeapon>();
        characterFlip = GetComponent<CharacterFlip>();
        characterDash = GetComponent<CharacterDash>();
        CurrentHealth = initialHealthPlayer;
        CurrentShield = initialShield;

        UpdateHealth();

        //characterSkills = GetComponent<CharacterSkills>();
        skillMenu = SkillMenu.skillMenu;

        lastCheckHealth = 0;
        lastCheckShield = 0;
    }

    protected override void Update ()
    {
        base.Update();

        // fieldOfView.SetAimDirection(aimDir);
        fieldOfView.SetOrigin(transform.position);

        if (skillMenu.skillLevels[(int)SkillMenu.SkillEnum.IncreaseHealth] > lastCheckHealth) {
            GainMaxHealth(1);
        }

        if (skillMenu.skillLevels[(int)SkillMenu.SkillEnum.IncreaseShield] > lastCheckShield) {
            GainMaxShield(2);
        }

        if (needRegen && Time.time > timeToStartRegen)
        {
            CurrentShield += shieldRegenRate * Time.deltaTime;
            shieldBroken = false;
            if (CurrentShield >= maxShield)
            {
                needRegen = false;
                CurrentShield = maxShield;
            }
            UpdateHealth();
        }
    }

    public override void TakeDamage(int damage)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        UIManager.Instance.FlashDamageEffect();
        if (!shieldBroken)
        {
            CurrentShield -= damage;
            CurrentShield = Mathf.Max(CurrentShield, 0); // prevent negative numbers
            UpdateHealth();
            if (CurrentShield <= 0)
            {
                shieldBroken = true;
            }
            needRegen = true;
            timeToStartRegen = delayAfterDmgToBeginShieldRegen + Time.time;
            return;
        }

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0); // prevent negative numbers
        UpdateHealth();

        if (CurrentHealth <= 0)
        {
            TriggerDeath();
        }
        else
        {
            needRegen = true;
            timeToStartRegen = delayAfterDmgToBeginShieldRegen + Time.time;
        }
    }

    protected void TriggerDeath()
    {
        needRegen = false;
        character.enabled = false;
        controller.enabled = false;
        collide2D.enabled = false;
        characterWeapon.Disable();
        characterWeapon.enabled = false;
        characterFlip.enabled = false;
        characterDash.enabled = false;
        character.CharacterAnimator.SetTrigger("PlayerDeath");
    }

    public void Kill()
    {
        Die();
    }

    protected override void Die()
    {
        if (character != null)
        {
            character.enabled = false;
            controller.enabled = false;
            collide2D.enabled = false;
            foreach (SpriteRenderer s in spriteRenderers)
            {
                s.enabled = false;
            }
        }

        if (destroyObject)
        {
            DestroyObject();
        }
    }

    public override void Revive()
    {
        if (character != null)
        {
            character.enabled = true;
            controller.enabled = true;
            collide2D.enabled = true;
            characterWeapon.enabled = true;
            characterWeapon.Enable();
            characterFlip.enabled = true;
            characterDash.enabled = true;
            foreach (SpriteRenderer s in spriteRenderers)
            {
                s.enabled = true;
            }
        }

        gameObject.SetActive(true);
        CurrentHealth = initialHealth;
        CurrentShield = initialShield;
        shieldBroken = false;

        UpdateHealth();
        
    }

    public override void GainHealth(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealthPlayer);
        UIManager.Instance.FlashHealthEffect();
        UpdateHealth();
    }

    public void GainMaxHealth(int amount)
    {
        if (skillMenu.skillLevels[(int)SkillMenu.SkillEnum.IncreaseHealth] > lastCheckHealth) {
            maxHealthPlayer *= amount;
            CurrentHealth = maxHealthPlayer;
            UpdateHealth();
            UIManager.Instance.FlashHealthEffect(); // Might want to disable this b/c it might just flash even though the health UI isnt visible
            lastCheckHealth ++;
        }
    }

    public void GainShield(int amount)
    {
        CurrentShield = Mathf.Min(CurrentShield + amount, maxShield);
        UIManager.Instance.FlashShieldEffect();
        UpdateHealth();
        if (CurrentShield > 0 && shieldBroken)
        {
            shieldBroken = false;
        }
    }

    public void GainMaxShield(int amount)
    {
        if (skillMenu.skillLevels[(int)SkillMenu.SkillEnum.IncreaseShield] > lastCheckShield) {
            maxShield *= amount;
            CurrentShield = maxShield;
            UpdateHealth();
            UIManager.Instance.FlashShieldEffect();
            lastCheckShield ++;
        }
    }

    protected override void UpdateHealth()
    {
        UIManager.Instance.UpdateHealth(CurrentHealth, maxHealthPlayer, CurrentShield, maxShield);
    }

    public bool IsFullHealth(string healthType ) {
        if (healthType.Equals("health")) {
            if (maxHealthPlayer == CurrentHealth) {
                return true;
            }
        }

        if (healthType.Equals("shield")) {
            if (maxShield == CurrentShield) {
                return true;
            }
        }

        return false;
    }
}
