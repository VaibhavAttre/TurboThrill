using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]

    private InputAction action;

    [SerializeField]
    private CinemachineVirtualCamera mainCamA;

    [SerializeField]
    private CinemachineVirtualCamera adsCamA;


    private Animator animator;
    public Camera normalCam;
    public Camera adsCam;
    private bool mainCam = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    void Start()
    {
        action.performed += _ => SwitchState();
    }

    private void SwitchState()
    {
        if (mainCam)
        {
            animator.Play("ADSCam");
            //normalCam.gameObject.SetActive(false);
            //adsCam.gameObject.SetActive(true);
        } else
        {
            animator.Play("NormalCam");
            //normalCam.gameObject.SetActive(true);
            //adsCam.gameObject.SetActive(false);
        }
        mainCam = !mainCam;
    }

    private void SwitchPriority()
    {
        if (mainCam)
        {
            mainCamA.Priority = 0;
            adsCamA.Priority = 1;
        }
        else
        {
            mainCamA.Priority = 1;
            adsCamA.Priority = 0;
        }
        mainCam = !mainCam;
    }
}
