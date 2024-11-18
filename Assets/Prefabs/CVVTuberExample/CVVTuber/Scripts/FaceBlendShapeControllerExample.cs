using System.Collections.Generic;
using UnityEngine;

namespace CVVTuber
{
    public class FaceBlendShapeControllerExample : FaceAnimationController
    {
        [Header("[Target]")]

        public SkinnedMeshRenderer FACE_DEF;
        float smoothTime = 0.6f;


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Update face BlendShape using FaceLandmarkGetter.";
        }

        public override void LateUpdateValue()
        {
            if (FACE_DEF == null)
                return;

            if (enableEye)
            {
                FACE_DEF.SetBlendShapeWeight(0, EyeRightParam * 100);
                FACE_DEF.SetBlendShapeWeight(1, EyeRightParam * 100);
            }

            if (enableMouth)
            {
                if (MouthYParam >= 0.7f)
                {
                    FACE_DEF.SetBlendShapeWeight(2, MouthYParam * 100);
                }
                else if (MouthYParam >= 0.25f)
                {
                    FACE_DEF.SetBlendShapeWeight(2, MouthYParam * 80);
                }
                else
                {
                    FACE_DEF.SetBlendShapeWeight(2, 0);
                }
            }
        }

        #endregion


        #region FaceAnimationController

        public override void Setup()
        {
            base.Setup();

            NullCheck(FACE_DEF, "FACE_DEF");
        }

        protected override void UpdateFaceAnimation(List<Vector2> points)
        {
            if (enableEye)
            {
                float eyeOpen = (GetLeftEyeOpenRatio(points) + GetRightEyeOpenRatio(points)) / 2.0f;
                //Debug.Log("eyeOpen " + eyeOpen);

                if (eyeOpen >= 0.4f)
                {
                    eyeOpen = 1.0f;
                }
                else
                {
                    eyeOpen = 0.0f;
                }
                EyeRightParam = Mathf.Lerp(EyeRightParam, 1 - eyeOpen, smoothTime);
            }

            if (enableMouth)
            {
                float mouthOpen = GetMouthOpenYRatio(points);
                //Debug.Log("mouthOpen " + mouthOpen);

                if (mouthOpen >= 0.7f)
                {
                    mouthOpen = 1.0f;
                }
                else if (mouthOpen >= 0.25f)
                {
                    mouthOpen = 0.5f;
                }
                else
                {
                    mouthOpen = 0.0f;
                }
                MouthYParam = Mathf.Lerp(MouthYParam, mouthOpen, smoothTime);
            }
        }

        protected override void LateUpdateFaceAnimation()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}