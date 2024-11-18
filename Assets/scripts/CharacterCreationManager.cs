
using System.Collections.Generic;
using UnityEngine;

public enum FacePart { RightEye, LeftEye, BlinkEye, MouthX, MouthY, RightBrow, LeftBrow}

[CreateAssetMenu(fileName ="Character Creation", menuName = "Create Character/Character")]
public class CharacterCreationManager : ScriptableObject
{
    [System.Serializable]
    public struct FaceBlendShapeSettings
    {
        public FacePart facePart;
        public int blendShapeIndex;
    }

    [System.Serializable]
    public struct FaceBlendShapeVariations
    {
        public string variationName;
        public int[] blendShapeIndex;
        public bool useEyes;
        public bool useMouth;
        public bool useBrows;
    }

    [System.Serializable]
    public struct ToggleableFaceVariations
    {
        public string toggleableVariationName;
        public int blendShapeIndex;
    }

   public FaceBlendShapeSettings[] baseBlendShapeSetUp;
   public FaceBlendShapeVariations[] faceVariations;
    public ToggleableFaceVariations[] toggleableFaceVariations;

    //public bool isUsingHeadEffector = false;

    [Header("Smoothing Settings")]
    [Range(0.1f, 1)]
    public float eyeSmoothing = 0.1f;

    [Range(0.1f, 1)]
    public float mouthSmoothing = 0.1f;

    [Header("Auto-Blink Settings")]
    public float minTimer = 3;
    public float maxTimer = 7;
    
    public Dictionary<FacePart, int> faceBlendShapeDictionary = new Dictionary<FacePart, int>();

    [Header("Additional Prefabs")]
    public GameObject handManagerPrefab;
    public GameObject leapServicesPrefab;

    [ContextMenu("Set Up Blend Shape Dictionary")]
    public void SetUpFaceBlendShape()
    {
        SetUpFacePart();
    }

    [ContextMenu("Reset Blend Shape Dictionary")]
    public void ResetFaceBlendShape()
    {
        faceBlendShapeDictionary.Clear();
        DebugExt.Log("Reset Done");
    }


    void SetUpFacePart()
    {
        if(faceBlendShapeDictionary.Count > 0)
            faceBlendShapeDictionary.Clear();
        

        for (int i = 0; i < baseBlendShapeSetUp.Length; i++)
        {
            if (!faceBlendShapeDictionary.ContainsKey(baseBlendShapeSetUp[i].facePart))
            {
                faceBlendShapeDictionary.Add(baseBlendShapeSetUp[i].facePart, baseBlendShapeSetUp[i].blendShapeIndex);
                DebugExt.Log(baseBlendShapeSetUp[i].facePart + " Assigned!");
            }
            else 
            {
                DebugExt.Log(baseBlendShapeSetUp[i].facePart + " Is Already Exist");
            }
        }
    }

}
