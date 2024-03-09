using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTorch : Singleton<CTorch> {
    [Header("Required Connections")]
    [SerializeField] private GameObject blackMask;
    [SerializeField] private GameObject fieldOfView;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private GameObject camera;

    [Header("Torch Settings")]
    [SerializeField] private GameObject vendor;
    private Vendor vendorScript;
    [SerializeField] private Transform vendorPosition;
    [SerializeField] private float spawnOffsetX;
    [SerializeField] private float spawnOffsetY;
    [SerializeField] public float newViewDistance = 12.0f;
    [SerializeField] private float newCameraDistance = 6.5f;
    [SerializeField] private float torchFollowSpeed = 1.0f;
    [SerializeField] public float torchLerpTime = 2.0f;

    private float lastOffsetX;
    private float lastOffsetY;

    public bool torchHasSpawned;

    private readonly string TORCHKEY = "TORCH_KEY";

    // EVEN THOUGH WE HAVE THIS SAVING, WHAT HAPPENS IF WE BUY THE TORCH,
    // GO TO A SCENE THAT DOESNT HAVE IT, THEN QUIT THE GAME? DOES IT SAVE?
    protected override void Awake() {
        if (GameObject.FindObjectsOfType<CTorch>().Length > 1) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        base.Awake();
    }

    private void Start() {
        // Im not using the base class Start() method because I want to 
        // have it follow the player like the things in binding of isaac
        // (sorry if that doesn't make sense, I don't know how else to explain it)
        characterTransform = GameObject.Find("Player").transform;
        camera = GameObject.Find("Main Camera");

        //spriteRenderer = GetComponent<SpriteRenderer>();

        vendor = GameObject.Find("Vendor");
        vendorScript = vendor.GetComponent<Vendor>();
        vendorPosition = vendor.transform;

        transform.position = new Vector2(vendorPosition.position.x + spawnOffsetX, vendorPosition.position.y + spawnOffsetY);
    }

    private void Update() {
        if (blackMask == null) {
            blackMask = GameObject.Find("Black");
        }

        if (fieldOfView == null) {
            fieldOfView = GameObject.Find("FieldOfView");
        }

        if (characterTransform == null) {
            characterTransform = GameObject.Find("Player").transform;
        }

        if (vendor == null) {
            vendor = GameObject.Find("Vendor");
            vendorScript = vendor.GetComponent<Vendor>();
            vendorPosition = vendor.transform;
        }

        if (vendorScript.torchBought || PlayerPrefs.GetInt(TORCHKEY) == 1) {
            vendorScript.torchBought = true;

            SpawnTorch();

            torchHasSpawned = true;

            PlayerPrefs.SetInt(TORCHKEY, 1);
        }

        if (torchHasSpawned) {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0) {
                float offsetX = (horizontal < 0) ? Mathf.Floor(horizontal) : Mathf.Ceil(horizontal);
                float offsetY = (vertical < 0) ? Mathf.Floor(vertical) : Mathf.Ceil(vertical);

                transform.position = Vector2.Lerp(transform.position, characterTransform.position, torchFollowSpeed * Time.deltaTime);
                lastOffsetX = offsetX;
                lastOffsetY = offsetY;
            } else {
                if (!(transform.position.x < lastOffsetX + 0.5f) || !(transform.position.y < lastOffsetY + 0.5f)) {
                    transform.position = Vector2.Lerp(transform.position, new Vector2(characterTransform.position.x - lastOffsetX, characterTransform.position.y - lastOffsetY), torchFollowSpeed * Time.deltaTime);
                }
            }

            StartFadingAlpha( (195f/ 255f), torchLerpTime);
        } else {
            vendor = GameObject.Find("Vendor");
            vendorScript = vendor.GetComponent<Vendor>();
            vendorPosition = GameObject.Find("Vendor").transform;

            transform.position = new Vector2(vendorPosition.position.x + spawnOffsetX, vendorPosition.position.y + spawnOffsetY);

            PlayerPrefs.SetInt(TORCHKEY, 0);
        }
    }

    public void SpawnTorch() {
        if (camera == null) {
            camera = GameObject.Find("Main Camera");
        } else {
            if (Camera.main.orthographicSize != newCameraDistance) {
                camera.GetComponent<Camera2D>().originalZoom = newCameraDistance;
                camera.GetComponent<Camera2D>().vendorZoomOut = 1.5f;
                camera.GetComponent<Camera2D>().bossZoomOut = 1.9f;
            }
        }
    }

    public void StartFadingAlpha(float targetAlpha, float duration) {
        SpriteRenderer blackMaskSpriteRenderer = blackMask.GetComponent<SpriteRenderer>();
        if (blackMaskSpriteRenderer != null) {
            StartCoroutine(FadeToAlpha(blackMaskSpriteRenderer, targetAlpha, duration));
        } else {
            Debug.LogError("blackMask does not have a SpriteRenderer component.");
        }
    }

    private IEnumerator FadeToAlpha(SpriteRenderer spriteRenderer, float targetAlpha, float duration) {
        float time = 0;
        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (time < duration) {
            time += Time.deltaTime;
            float normalizedTime = time / duration; // This will go from 0 to 1, thus completing the fade.
            // Ensure the normalized time is not greater than 1.
            normalizedTime = Mathf.Clamp01(normalizedTime);

            spriteRenderer.color = Color.Lerp(startColor, targetColor, normalizedTime);
            yield return null;
        }
    }

    public void Pick() {
        SpawnTorch();
    }

    private void OnQuit() {
        PlayerPrefs.SetInt(TORCHKEY, 0);
    }
}
