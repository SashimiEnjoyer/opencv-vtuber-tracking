using System.Collections.Generic;
using UnityEngine;
using CVVTuber;
using TMPro;
using System;

public class CharacterFaceBlendShapeManager : FaceAnimationController
{
    
    [Header("[Blend Shape References]")]
    public SkinnedMeshRenderer blendShapeReference;
    [SerializeField] int rightEyeIndex = 43;
    [SerializeField] int leftEyeIndex = 42;
    [SerializeField] int blinkEyeIndex = 0; 
    [SerializeField] int rightBrowIndex = 69;
    [SerializeField] int leftBrowIndex = 70;
    [SerializeField] int mouthYIndex = 0;
    [SerializeField] int minMouthXIndex = 9;
    [SerializeField] int maxMouthXIndex = 27;

    [Header("[Settings]")]
    [SerializeField] [Range(0, 1)] float smoothTime;
    [SerializeField] [Range(0, 1)] float smoothMouth;
    float RightEyeOpen;
    float LeftEyeOpen;
    float mouthY;
    float mouthX;
    
    float blinkEyeParamWhenNoTracking;
    float countTimeBetweenBlink;
    float maxTimeBetweenBlink;
    bool isEyeClosed = false;

    [Header("[Debug Text]")]
    [SerializeField] TMP_Text mouthYText;
    [SerializeField] TMP_Text mouthXText;
    [SerializeField] TMP_Text rightEyeText;
    [SerializeField] TMP_Text leftEyeText;

    public override string GetDescription()
    {
        return "Update face BlendShape of UnityChan using FaceLandmarkGetter.";
    }

    protected override void LateUpdateFaceAnimation()
    {
    
        if (blendShapeReference != null && enableFaceTracking && FaceLandmarkConnector.instance.isInFaceTrackingState)
        {
            if (enableEye)
            {
                if (!isUsingBlinkEye)
                {
                    blendShapeReference.SetBlendShapeWeight(leftEyeIndex, EyeLeftParam * 100);
                    blendShapeReference.SetBlendShapeWeight(rightEyeIndex, EyeRightParam * 100);
                }
                else
                {
                    blendShapeReference.SetBlendShapeWeight(blinkEyeIndex, ((EyeLeftParam + EyeRightParam) / 2) * 100);
                }

                //rightEyeText.text = "Right Eye : " + EyeRightParam.ToString();
                //leftEyeText.text = "Left Eye : " + EyeLeftParam.ToString();
            }

            if (enableBrow)
            {
                blendShapeReference.SetBlendShapeWeight(rightBrowIndex, BrowRightParam * 100);
                blendShapeReference.SetBlendShapeWeight(leftBrowIndex, BrowLeftParam * 100);
            }

            if (enableMouth)
            {
                #region simple mouth shape

                if (MouthYParam > 0.1f)
                {

                    if (MouthXParam > 0.9f && MouthYParam <= 0.5f)
                    {
                        blendShapeReference.SetBlendShapeWeight(maxMouthXIndex, Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(maxMouthXIndex), 75, smoothMouth));

                        //blendShapeReference.SetBlendShapeWeight(mouthYIndex, Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(mouthYIndex), 0, smoothMouth));
                        blendShapeReference.SetBlendShapeWeight(mouthYIndex, 0);
                    }
                    else
                        blendShapeReference.SetBlendShapeWeight(maxMouthXIndex, Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(maxMouthXIndex), 0, smoothMouth));
                        blendShapeReference.SetBlendShapeWeight(mouthYIndex, MouthYParam * 100);
                }
                else
                {
                    blendShapeReference.SetBlendShapeWeight(mouthYIndex, 0);
                    blendShapeReference.SetBlendShapeWeight(maxMouthXIndex, 0);
                }
                

                #endregion
                #region old mouth logic. Vowel using A, I, O

                //if (MouthYParam > 0.3f)
                //{
                //    //blendShapeReference.SetBlendShapeWeight(minMouthXIndex, 0);

                //    if (MouthXParam > 0.4f)
                //    {
                //        if (MouthYParam > 0.7f)
                //            SetMouthShape(MouthYParam, 0, 0, 0, 0); //Vowel A
                //        else if (MouthYParam <= 0.7f)
                //            SetMouthShape(0, MouthXParam, 0, 0, 0); //Vowel I
                //    }
                //    else
                //    {
                //        SetMouthShape(0, 0, 0, 0, MouthYParam); // Vowel O
                //    }
                //}
                //else if (MouthYParam <= 0.3f)
                //{
                //    SetMouthShape(0, 0, 0, 0, 0);
                //}

                //blendShapeReference.SetBlendShapeWeight(minMouthXIndex, Mathf.Abs((MouthXParam * 100) - 100));

                #endregion
                //mouthYText.text = "Mouth Y : " + MouthYParam.ToString();
                //mouthXText.text = "Mouth X : " + MouthXParam.ToString();

            }

        }
    }

    public override void Setup()
    {
        base.Setup();

        NullCheck(blendShapeReference, "Blend shape reference");

        maxTimeBetweenBlink = UnityEngine.Random.Range(0.5f, 3f);
    }

    protected override void UpdateFaceAnimation(List<Vector2> points)
    {
        if (FaceLandmarkConnector.instance.isInFaceTrackingState)
        {
            countTimeBetweenBlink = 0;

            if (enableEye)
            {
                RightEyeOpen = GetRightEyeOpenRatio(points);
                LeftEyeOpen = GetLeftEyeOpenRatio(points);

                if (RightEyeOpen > 0.5f)
                    RightEyeOpen = 1f;
                else if (RightEyeOpen < 0.5f)
                    RightEyeOpen = 0f;

                if (LeftEyeOpen > 0.5f)
                    LeftEyeOpen = 1f;
                else if (LeftEyeOpen < 0.5f)
                    LeftEyeOpen = 0f;


                EyeRightParam = Mathf.Lerp(EyeRightParam, 1 - RightEyeOpen, /*ref refRightEyeVelocity,*/ smoothTime);
                EyeRightParam = (float)Math.Round(EyeRightParam, 2);
                EyeLeftParam = Mathf.Lerp(EyeLeftParam, 1 - LeftEyeOpen, /*ref refLeftEyeVelocity,*/ smoothTime);
                EyeLeftParam = (float)Math.Round(EyeLeftParam, 2);
                //EyeParam = Mathf.Lerp(EyeParam, 1 - eyeOpen, eyeLeapT);

            }

            if (enableBrow)
            {
                float rightBrowOpen = GetRightEyebrowUPRatio(points);
                float leftBrowOpen = GetLeftEyebrowUPRatio(points);

                BrowRightParam = Mathf.Lerp(BrowRightParam, rightBrowOpen, /*ref refRightBrow,*/ smoothTime);
                BrowLeftParam = Mathf.Lerp(BrowLeftParam, leftBrowOpen, /*ref refLeftBrow,*/ smoothTime);
                //BrowRightParam = Mathf.Lerp(BrowRightParam, rightBrowOpen, browLeapT);
                //BrowLeftParam = Mathf.Lerp(BrowLeftParam, leftBrowOpen, browLeapT);
            }

            if (enableMouth)
            {
                mouthY = GetMouthOpenYRatio(points);
                mouthX = GetMouthOpenXRatio(points);

                if (mouthY < 0.3f)
                    mouthY = 0f;

                MouthYParam = Mathf.Lerp(MouthYParam, mouthY, /*ref refMouthYVelocity,*/ smoothTime);
                MouthYParam = (float)Math.Round(MouthYParam, 2);
                //MouthYParam = Mathf.Clamp(MouthYParam,0, 0.8f);
                //MouthOpenParam = Mathf.Lerp(MouthOpenParam, mouthOpen, mouthLeapT);

                MouthXParam = Mathf.Lerp(MouthXParam, mouthX, /*ref refMouthXVelocity,*/ smoothTime);
                MouthXParam = (float)Math.Round(MouthXParam, 2);
                //MouthXParam = Mathf.Clamp(MouthXParam, 0, 0.8f);
            }
        }
        else
        {
            // Ngitung jeda waktu antar blink
            if(countTimeBetweenBlink < maxTimeBetweenBlink)
            {
                countTimeBetweenBlink += Time.deltaTime;
            }
            // Kalo waktu counternya udah sama dengan max time between blink, lakukan blinking
            else
            {
                countTimeBetweenBlink = maxTimeBetweenBlink;

                // Waktu mata kebuka, perlahan mata nutup. Yang ini cuma gerakin parameter buat blendshape nanti 
                if (!isEyeClosed)
                {
                    if (blinkEyeParamWhenNoTracking < 0.9f)
                    {
                        blinkEyeParamWhenNoTracking += Time.deltaTime * 4;
                    }

                    if (blinkEyeParamWhenNoTracking >= 0.9f)
                    {
                        blinkEyeParamWhenNoTracking = 1;
                        isEyeClosed = true;
                    }
                }
                //Waktu mata ketutup, perlahan mata kebuka. Yang ini cuma gerakin parameter buat blendshape nanti
                else
                {
                    if (blinkEyeParamWhenNoTracking >= 0.9f || blinkEyeParamWhenNoTracking > 0.2f)
                    {
                        blinkEyeParamWhenNoTracking -= Time.deltaTime * 4;
                    }

                    if (blinkEyeParamWhenNoTracking <= 0.2f)
                    {
                        blinkEyeParamWhenNoTracking = 0;
                        isEyeClosed = false;


                        //Reset timer
                        maxTimeBetweenBlink = UnityEngine.Random.Range(0.5f, 3f); // Random waktu jedanya
                        countTimeBetweenBlink = 0;
                    }
                }
            }
            // Set blink blendshape
            blendShapeReference.SetBlendShapeWeight(blinkEyeIndex, blinkEyeParamWhenNoTracking * 100);
        }

    }

    //Attached to toggle UI
    public void SetTrackingFace(bool _isOn)
    {
        enableFaceTracking = _isOn;
    }


    //void SetMouthShape(float a, float i, float u, float e, float o)
    //{

    //    if (a > 0)
    //    {
    //        blendShapeReference.SetBlendShapeWeight(1, blendShapeReference.GetBlendShapeWeight(1) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(1), 0, smoothMouth * 4) * 100 : 0);
    //        blendShapeReference.SetBlendShapeWeight(0, blendShapeReference.GetBlendShapeWeight(0) < a * 100 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(0), a * 100, smoothMouth) : a * 100);
    //        //blendShapeReference.SetBlendShapeWeight(2, blendShapeReference.GetBlendShapeWeight(2) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(2), 0, smoothMouth) * 100 : 0);
    //        //blendShapeReference.SetBlendShapeWeight(3, blendShapeReference.GetBlendShapeWeight(3) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(3), 0, smoothMouth) * 100 : 0);
    //        blendShapeReference.SetBlendShapeWeight(4, blendShapeReference.GetBlendShapeWeight(4) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(4), 0, smoothMouth) * 100 : 0);
    //        return;
    //    }
    //    else if (i > 0 && a <= 0 && o <= 0)
    //    {
    //        blendShapeReference.SetBlendShapeWeight(1, blendShapeReference.GetBlendShapeWeight(1) < i * 100 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(1), i * 100, smoothMouth) : i * 100);
    //        blendShapeReference.SetBlendShapeWeight(0, blendShapeReference.GetBlendShapeWeight(0) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(0), 0, smoothMouth * 4) * 100 : 0);
    //        //blendShapeReference.SetBlendShapeWeight(2, blendShapeReference.GetBlendShapeWeight(2) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(2), 0, smoothMouth) * 100 : 0);
    //        //blendShapeReference.SetBlendShapeWeight(3, blendShapeReference.GetBlendShapeWeight(3) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(3), 0, smoothMouth) * 100 : 0);
    //        blendShapeReference.SetBlendShapeWeight(4, blendShapeReference.GetBlendShapeWeight(4) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(4), 0, smoothMouth * 4) * 100 : 0);
    //        return;
    //    }
    //    //else if (u > 0)
    //    //{
    //    //    blendShapeReference.SetBlendShapeWeight(2, u * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(0, Mathf.Lerp(a, 0, smoothTime) * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(1, Mathf.Lerp(i, 0, smoothTime) * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(3, Mathf.Lerp(e, 0, smoothTime) * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(4, Mathf.Lerp(o, 0, smoothTime) * 100);
    //    //}
    //    //else if (e > 0)
    //    //{
    //    //    blendShapeReference.SetBlendShapeWeight(3, e * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(0, Mathf.Lerp(a, 0, smoothTime) * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(1, Mathf.Lerp(i, 0, smoothTime) * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(2, Mathf.Lerp(u, 0, smoothTime) * 100);
    //    //    blendShapeReference.SetBlendShapeWeight(4, Mathf.Lerp(o, 0, smoothTime) * 100);
    //    //}
    //    else if (o > 0)
    //    {
    //        blendShapeReference.SetBlendShapeWeight(0, blendShapeReference.GetBlendShapeWeight(0) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(0), 0, smoothMouth) * 100 : 0);
    //        blendShapeReference.SetBlendShapeWeight(4, blendShapeReference.GetBlendShapeWeight(4) < o * 100 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(4), o * 100, smoothMouth) : o * 100);
    //        blendShapeReference.SetBlendShapeWeight(1, blendShapeReference.GetBlendShapeWeight(1) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(1), 0, smoothMouth * 4) * 100 : 0);
    //        //blendShapeReference.SetBlendShapeWeight(2, blendShapeReference.GetBlendShapeWeight(2) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(2), 0, smoothMouth) * 100 : 0);
    //        //blendShapeReference.SetBlendShapeWeight(3, blendShapeReference.GetBlendShapeWeight(3) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(3), 0, smoothMouth) * 100 : 0);
    //        return;
    //    }

    //    blendShapeReference.SetBlendShapeWeight(0, blendShapeReference.GetBlendShapeWeight(0) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(0), 0, smoothMouth) * 100 : 0);
    //    blendShapeReference.SetBlendShapeWeight(1, blendShapeReference.GetBlendShapeWeight(1) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(1), 0, smoothMouth * 4) * 100 : 0);
    //    //blendShapeReference.SetBlendShapeWeight(2, blendShapeReference.GetBlendShapeWeight(2) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(2), 0, smoothMouth) * 100 : 0);
    //    //blendShapeReference.SetBlendShapeWeight(3, blendShapeReference.GetBlendShapeWeight(3) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(3), 0, smoothMouth) * 100 : 0);
    //    blendShapeReference.SetBlendShapeWeight(4, blendShapeReference.GetBlendShapeWeight(4) > 0 ? Mathf.Lerp(blendShapeReference.GetBlendShapeWeight(4), 0, smoothMouth) * 100 : 0);
    //}
}
