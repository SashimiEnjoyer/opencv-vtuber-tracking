using UnityEngine;

public class EyesBlendShapeController : IFaceTracking
{
    FaceBlendShapeController faceBlendShapeController;
    CharacterCreationManager character;
    SkinnedMeshRenderer faceBlendShape;

    int rightEyeIndex;
    int leftEyeIndex;
    
    float countTimeBetweenBlink;
    float blinkEyeParamWhenNoTracking = 0;
    float timeBetweenBlink;
    bool isEyeClosed = false;
    bool isUsingBothEyes = false;

    public EyesBlendShapeController(FaceBlendShapeController _faceBlendShapeController)
    {
        faceBlendShapeController = _faceBlendShapeController;
        character = faceBlendShapeController.character;
        faceBlendShape = faceBlendShapeController.faceBlendShape;
    }

    public void UpdateFaceTracking(float eyeLeft, float eyeRight)
    {
        if (FaceLandmarkConnector.instance.isInFaceTrackingState)
        {

            if (!isUsingBothEyes)
            {
                //reset setelah autoblink
                if (faceBlendShape.GetBlendShapeWeight(character.faceBlendShapeDictionary[FacePart.BlinkEye]) != 0)
                    faceBlendShape.SetBlendShapeWeight(character.faceBlendShapeDictionary[FacePart.BlinkEye], 0);

                if (character.faceBlendShapeDictionary.TryGetValue(FacePart.LeftEye, out leftEyeIndex))
                    faceBlendShape.SetBlendShapeWeight(leftEyeIndex, eyeLeft * 100);

                if (character.faceBlendShapeDictionary.TryGetValue(FacePart.RightEye, out rightEyeIndex))
                    faceBlendShape.SetBlendShapeWeight(rightEyeIndex, eyeRight * 100);
            }
            else
                faceBlendShape.SetBlendShapeWeight(character.faceBlendShapeDictionary[FacePart.BlinkEye], ((eyeLeft + eyeRight) / 2) * 100);

        }
        else if (!FaceLandmarkConnector.instance.isInFaceTrackingState && !faceBlendShapeController.isUsingExpression)
        {
            AutoBlink();
        }

    }

    public void CheckEyes(bool _isLeftEye, bool _isRightEye) 
    {
        isUsingBothEyes = (!_isLeftEye || !_isRightEye);
    }

    void AutoBlink()
    {
        if (faceBlendShape.GetBlendShapeWeight(rightEyeIndex) > 0)
            faceBlendShape.SetBlendShapeWeight(rightEyeIndex, 0);

        if (faceBlendShape.GetBlendShapeWeight(leftEyeIndex) > 0)
            faceBlendShape.SetBlendShapeWeight(leftEyeIndex, 0);

        // Ngitung jeda waktu antar blink
        if (countTimeBetweenBlink < timeBetweenBlink)
        {
            countTimeBetweenBlink += Time.deltaTime;
        }
        // Kalo waktu counternya udah sama dengan max time between blink, lakukan blinking
        else
        {
            countTimeBetweenBlink = timeBetweenBlink;

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
                    blinkEyeParamWhenNoTracking -= Time.deltaTime * 2;
                }

                if (blinkEyeParamWhenNoTracking <= 0.2f)
                {
                    blinkEyeParamWhenNoTracking = 0;
                    isEyeClosed = false;


                    //Reset timer
                    timeBetweenBlink = UnityEngine.Random.Range(character.minTimer, character.maxTimer); // Random waktu jedanya
                    countTimeBetweenBlink = 0;
                }
            }
        }
        faceBlendShape.SetBlendShapeWeight(character.faceBlendShapeDictionary[FacePart.BlinkEye], blinkEyeParamWhenNoTracking * 100);
        
    }

}
