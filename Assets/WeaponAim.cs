using UnityEngine;
using System.Collections;
using TMPro;  // Required for UI text

public class WeaponAim : MonoBehaviour
{
    public Transform playerTransform;
    public Transform weaponHolderTransform;
    public Transform weaponSpriteTransform;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;

    private SpriteRenderer weaponSprite;

    public AudioClip shootSound;
    public AudioClip reloadSound;  // Reload sound effect
    public AudioSource audioSource;
    public float pitchMin = 0.8f;
    public float pitchMax = 1.2f;
    public float volume = 0.8f;

    public int maxAmmo = 8;
    private int currentAmmo;

    public int maxRapidAmmo = 20;
    private int currentRapidAmmo;

    private bool isReloading = false;

    public TextMeshProUGUI ammoText;  // UI Element for displaying ammo

    // Rapid fire variables
    public float rapidFireRate = 0.2f; // Fire rate in seconds
    private bool isRapidFiring = false;

    // Pump shotgun cooldown
    public float pumpShotCooldown = 0.6f; // Cooldown between pump shots
    private float lastShotTime = -Mathf.Infinity;

    // Reference to the PauseMenu script
    public PauseMenu pauseMenu;

    void Start()
    {
        weaponSprite = weaponSpriteTransform.GetComponent<SpriteRenderer>();
        currentAmmo = maxAmmo;
        currentRapidAmmo = maxRapidAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (PauseMenu.isPaused)  // Or PauseMenu.IsPaused() if using a method
            return;  // Don't allow shooting when paused

        if (isReloading)
            return;

        AimWeapon();
        FlipWeapon();

        // Pump-action single shot (left click) with cooldown
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && Time.time - lastShotTime >= pumpShotCooldown)
            Shoot();

        if (Input.GetMouseButton(1) && !isRapidFiring && currentRapidAmmo > 0)
            StartCoroutine(RapidFire());

        if (Input.GetMouseButtonUp(1))
            isRapidFiring = false;

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            StartCoroutine(Reload());
    }


    void AimWeapon()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - weaponSpriteTransform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (Vector2)(mouseWorldPos - weaponSpriteTransform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weaponSpriteTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FlipWeapon()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        bool isLeft = mouseWorldPos.x < playerTransform.position.x;

        weaponSprite.flipY = isLeft;
        weaponHolderTransform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
        playerTransform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
    }

    void Shoot()
    {
        if (currentAmmo <= 0 || isReloading)
            return;

        CameraShake.Instance.Shake(2f, 0.2f);

        float bulletSpreadAngle = 3f;
        int bulletCount = 3;
        float minBulletSpeed = bulletSpeed * 0.8f;
        float maxBulletSpeed = bulletSpeed * 1.2f;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = volume;
        audioSource.PlayOneShot(shootSound);

        for (int i = 0; i < bulletCount; i++)
        {
            float angleOffset = (i - (bulletCount - 1) / 2f) * bulletSpreadAngle;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, weaponSpriteTransform.eulerAngles.z + angleOffset);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = bulletRotation * Vector2.right * UnityEngine.Random.Range(minBulletSpeed, maxBulletSpeed);
            Destroy(bullet, 0.3f);
        }

        currentAmmo--;
        UpdateAmmoUI();
        lastShotTime = Time.time;
    }

    IEnumerator RapidFire()
    {
        isRapidFiring = true;

        while (Input.GetMouseButton(1) && currentRapidAmmo > 0 && !isReloading)
        {
            CameraShake.Instance.Shake(2f, 0.2f);
            FireSingleBullet();
            yield return new WaitForSeconds(rapidFireRate);
        }

        isRapidFiring = false;
    }

    void FireSingleBullet()
    {
        if (currentRapidAmmo <= 0 || isReloading)
            return;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = volume;
        audioSource.PlayOneShot(shootSound);

        Quaternion bulletRotation = Quaternion.Euler(0, 0, weaponSpriteTransform.eulerAngles.z);
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = bulletRotation * Vector2.right * bulletSpeed;
        Destroy(bullet, 0.3f);

        currentRapidAmmo--;
        UpdateAmmoUI();
    }

    IEnumerator Reload()
    {
        isReloading = true;
        ammoText.text = "RELOADING...";

        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(1f);
        currentAmmo = maxAmmo;
        currentRapidAmmo = maxRapidAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (!isReloading)
            ammoText.text = $"Pump: {currentAmmo}/{maxAmmo}  |  Rapid: {currentRapidAmmo}/{maxRapidAmmo}";
    }
}
