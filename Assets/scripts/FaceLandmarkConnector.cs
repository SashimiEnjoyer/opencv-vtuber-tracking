
using System.Collections.Generic;
using UnityEngine;
using CVVTuber;

public class FaceLandmarkConnector : CVVTuberProcess
{
    public static FaceLandmarkConnector instance;

    [SerializeField, InterfaceRestriction(typeof(IFaceLandmarkGetter))]
    protected CVVTuberProcess faceLandmarkGetter;

    protected IFaceLandmarkGetter _faceLandmarkGetterInterface = null;
    public IFaceLandmarkGetter faceLandmarkGetterInterface
    {
        get
        {
            if (faceLandmarkGetter != null && _faceLandmarkGetterInterface == null)
                _faceLandmarkGetterInterface = faceLandmarkGetter.GetComponent<IFaceLandmarkGetter>();
            return _faceLandmarkGetterInterface;
        }
    }

    [SerializeField] FaceLandmarkHeadPositionAndRotationGetter faceLandmark;
    
    // Public variables
    [HideInInspector] public List<Vector2> landmarkPoints = new List<Vector2>();
    [HideInInspector] public bool isFaceTracking = true;
    [HideInInspector] public bool isInFaceTrackingState { get { return faceLandmark.didUpdateHeadPositionAndRotation && isFaceTracking; } }

    public override string GetDescription()
    {
        return "Connector Face Landmark To Blend Shape";
    }

    public override void Setup()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public override void UpdateValue()
    {
        landmarkPoints = faceLandmarkGetterInterface.GetFaceLandmarkPoints();

    }
}
