using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    public static bool GameIsPaused = false;

    [Header("Settings")]
    [SerializeField] private Image damageIndicator;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private TextMeshProUGUI currentHealthTMP;
    [SerializeField] private TextMeshProUGUI currentShieldTMP;

    [Header("Weapon")]
    [SerializeField] private TextMeshProUGUI currentAmmoTMP;
    [SerializeField] private Image weaponImage;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI coinsTMP;

    private float playerCurrentHealth;
    private float playerMaxHealth;
    private float playerCurrentShield;
    private float playerMaxShield;
    private bool isPlayer;

    private int playerCurrentAmmo;
    private int playerMaxAmmo;

    private void Start()
    {
        Color c = damageIndicator.color;
        c.a = 0;
        damageIndicator.color = c;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }

        InternalUpdate();
    }

    public void FlashDamageEffect()
    {
        damageIndicator.enabled = true;
        Color c = damageIndicator.color;
        c.a = 1;
        damageIndicator.color = c;
    }

    public void UpdateHealth(float currentHealth, float maxHealth, float currentShield, float maxShield)
    {
        playerCurrentHealth = currentHealth;
        playerMaxHealth = maxHealth;
        playerCurrentShield = currentShield;
        playerMaxShield = maxShield;
    }

    public void UpdateWeaponSprite(Sprite weaponSprite) {
        weaponImage.sprite = weaponSprite;
        weaponImage.SetNativeSize();
    }

    public void HideAmmo()
    {
        currentAmmoTMP.enabled = false;
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        currentAmmoTMP.enabled = true;
        playerCurrentAmmo = currentAmmo;
        playerMaxAmmo = maxAmmo;
    }

    private void InternalUpdate()
    {
        // PLAYER HEALTH
        // to make health bar update smoothly, we lerp
        // visually it will look smooth, but each time it comes in here it just moves a little more
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, playerCurrentHealth / playerMaxHealth, 10f * Time.deltaTime);
        currentHealthTMP.text = playerCurrentHealth.ToString() + "/" + playerMaxHealth.ToString();
        shieldBar.fillAmount = Mathf.Lerp(shieldBar.fillAmount, playerCurrentShield / playerMaxShield, 10f * Time.deltaTime);
        currentShieldTMP.text = playerCurrentShield.ToString() + "/" + playerMaxShield.ToString();

        // DAMAGE INDICATOR
        if (damageIndicator.enabled)
        {
            Color c = damageIndicator.color;
            c.a = Mathf.Lerp(c.a, 0, 5f * Time.deltaTime);
            damageIndicator.color = c;
            if (c.a == 0)
            {
                damageIndicator.enabled = false;
            }
        }

        // PLAYER AMMO
        currentAmmoTMP.text = playerCurrentAmmo + " / " + playerMaxAmmo;

        // PLAYER COINS
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame() {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void LoadMainMenu() {
        Resume();
        SceneManager.LoadScene(0);
    }
}
