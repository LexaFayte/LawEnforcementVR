using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public GameObject controllerRight;
    public Transform muzzleTrans;
    public AudioSource source;
    public AudioSource ReloadSource;
    public AudioSource GunEmptySource;
    public GameObject bullet_mark;
    public Animation GunAnims;
    public GameObject CameraContainer;

    [Range(1,17)]
    public int MagSize;

    //SteamVR
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;


    private int currentBullets;

    //properties
    public int Bullets
    {
        get { return Bullets; }
        set { Bullets = value; }
    }

    //initialization
    private void Awake()
    {
        if(MagSize == 0)
        {
            MagSize = 9;
        }

        currentBullets = MagSize;

        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.TriggerClicked += TriggerPressed;
        controller.Gripped += GripPressed;
        trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
    }
	
    //events
    private void TriggerPressed(object sender, ClickedEventArgs e)
    {
        if(CameraContainer.GetComponent<PauseController>().IsPaused == false)   
            Shoot();
    }

    private void GripPressed(object sender, ClickedEventArgs e)
    {
        if (CameraContainer.GetComponent<PauseController>().IsPaused == false)
            Reload();
    }

    

    //functions

    /// <summary>
    /// deals with shooting logic
    /// </summary>
    public void Shoot()
    {
        if (currentBullets != 0)
        {
            --currentBullets;

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(muzzleTrans.position, muzzleTrans.forward);

            GunAnims.Play("GunShot", PlayMode.StopSameLayer);
            source.Play();

            //device = SteamVR_Controller.Input((int)trackedObj.index);
            //device.TriggerHapticPulse(750);

            SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(3999);

            if (Physics.Raycast(ray, out hit, 5000f))
            {
                if (hit.collider.attachedRigidbody)
                {
                    hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction, ForceMode.Impulse);
                    GameObject Bullet_Mark = Instantiate(bullet_mark, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
                    Bullet_Mark.transform.Rotate(new Vector3(180, 0, 0));
                    Bullet_Mark.transform.Translate(new Vector3(0, 0, -0.005f));
                    Bullet_Mark.transform.SetParent(hit.rigidbody.gameObject.transform);

                    if(hit.rigidbody.gameObject.tag == "Target" && hit.rigidbody.gameObject.GetComponent<TargetMove>().Hit == false)
                    {
                        hit.rigidbody.gameObject.GetComponent<TargetMove>().Hit = true;
                    }
                }
            }

            if(currentBullets == 0)
            {
                GunAnims.Play("GunEmpty", PlayMode.StopSameLayer);
            }
        }
        else
        {
            GunEmptySource.Play();
        }
    }

    /// <summary>
    /// reloads the gun (plays animation, sound clip, and resets bullet count)
    /// </summary>
    public void Reload()
    {
        //play reload sound
        GunAnims.Play("Reload", PlayMode.StopSameLayer);
        ReloadSource.Play();
        currentBullets = MagSize;
    }

}
