using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerCameraRig : MonoBehaviour {
    private Vector3 defaultOffset;
    private float pitch;
    
    private PlayerMovement playerMovement;
    private Player player;
    private PostProcessingBehaviour postProcessingBehaviour;
    public PostProcessingProfile defaultPPProfile;
    public PostProcessingProfile stunnedPPProfile;
    public float horizontalSensitivity = 1f;
    public float verticalSensitivity = 1f;
    public LayerMask whatIsCollidable;
    public LayerMask whatIsGround;
    public float transitionTime = 0.05f;
    public Transform cam;

    public enum Shoulder {
        Left, Right    
    }
    public Shoulder shoulder;

    public LayerMask whatIsOpaque;
    

    private Vector3 camOffsetVelocity;

    void Start () {
        defaultOffset.x = Mathf.Abs(defaultOffset.x);
        //cam = GetComponentInChildren<Camera>();
        cam = transform.Find("Player Camera").transform;
        defaultOffset = cam.gameObject.transform.localPosition;
        playerMovement = GetComponentInParent<PlayerMovement>();
        player = GetComponentInParent<Player>();

        postProcessingBehaviour = GetComponentInChildren<PostProcessingBehaviour>();
    }
	
	void Update () {
        if(player.magic == null) { return; } // player script not yet initialized
        if (player.magic.isAiming)
        {
            playerMovement.lookPosition = GetAimPoint();
            
            
        } else
        {
            playerMovement.lookPosition = Vector3.zero;
        }


        if (player.magic.isHandOpen || GameManager.instance.isMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }







        if (Input.GetKeyDown(KeyCode.Q))
        {
            shoulder = Shoulder.Left;
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            shoulder = Shoulder.Right;
        }
        
        float axisX = Input.GetAxis("Mouse X");
        float axisY = Input.GetAxis("Mouse Y");

        Vector3 desiredOffset = defaultOffset;

        if (shoulder == Shoulder.Right)
        {
            desiredOffset.x *= -1;
        }

        Ray ray = new Ray(transform.position, transform.TransformDirection(desiredOffset));
        RaycastHit hit;
        Debug.DrawRay(transform.position, ray.direction);

        if (player.magic.isAiming)
        {
            cam.gameObject.transform.position = transform.position;
            

        }
        else if(Physics.Raycast(ray, out hit, desiredOffset.magnitude, whatIsOpaque, QueryTriggerInteraction.Ignore))
        {
            cam.gameObject.transform.position = hit.point;

        }
        else
        {
            cam.gameObject.transform.localPosition = Vector3.SmoothDamp(cam.gameObject.transform.localPosition, desiredOffset, ref camOffsetVelocity, transitionTime);

        }

        

        float pitchAngle = -axisY * verticalSensitivity;
        float yawAngle = axisX * horizontalSensitivity;

        Vector3 currentEulerAngles = transform.rotation.ToEuler() / Mathf.PI * 180;
        
        if(currentEulerAngles.x+pitchAngle > 80)
        {
            pitchAngle = 80 - currentEulerAngles.x;
        }else if (currentEulerAngles.x + pitchAngle < -80)
        {
            pitchAngle = -80 - currentEulerAngles.x;
        }

        if (!player.magic.isHandOpen && !GameManager.instance.isMenuOpen)
        {
            transform.Rotate(pitchAngle, 0, 0, Space.Self);
            transform.Rotate(0, yawAngle, 0, Space.World);
        }


        ApplyStatusEffects();


        

    }

    public Vector3 GetAimPoint()
    {
        return GetAirAimPoint(100f);
    }

    public Vector3 GetAirAimPoint(float maxDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, whatIsCollidable, QueryTriggerInteraction.Ignore))
        {
            return hit.point;

        }
        else
        {
            return cam.position + cam.transform.forward * maxDistance;
            
        }

    }
    public Vector3 GetGroundAimPoint(float maxDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, whatIsGround, QueryTriggerInteraction.Ignore))
        {
            return hit.point;

        }
        else
        {

            if (Physics.Raycast(cam.position + cam.transform.forward * maxDistance, -Vector3.up, out hit, maxDistance, whatIsGround, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
            else
            {
                return cam.position + cam.transform.forward * maxDistance + -Vector3.up*maxDistance;
            }

            
        }

    }

    private void ApplyStatusEffects()
    {
        if (player.cameraShake >= 0)
        {
            cam.gameObject.transform.localPosition += Random.insideUnitSphere * player.cameraShake / 2;
        }
        if (player.isStunned)
        {
            postProcessingBehaviour.profile = stunnedPPProfile;
        
        }
        else
        {
            postProcessingBehaviour.profile = defaultPPProfile;
        }
        
    }

}
