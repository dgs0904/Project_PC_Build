using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using BNG;

public class ExpositGrabInteractable : MonoBehaviour
{
    private Transform originalParent;
    public Collider RighthandColider;
    public Collider LefthandColider;
    public Transform RightAttachTransform;
    public Transform LeftAttachTransform;
    private Collider DropColider;
    public GameObject DropPoint;
    
    [Header("Settings")]
    private bool isGrabbed = false;
    public bool UseAttachTransform = false;
    public bool DropWithAnimation = true;
    public float GhostEnableTime = 3f;

    [Header("Hands")]
    public GameObject BNGRightHand;
    public GameObject BNGLeftHand;
    public GameObject ObjectSanpRightHand;
    public GameObject ObjectSanpLeftHand;

    [Header("Haptic Settings")]
    private float lerpDuration = 1f;
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;
    [Space]
    public UnityEvent OnGrab;
    public UnityEvent OnRelese;
    public UnityEvent FastRelese;
    [Space]
    bool isRightHandGrab = false;
    bool isLeftHandGrab = false;


    void Start()
    {
        ObjectSanpLeftHand.SetActive(false);
        ObjectSanpRightHand.SetActive(false);
        originalParent = DropPoint.transform.parent; // Store the original parent of the object
        DropPoint.SetActive(false);
        DropColider = DropPoint.GetComponent<Collider>();
       

    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the other object is tagged as "Hand"
        if (other == RighthandColider)
        {
            if (!isGrabbed)
            {
                StartCoroutine(Grab(other.transform));
                StartCoroutine(OnDropObjectLate());
                isRightHandGrab = true;

                BNGRightHand.SetActive(false);
                ObjectSanpRightHand.SetActive(true);
                RighthandColider.enabled = false;
               
                

                // Trigger haptic feedback
                TriggerHapticFeedback(true);
            }
        }
        if (other == LefthandColider)
        {
            if (!isGrabbed)
            {
                StartCoroutine(Grab(other.transform));
                StartCoroutine(OnDropObjectLate());
                isLeftHandGrab = true;

                BNGLeftHand.SetActive(false);
                ObjectSanpLeftHand.SetActive(true);
                LefthandColider.enabled = false;


                // Trigger haptic feedback
                TriggerHapticFeedback(false);
            }
        }
        if (other == DropColider)
        {
            FaaaastRelese();

            if (isGrabbed)
            {
                if (isRightHandGrab)
                {
                    StartCoroutine(Release());
                    DropPoint.SetActive(false);
                    Debug.Log("function called relese");
                    BNGRightHand.SetActive(true);
                    ObjectSanpRightHand.SetActive(false);
                    RighthandColider.enabled = true;
                }
                if (isLeftHandGrab)
                {
                    StartCoroutine(Release());
                    DropPoint.SetActive(false);
                    Debug.Log("function called relese");
                    BNGLeftHand.SetActive(true);
                    ObjectSanpLeftHand.SetActive(false);
                    LefthandColider.enabled = true;
                }
                
               
            }
        }
    }

    IEnumerator Grab(Transform hand)
    {
        isGrabbed = true;
        transform.SetParent(hand); // Make the object a child of the hand
        GetComponent<Rigidbody>().isKinematic = true; // Make it kinematic so it moves with the hand
        OnGrab.Invoke();
   
        float elapsedTime = 0f;
        if (UseAttachTransform)
        {
            while (elapsedTime < lerpDuration)
            {
                if (isRightHandGrab)
                {
                    transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, RightAttachTransform.transform.localPosition, elapsedTime / lerpDuration);
                    transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, RightAttachTransform.transform.localRotation, elapsedTime / lerpDuration);
                   
                }
                if (isLeftHandGrab)
                {
                    transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, LeftAttachTransform.transform.localPosition, elapsedTime / lerpDuration);
                    transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, LeftAttachTransform.transform.localRotation, elapsedTime / lerpDuration);
                 
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            transform.localPosition = Vector3.zero; // Optionally, reset the position relative to the hand
            transform.localRotation = Quaternion.identity; // Optionally, reset the rotation relative to the hand
        }
    }
    private void Update()
    {
        if (isRightHandGrab)
        {
            isLeftHandGrab = false;
        }
        if (isLeftHandGrab)
        {
            isRightHandGrab = false;
        }
    }

    IEnumerator Release()
    {
        float elapsedTime = 0f;

        StartCoroutine(OnGrabBoolAgine());
        transform.SetParent(originalParent); // Restore the original parent
        GetComponent<Rigidbody>().isKinematic = true; // Make it non-kinematic again

        if (DropWithAnimation)
        {
            while (elapsedTime < lerpDuration)
            {
                transform.position = Vector3.Lerp(gameObject.transform.position, DropPoint.transform.position, elapsedTime / lerpDuration);
                transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, DropPoint.transform.rotation, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
                isRightHandGrab = false;
                isLeftHandGrab = false;
            }
        }
        else
        {
            gameObject.transform.localRotation = DropPoint.transform.localRotation;
            gameObject.transform.localPosition = DropPoint.transform.localPosition;
        }
        OnRelese.Invoke();
    }

    IEnumerator OnGrabBoolAgine()
    {
        yield return new WaitForSeconds(3);
        isGrabbed = false;
    }

    IEnumerator OnDropObjectLate()
    {
        yield return new WaitForSeconds(GhostEnableTime);
        DropPoint.SetActive(true);
    }

    public void FaaaastRelese()
    {
        FastRelese.Invoke();
     
    }

    private void TriggerHapticFeedback(bool isRightHand)
    {
        if (isRightHand)
        {
            InputBridge.Instance.VibrateController(0, hapticIntensity, hapticDuration, ControllerHand.Right);
        }
        else
        {
            InputBridge.Instance.VibrateController(0, hapticIntensity, hapticDuration, ControllerHand.Left);
        }
    }
}
