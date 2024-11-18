using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CVVTuber.UnityChan
{
    public class UnityChanFaceBlendShapeController : FaceAnimationController
    {
        [Header("[Target]")]

        public SkinnedMeshRenderer EYE_DEF;

        public SkinnedMeshRenderer EL_DEF;

        public SkinnedMeshRenderer BLW_DEF;

        public SkinnedMeshRenderer MTH_DEF;

        [Header("Debug Text")]
        [SerializeField] TMP_Text leftEyeText;
        [SerializeField] TMP_Text rightEyeText;
        [SerializeField] TMP_Text MouthYText;
        [SerializeField] TMP_Text MouthXText;
        [SerializeField] GameObject debugPos;
        [SerializeField] float smoothTime = 0.1f;
        float refEyeVelocity;
        float refMouthVelocity;
        Vector3 pos;

        #region CVVTuberProcess

        //Commented, because parent class disable all unity callbacks
        //protected override void Start()
        //{
        //    pos = new Vector3(pos.x + 0.3f, pos.y + 1.3f, pos.z);
        //}

        public override string GetDescription()
        {
            return "Update face BlendShape of UnityChan using FaceLandmarkGetter.";
        }

        public override void LateUpdateValue()
        {
            if (enableEye && EYE_DEF != null && EL_DEF != null)
            {
                EYE_DEF.SetBlendShapeWeight(6, EyeRightParam * 100);
                EL_DEF.SetBlendShapeWeight(6, EyeRightParam * 100);
            }

            if (enableBrow && BLW_DEF != null)
            {
                BLW_DEF.SetBlendShapeWeight(2, BrowRightParam * 100);
            }

            if (enableMouth && MTH_DEF != null)
            {
                if (MouthYParam >= 0.7f)
                {
                    MTH_DEF.SetBlendShapeWeight(0, MouthYParam * 80);
                    MTH_DEF.SetBlendShapeWeight(10, MouthYParam * 60);
                }
                else if (MouthYParam >= 0.25f)
                {
                    MTH_DEF.SetBlendShapeWeight(0, MouthYParam * 100);
                }
                else
                {
                    MTH_DEF.SetBlendShapeWeight(0, 0);
                    MTH_DEF.SetBlendShapeWeight(10, 0);
                }
            }
        }

        protected override void LateUpdateFaceAnimation()
        {
            throw new System.NotImplementedException();
        }

        #endregion


        #region FaceAnimationController

        public override void Setup()
        {
            base.Setup();

            NullCheck(EYE_DEF, "EYE_DEF");
            NullCheck(EL_DEF, "EL_DEF");
            NullCheck(BLW_DEF, "BLW_DEF");
            NullCheck(MTH_DEF, "MTH_DEF");
        }

        protected override void UpdateFaceAnimation(List<Vector2> points)
        {
            if (enableEye)
            {
                float eyeOpen = (GetLeftEyeOpenRatio(points) + GetRightEyeOpenRatio(points)) / 2.0f;
                //Debug.Log ("eyeOpen " + eyeOpen);
                leftEyeText.text = "Left Eye : " + GetLeftEyeOpenRatio(points).ToString();
                rightEyeText.text = "Right Eye : " + GetRightEyeOpenRatio(points).ToString();

                //if (eyeOpen >= 0.4f)
                //{
                //    eyeOpen = 1.0f;
                //}
                //else
                //{
                //    eyeOpen = 0.0f;
                //}
                //EyeParam = Mathf.Lerp(EyeParam, 1 - eyeOpen, eyeLeapT);
                EyeRightParam = Mathf.SmoothDamp(EyeRightParam, 1 - eyeOpen, ref refEyeVelocity, smoothTime);
            }

            if (enableBrow)
            {
                float browOpen = (GetLeftEyebrowUPRatio(points) + GetRightEyebrowUPRatio(points)) / 2.0f;
                //Debug.Log("browOpen " + browOpen);

                //if (browOpen >= 0.7f)
                //{
                //    browOpen = 1.0f;
                //}
                //else if (browOpen >= 0.3f)
                //{
                //    browOpen = 0.5f;
                //}
                //else
                //{
                //    browOpen = 0.0f;
                //}
                BrowRightParam = Mathf.Lerp(BrowRightParam, browOpen, smoothTime);
            }

            if (enableMouth)
            {
                float mouthOpen = GetMouthOpenYRatio(points);

                //Debug.Log("mouthOpen " + mouthOpen);

                if (mouthOpen < 0.4f)
                    mouthOpen = 0f;


                //if (mouthOpen >= 0.7f)
                //{
                //    mouthOpen = 1.0f;
                //}
                //else if (mouthOpen >= 0.25f)
                //{
                //    mouthOpen = 0.5f;
                //}
                //else
                //{
                //    mouthOpen = 0.0f;
                //}
                //MouthOpenParam = Mathf.Lerp(MouthOpenParam, mouthOpen, mouthLeapT);
                MouthYParam = Mathf.SmoothDamp(MouthYParam, mouthOpen, ref refMouthVelocity, smoothTime);
                MouthYText.text = "Mouth Y : " + GetMouthOpenYRatio(points).ToString();
                MouthXText.text = "Mouth X : " + GetMouthOpenXRatio(points).ToString();
            }

            pos.x = -points[27].x * 0.001f + 0.3f;
            debugPos.transform.position = pos;

        }

        #endregion
    }
}