
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;

/// <summary>
/// sits on disk Prefab
/// 1)disk was spawn by game manager with respective client authority
/// 2)get destroied after 5sec when comes to rest withi in the script
/// 3)send power by power script with sync envet and sync var but not thorugh server
/// 4)But hit state set by similar tyle of event but through server by power script
/// 5)airdrag,drag set to 30
/// 6)set Back to 0 by event invoke by right click 
/// </summary>


public class DiskMove : NetworkBehaviour
{
   
    [SerializeField] private Rigidbody disk=null;
    [SerializeField] Material diskMat=null;
    private bool hitPosibility;
    public float poweMultiplies = 12f;
    public static event Action OnHit;
    private bool localHiStart,alreadyHit;
    private Vector3 power;
    private int count=0;
    private bool hitBlock;


    public void OnEnable()
    {
        Power.EventLocalHit += LocalHit;
        diskMat.color = Color.white;
    }
    private void OnDestroy()
    {
        Power.EventLocalHit -= LocalHit; 
        diskMat.color = Color.white;
    }
    private void Start()
    {
        localHiStart = false;
        alreadyHit = false;
        disk.isKinematic = true;
        hitPosibility = true;
        hitBlock = false;
    }
    private void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            LocalHit(new Vector3(0, 0, 10));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            LocalHit(new Vector3(0, 0,- 10));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            LocalHit(new Vector3(10, 0,0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            LocalHit(new Vector3(-10, 0,0));
        }

    }
 
    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            if (localHiStart)
            {
               // Physics.defaultContactOffset = 0.01f;
                alreadyHit = true;
                disk.isKinematic = false;
                OnHit?.Invoke();
                disk.AddForce(power * poweMultiplies);
                localHiStart = false;
            }
        }


    }
    private void LocalHit(Vector3 power)
    {
        if (hitPosibility)
        {
            if (!hitBlock)
            {
                hitBlock = true;
                this.power = power;
                localHiStart = true;
            }
            
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasAuthority)
        {

            if (collision.collider.CompareTag("Walls"))
            {
                float volume = collision.relativeVelocity.magnitude;
                AudioManeger.audioManegerInstance.Play("DiskWall", volume / 2.4f);

            }
            if (collision.collider.CompareTag("Pieces") && alreadyHit)
            {
                float volume = collision.relativeVelocity.magnitude;
                AudioManeger.audioManegerInstance.Play("DiskPiece", volume / 2.4f);

            }
        }
        else
        {
            if (collision.collider.CompareTag("Walls"))
            {
                AudioManeger.audioManegerInstance.Play("DiskWall", 0.5f);
            }
            if (collision.collider.CompareTag("Pieces"))
            {
                AudioManeger.audioManegerInstance.Play("DiskPiece", 0.5f);
            }

        }

    }

    private void OnCollisionStay(Collision collision)
    {
       
        if (hasAuthority)
        {
            if (collision.collider.CompareTag("Pieces") && !alreadyHit)
            {
                float difX = Vector3.Distance(collision.collider.transform.position, transform.GetComponentInChildren<Collider>().transform.position);
                if (difX < 0.04f)
                {
                    diskMat.color = Color.red;
                    hitPosibility = false;
                }
                else if (!hitPosibility)
                {
                    diskMat.color = Color.white;
                    hitPosibility = true;
                }

            }
        }
    }

   

}
