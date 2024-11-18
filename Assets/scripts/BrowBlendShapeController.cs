
using UnityEngine;

public class BrowBlendShapeController : IFaceTracking
{
    FaceBlendShapeController faceBlendShapeController;
    CharacterCreationManager character;
    SkinnedMeshRenderer faceBlendShape;

    int leftBrow;
    int rightBrow;

    public BrowBlendShapeController(FaceBlendShapeController _faceBlendShapeController)
    {
        faceBlendShapeController = _faceBlendShapeController;
        character = faceBlendShapeController.character;
        faceBlendShape = faceBlendShapeController.faceBlendShape;
    }

    public void UpdateFaceTracking(float _leftBrow, float _rightBrow)
    {
        if (character.faceBlendShapeDictionary.TryGetValue(FacePart.LeftBrow, out leftBrow))
            faceBlendShape.SetBlendShapeWeight(leftBrow, _leftBrow * 100);

        if (character.faceBlendShapeDictionary.TryGetValue(FacePart.RightBrow, out rightBrow))
            faceBlendShape.SetBlendShapeWeight(rightBrow, _rightBrow * 100);
    }
}
