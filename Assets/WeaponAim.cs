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
    private bool isReloading = false;

    public TextMeshProUGUI ammoText;  // UI Element for displaying ammo

    // Rapid fire variables
    public float rapidFireRate = 0.2f; // Fire rate in seconds
    private bool isRapidFiring = false;

    void Start()
    {
        weaponSprite = weaponSpriteTransform.GetComponent<SpriteRenderer>();
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (PauseManager.IsPaused || isReloading)
            return;

        AimWeapon();
        FlipWeapon();

        if (Input.GetMouseButtonDown(0) && currentAmmo > 0) // Left click for normal shot
            Shoot();

        if (Input.GetMouseButton(1) && !isRapidFiring && currentAmmo > 0) // Right click for rapid fire
            StartCoroutine(RapidFire());

        if (Input.GetMouseButtonUp(1)) // Stop rapid fire when right click is released
            isRapidFiring = false;

        if (Input.GetKeyDown(KeyCode.R) && !isReloading) // Press 'R' to reload manually
            StartCoroutine(Reload());
    }

    void AimWeapon()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        // Set the correct z-distance from the camera to your weapon
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

        // Ensure correct Z-depth for ScreenToWorldPoint (commonly 10 units ahead of camera)
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

        CameraShake.Instance.Shake(3f, 0.2f);

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
    }

    IEnumerator RapidFire()
    {
        isRapidFiring = true;

        while (Input.GetMouseButton(1) && currentAmmo > 0 && !isReloading) // Fire continuously while right-click is held
        {
            CameraShake.Instance.Shake(3f, 0.2f);

            FireSingleBullet(); // Fire a single bullet
            yield return new WaitForSeconds(rapidFireRate); // Wait for next shot
        }

        isRapidFiring = false;
    }

    void FireSingleBullet()
    {
        if (currentAmmo <= 0 || isReloading)
            return;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = volume;
        audioSource.PlayOneShot(shootSound);

        Quaternion bulletRotation = Quaternion.Euler(0, 0, weaponSpriteTransform.eulerAngles.z);
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = bulletRotation * Vector2.right * bulletSpeed;
        Destroy(bullet, 0.3f);

        currentAmmo--;
        UpdateAmmoUI();
    }

    IEnumerator Reload()
    {
        isReloading = true;
        ammoText.text = "RELOADING...";

        // Play reload sound
        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(1f);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (!isReloading)
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }
}
