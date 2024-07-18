    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ProjectileGun : MonoBehaviour
{
    public GameObject bullet; //Bullet
    public GameObject cannon;
    public int damage;
    public float shootForce, upwardForce; //forces applied to the bullet 
    public ParticleSystem smoke;
    //gun variables 
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public GameObject shootingAudio;
    public AudioClip shootingAudioClip;
    public AudioClip reloadingAudioClip;
    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    public Camera cam;
    public Transform attackPoint;

    public bool allowInvoke = true;

    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunition;
    public float zOffset = 0f;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        Vector3 direction = attackPoint.transform.position - cannon.transform.position;
        attackPoint.transform.rotation = cannon.transform.rotation;
        attackPoint.transform.position = cannon.transform.position;
        MyInput();

        if(ammunition != null)
        {
            ammunition.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    private void MyInput()
    {

        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            
            Reload();
            smoke.Play();
        } 
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            Reload();
            smoke.Play();
        }

        if(readyToShoot && shooting && !reloading && bulletsLeft >0)
        {
            bulletsShot = 0;
            Shoot();
            smoke.Stop();
            if(allowButtonHold)
            {
                shootingAudio.GetComponent<AudioSource>().PlayOneShot(shootingAudioClip);
            }
    
        }
    }

    private void Shoot()
    {

        readyToShoot = false;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        } else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread =  attackPoint.transform.forward;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread;

        GameObject currentBullet = Instantiate(bullet, attackPoint.transform.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized; 

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up* upwardForce, ForceMode.Impulse);

        if(muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShooting);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        shootingAudio.GetComponent<AudioSource>().PlayOneShot(reloadingAudioClip);
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

}
