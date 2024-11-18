
using UnityEngine;
using Leap.Unity;
//using TMPro;
using RootMotion.FinalIK;

enum HandType { Left, Right}

public class HandUpdateManager : MonoBehaviour
{
    [SerializeField] HandModelBase handModelBase;

    [SerializeField] FullBodyBipedIK fbbik; 

    [SerializeField] HandType handType;
    //[SerializeField] Animator animator;

    //[SerializeField] BothHandManager bothHand;
    //[SerializeField] Avatar withHand;
    //[SerializeField] Avatar noHand;
    //[SerializeField] float smoothing;
    //[SerializeField] TMP_Text debugText;
    //[SerializeField] string activeHandMessage;
    //[SerializeField] string deActiveHandMessage;

    bool isActive = false;

    float transitionValue;

    private void Awake()
    {

        AssignVariables();

        //handModelBase.OnBegin -= HandActive;
        handModelBase.OnBegin += HandActive;

        //handModelBase.OnFinish -= HandNotActive;
        handModelBase.OnFinish += HandNotActive;
    }

    private void OnDestroy()
    {
        handModelBase.OnBegin -= HandActive;
        handModelBase.OnFinish -= HandNotActive;
    }

    private void Start()
    {
        if (fbbik == null)
            fbbik = FindObjectOfType<FullBodyBipedIK>();

    }

    private void Update()
    {
        UpdateHands();
    }

    public void AssignVariables()
    {
        if (handModelBase == null)
            handModelBase = GetComponent<HandModelBase>();

        if (fbbik == null)
            fbbik = GetComponentInParent<FullBodyBipedIK>();

    }

    public void SetHandType(bool _isLeft)
    {
        if (_isLeft)
            handType = HandType.Left;
        else
            handType = HandType.Right;
    }    

    void UpdateHands()
    {

        if (isActive)
        {
            if(transitionValue < 1)
                transitionValue += Time.deltaTime * 4;

            if (transitionValue < 0)
                transitionValue = 0;

        }
        else
        {
            if(transitionValue > 0)
                transitionValue -= Time.deltaTime * 2;

            if (transitionValue > 1)
                transitionValue = 1;
        }

        transitionValue = Mathf.Clamp(transitionValue, 0, 1);

        switch (handType)
        {
            case HandType.Left:

                fbbik.solver.leftHandEffector.positionWeight = transitionValue;
                fbbik.solver.leftHandEffector.rotationWeight = transitionValue;

                //if (fbbik.solver.leftArmChain.bendConstraint.bendGoal != null)
                    fbbik.solver.leftArmChain.bendConstraint.weight = transitionValue;

                break;
            case HandType.Right:

                fbbik.solver.rightHandEffector.positionWeight = transitionValue;
                fbbik.solver.rightHandEffector.rotationWeight = transitionValue;

                //if (fbbik.solver.rightArmChain.bendConstraint.bendGoal != null)
                    fbbik.solver.rightArmChain.bendConstraint.weight = transitionValue;

                break;
        }

    }
    

    void HandActive()
    {
        isActive = true;
        //if (bothHand.handCount < 2)
        //    bothHand.handCount += 1;
        //if(animator.avatar == withHand)
        //    animator.avatar = noHand;

        //debugText.text = activeHandMessage;
    }

    void HandNotActive()
    {
        isActive = false;
        //if (bothHand.handCount > 0)
        //    bothHand.handCount -= 1;
        //if(animator.avatar == noHand)
        //    animator.avatar = withHand;

        //debugText.text = deActiveHandMessage;
    }
}
