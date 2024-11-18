using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouthBlendShapeController : IFaceTracking
{
    FaceBlendShapeController faceBlendShapeController;
    CharacterCreationManager character;
    SkinnedMeshRenderer faceBlendShape;

    int mouthYIndex;
    int mouthXIndex;

    public MouthBlendShapeController(FaceBlendShapeController _faceBlendShapeController)
    {
        faceBlendShapeController = _faceBlendShapeController;
        character = faceBlendShapeController.character;
        faceBlendShape = faceBlendShapeController.faceBlendShape;
    }

    public void UpdateFaceTracking(float mouthX, float mouthY)
    {
        if (FaceLandmarkConnector.instance.isInFaceTrackingState)
        {
            if (character.faceBlendShapeDictionary.TryGetValue(FacePart.MouthX, out mouthXIndex))
                faceBlendShape.SetBlendShapeWeight(mouthXIndex, mouthX * 100);

            if (character.faceBlendShapeDictionary.TryGetValue(FacePart.MouthY, out mouthYIndex))
                faceBlendShape.SetBlendShapeWeight(mouthYIndex, mouthY * 100);
        }
        else
        {
            if (character.faceBlendShapeDictionary.TryGetValue(FacePart.MouthX, out mouthXIndex))
                faceBlendShape.SetBlendShapeWeight(mouthXIndex, 0);

            if (character.faceBlendShapeDictionary.TryGetValue(FacePart.MouthY, out mouthYIndex))
                faceBlendShape.SetBlendShapeWeight(mouthYIndex, 0);
        }
    }

}
