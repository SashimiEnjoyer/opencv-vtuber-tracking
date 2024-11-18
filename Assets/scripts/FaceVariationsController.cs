
using System.Collections.Generic;
using UnityEngine;

public class FaceVariationsController
{
    List<int> currentExpression = new List<int>();
    List<int> currentToggleExpression = new List<int>();
    SkinnedMeshRenderer faceBlendShape;
    CharacterCreationManager character;
    FaceBlendShapeController faceBlendShapeController;
    bool defaultEnableBrow;
    bool defaultEnableMouth;
    bool defaultEnableEyes;


    public FaceVariationsController(FaceBlendShapeController _faceBlendShapeController)
    {
        faceBlendShapeController = _faceBlendShapeController;
        character = faceBlendShapeController.character;
        faceBlendShape = faceBlendShapeController.faceBlendShape;
    }

    public void InitializeFaceVariations()
    {
        defaultEnableEyes = faceBlendShapeController.enableEye;
        defaultEnableBrow = faceBlendShapeController.enableBrow;
        defaultEnableMouth = faceBlendShapeController.enableMouth;
    }

    public void ImplementFaceChanges(int _index, bool _isUsingToggle)
    {
        if (_index >= character.faceVariations.Length || character.faceVariations.Length <= 0 )
        {
            DebugExt.LogWarning("Expression index is not available or Expression list is empty");
            return;
        }

        if (_isUsingToggle && _index >= character.toggleableFaceVariations.Length || character.toggleableFaceVariations.Length <= 0)
            return;


        if (!_isUsingToggle)
        {
            ResetExpression();
            faceBlendShapeController.UseFacePart(character.faceVariations[_index].useEyes, character.faceVariations[_index].useMouth, character.faceVariations[_index].useBrows);
            SetExpression(_index);
        }
        else
            SetToggleExpression(_index);

    }


    public void ResetAllExpression()
    {
        foreach (int i in currentExpression)
        {
            faceBlendShape.SetBlendShapeWeight(i, 0);
        }

        foreach (int i in currentToggleExpression)
        {
            faceBlendShape.SetBlendShapeWeight(i, 0);
        }

        faceBlendShapeController.enableBrow = defaultEnableBrow;
        faceBlendShapeController.enableMouth = defaultEnableMouth;
        faceBlendShapeController.enableEye = defaultEnableEyes;
        currentExpression.Clear();
        currentToggleExpression.Clear();
    }

    void ResetExpression()
    {
        foreach (int i in currentExpression)
        {
            faceBlendShape.SetBlendShapeWeight(i, 0);
        }
        faceBlendShapeController.enableBrow = defaultEnableBrow;
        faceBlendShapeController.enableMouth = defaultEnableMouth;
        faceBlendShapeController.enableEye = defaultEnableEyes;
        currentExpression.Clear();
    }

    void SetExpression(int index)
    {
        foreach (int i in character.faceVariations[index].blendShapeIndex)
        {
            faceBlendShape.SetBlendShapeWeight(i, 100);
            currentExpression.Add(i);
        }
    }

    void SetToggleExpression(int index)
    {
        faceBlendShape.SetBlendShapeWeight(character.toggleableFaceVariations[index].blendShapeIndex, 
            faceBlendShape.GetBlendShapeWeight(character.toggleableFaceVariations[index].blendShapeIndex) == 0 ? 100 : 0);

        currentToggleExpression.Add(character.toggleableFaceVariations[index].blendShapeIndex);
    }

}
