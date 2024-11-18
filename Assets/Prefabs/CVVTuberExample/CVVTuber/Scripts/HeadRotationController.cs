using UnityEngine;
using System.Collections.Generic;
using RootMotion.FinalIK;
using TMPro;

namespace CVVTuber
{
    public class HeadRotationController : CVVTuberProcess
    {
        [Header("[Input]")]

        [SerializeField]
        protected CVVTuberProcess headRotationGetter;

        protected IHeadRotationGetter _headRotationGetterInterface = null;
        protected IHeadPositionGetter _headPositionGetterInterface = null;

        protected IHeadRotationGetter headRotationGetterInterface
        {
            get
            {
                if (headRotationGetter != null && _headRotationGetterInterface == null)
                    _headRotationGetterInterface = headRotationGetter.GetComponent<IHeadRotationGetter>();
                return _headRotationGetterInterface;
            }
        }

        protected IHeadPositionGetter headPositionGetterInterface
        {
            get
            {
                if (headRotationGetter != null && _headPositionGetterInterface == null)
                    _headPositionGetterInterface = headRotationGetter.GetComponent<IHeadPositionGetter>();
                return _headPositionGetterInterface;
            }
        }

        [SerializeField] FBBIKHeadEffector headEffector;

        [Header("[Setting]")]

        [SerializeField] Vector3 offsetAngle;
        [SerializeField] Vector3 _offsetPosition;

        [SerializeField] bool invertXAxis;

        [SerializeField] bool invertYAxis;

        [SerializeField] bool invertZAxis;

        [SerializeField] bool rotateXAxis;

        [SerializeField] bool rotateYAxis;

        [SerializeField] bool rotateZAxis;

        [SerializeField] bool useSmoothing;

        [SerializeField] bool canMovePos = true;

        [SerializeField] float timeBeforeResetHead;

        [Range(0, 1f)]
        public float rotationSmoothing = 0.1f;

        [Range(0, 1f)]
        public float positionSmoothing = 0.1f;

        [Header("[Target]")]

        public Transform headEffectorTransform;

        protected Vector3 headEulerAngles;

        protected Vector3 oldHeadEulerAngle;

        float refRotateXVelocity;
        float refRotateYVelocity;
        float refRotateZVelocity;

        Vector3 targetPos;
        [SerializeField] Vector3 defaultPos;

        public Vector3 offset
        {
            get { return _offsetPosition; }
            set { _offsetPosition = value; }
        }

        //bool isTrackingFace;
        float timer;


        #region CVVTuberProcess

        private void Awake()
        {
            if (headEffector == null)
                headEffector = FindObjectOfType<FBBIKHeadEffector>();
            
            if (headEffectorTransform == null)
                headEffectorTransform = headEffector.transform;

        }

        public override string GetDescription()
        {
            return "Update head rotation of target transform using HeadRotationGetter.";
        }

        public override void Setup()
        {
            NullCheck(headRotationGetterInterface, "headRotationGetter");

            if (headEffectorTransform != null)
            {
                oldHeadEulerAngle = headEffectorTransform.eulerAngles;
            }
            else
            {
                headEffectorTransform = FindObjectOfType<FBBIKHeadEffector>().gameObject.transform;
            }
        }

        public override void UpdateValue()
        {
            if (!FaceLandmarkConnector.instance.isInFaceTrackingState || !canMovePos)
            {
                if (timer < timeBeforeResetHead)
                {
                    timer += Time.deltaTime;
                }

                if (timer >= timeBeforeResetHead)
                {
                    timer = timeBeforeResetHead;

                    if (headEffector != null)
                    {
                        if (headEffector.positionWeight > 0)
                            headEffector.positionWeight = Mathf.Lerp(headEffector.positionWeight, 0, 0.1f);

                        if (headEffector.rotationWeight > 0)
                            headEffector.rotationWeight = Mathf.Lerp(headEffector.rotationWeight, 0, 0.1f);
                    }

                    headEffectorTransform.position = Vector3.Lerp(headEffectorTransform.localPosition, defaultPos, 0.1f);

                    headEffectorTransform.rotation = Quaternion.Lerp(headEffectorTransform.rotation, Quaternion.Euler(Vector3.zero), 0.1f);
                }
            }
            else
            {
                if (headEffector != null)
                {
                    if (headEffector.positionWeight < 1)
                        headEffector.positionWeight = Mathf.Lerp(headEffector.positionWeight, 1, 0.1f);

                    if (headEffector.rotationWeight < 1)
                        headEffector.rotationWeight = Mathf.Lerp(headEffector.rotationWeight, 1, 0.1f);
                }
                timer = 0;
            }
        }

        public override void LateUpdateValue()
        {
            if (headRotationGetterInterface == null)
                return;
            if (headEffectorTransform == null)
                return;
            if (!FaceLandmarkConnector.instance.isInFaceTrackingState)
                return;


            if (FaceLandmarkConnector.instance.landmarkPoints != null && canMovePos)
            {
                targetPos.x = headPositionGetterInterface.GetHeadPosition().x + offset.x;
                targetPos.z = -headPositionGetterInterface.GetHeadPosition().z + offset.z + 0.9f;
                targetPos.y = headPositionGetterInterface.GetHeadPosition().y + offset.y + 1.5f;
                //targetPos += ;

                headEffectorTransform.position = Vector3.Lerp(headEffectorTransform.position, targetPos, positionSmoothing);
                //debugText.text = headEffectorTransform.position.ToString();
            }

            if (headRotationGetterInterface.GetHeadEulerAngles() != Vector3.zero)
            {
                headEulerAngles = headRotationGetterInterface.GetHeadEulerAngles();

                headEulerAngles = new Vector3(headEulerAngles.x + offsetAngle.x, headEulerAngles.y + offsetAngle.y, headEulerAngles.z + offsetAngle.z);
                headEulerAngles = new Vector3(invertXAxis ? -headEulerAngles.x : headEulerAngles.x, invertYAxis ? -headEulerAngles.y : headEulerAngles.y, invertZAxis ? -headEulerAngles.z : headEulerAngles.z);
                headEulerAngles = Quaternion.Euler(rotateXAxis ? 90 : 0, rotateYAxis ? 90 : 0, rotateZAxis ? 90 : 0) * headEulerAngles;
            }

            if (useSmoothing)
            {
                headEffectorTransform.eulerAngles = new Vector3(Mathf.SmoothDampAngle(oldHeadEulerAngle.x, headEulerAngles.x, ref refRotateXVelocity,rotationSmoothing), // X Axis 
                                                 Mathf.SmoothDampAngle(oldHeadEulerAngle.y, headEulerAngles.y, ref refRotateYVelocity, rotationSmoothing), // Y Axis
                                                 Mathf.SmoothDampAngle(oldHeadEulerAngle.z, headEulerAngles.z, ref refRotateZVelocity, rotationSmoothing)); // Z Axis
            }
            else
            {
                headEffectorTransform.localEulerAngles = headEulerAngles;
            }

            oldHeadEulerAngle = headEffectorTransform.eulerAngles;
        }

        #endregion
    }
}