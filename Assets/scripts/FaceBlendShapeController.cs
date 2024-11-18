
using System.Collections.Generic;
using UnityEngine;
using System;

public class FaceBlendShapeController : FacePointsProcess
{
    [Header("BlendShape Set Up")]
    [SerializeField] HalfBodyTrackingCharacterSetUp halfBody;

    [HideInInspector] public CharacterCreationManager character;
    [HideInInspector] public SkinnedMeshRenderer faceBlendShape;

    MouthBlendShapeController mouthController;
    EyesBlendShapeController eyeController;
    BrowBlendShapeController browController;
    FaceVariationsController expressionController;

    [Header("[Setting]")]

    public bool enableFaceTracking = true;
    public bool enableBrow = true;
    public bool enableEye;
    public bool enableMouth = true;
    public bool isUsingBlinkEye = false;
    public bool isUsingExpression = false;

    public float mouthSmoothing = 0.1f;
    public float eyeSmoothing = 0.1f;

    private void Awake()
    {
        if (halfBody == null)
            halfBody = FindObjectOfType<HalfBodyTrackingCharacterSetUp>();

        character = halfBody.character;
        faceBlendShape = halfBody.faceBlendShape;

        character.SetUpFaceBlendShape();

    }

    private void OnDisable()
    {
        halfBody.onFaceChange -= UseExpression;
        halfBody.onFaceReset -= ResetExpression;
    }

    public override string GetDescription()
    {
        return "Face Blendshape Controller";
    }

    public override void Setup()
    {

        if (mouthController == null)
            mouthController = new MouthBlendShapeController(this);

        if (eyeController == null)
            eyeController = new EyesBlendShapeController(this);

        if (browController == null)
            browController = new BrowBlendShapeController(this);

        if (expressionController == null)
            expressionController = new FaceVariationsController(this);

        expressionController.InitializeFaceVariations();

        halfBody.onFaceChange += UseExpression;
        halfBody.onFaceReset += ResetExpression;

        eyeController.CheckEyes(character.faceBlendShapeDictionary.ContainsKey(FacePart.LeftEye), character.faceBlendShapeDictionary.ContainsKey(FacePart.RightEye));
    }

    protected override void UpdateFaceBlendShapeParameter(List<Vector2> points)
    {
        if (!enableFaceTracking)
            return;

        if (enableEye)
        {
            float RightEyeOpen = GetEyeOpenRatio(false);
            float LeftEyeOpen = GetEyeOpenRatio(true);

            if (RightEyeOpen > 0.9f)
                RightEyeOpen = 1f;
            else if (RightEyeOpen < 0.5f)
                RightEyeOpen = 0f;

            if (LeftEyeOpen > 0.9f)
                LeftEyeOpen = 1f;
            else if (LeftEyeOpen < 0.5f)
                LeftEyeOpen = 0f;

            EyeRightParam = Mathf.Lerp(EyeRightParam, 1 - RightEyeOpen, eyeSmoothing);
            EyeRightParam = Mathf.Clamp(EyeRightParam, 0, 1);
            EyeLeftParam = Mathf.Lerp(EyeLeftParam, 1 - LeftEyeOpen, eyeSmoothing);
            EyeLeftParam = Mathf.Clamp(EyeLeftParam, 0, 1);

        }

        if (enableBrow)
        {
            float rightBrowOpen = GetEyeBrowUpRatio(false);
            float leftBrowOpen = GetEyeBrowUpRatio(true);

            BrowRightParam = Mathf.Lerp(BrowRightParam, rightBrowOpen, eyeSmoothing);
            BrowLeftParam = Mathf.Lerp(BrowLeftParam, leftBrowOpen, eyeSmoothing);
        }

        if (enableMouth)
        {
            float mouthY = GetMouthOpenYRatio(points);
            float mouthX = GetMouthOpenXRatio(points);

            if (mouthY < 0.5f)
                mouthY = 0f;

            if (mouthX < 0.4f)
                mouthX = 0f;

            if (mouthX > 0.7f)
                mouthX = 0.7f;

            MouthYParam = Mathf.Lerp(MouthYParam, mouthY, mouthSmoothing);
            MouthYParam = Mathf.Clamp(MouthYParam, 0, 1);

            MouthXParam = Mathf.Lerp(MouthXParam, mouthX, mouthSmoothing);
            MouthXParam = Mathf.Clamp(MouthXParam, 0, 1);

        }
    }

    void UseExpression(int _value, bool _isUsingSfhit)
    {
        expressionController.ImplementFaceChanges(_value, _isUsingSfhit);
        isUsingExpression = true;
    }

    void ResetExpression()
    {
        expressionController.ResetAllExpression();
        isUsingExpression = false;
    }
    public override void LateUpdateValue()
    {
        mouthController.UpdateFaceTracking(MouthXParam, MouthYParam);
        eyeController.UpdateFaceTracking(EyeLeftParam, EyeRightParam);
        browController.UpdateFaceTracking(BrowLeftParam, BrowRightParam);
    }

    public void UseFacePart(bool _useEye, bool _useMouth, bool _useBrow)
    {
        enableEye = _useEye;
        enableMouth = _useMouth;
        enableBrow = _useBrow;
    }

}

