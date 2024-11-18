using UnityEngine;
using RootMotion.FinalIK;
using Leap.Unity;
using Leap.Unity.HandsModule;
using UnityEngine.Events;

public class HalfBodyTrackingCharacterSetUp : MonoBehaviour
{ 
    public CharacterCreationManager character;
    public SkinnedMeshRenderer faceBlendShape;
    public UnityAction<int, bool> onFaceChange;
    public UnityAction onFaceReset;

    [SerializeField] FullBodyBipedIK bipedIK;

    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    public void SetUpCharacter()
    {
        if (character == null)
        {
            DebugExt.LogError("SO Character not assigned!");
            return;
        }

        if (bipedIK == null)
            bipedIK = GetComponent<FullBodyBipedIK>();

        SetUpFaceBlendShape();

        SetUpCharacterHand();

        SetUpHeadEffector();

        SetUpPrefabs();

    }

    //Assign to button or keyboard
    public void OnFaceChange(int _index, bool _isUsingShift)
    {
        onFaceChange?.Invoke(_index, _isUsingShift);
    }

    //Assign to button or keyboard
    public void OnFaceReset()
    {
        onFaceReset?.Invoke();
    }

    void SetUpPrefabs()
    {
        if(character.handManagerPrefab == null || character.leapServicesPrefab == null)
        {
            DebugExt.LogWarning("You need to assign missing prefabs");
            return;
        }    

        if(gameObject.GetComponentInChildren<HandModelManager>() == null)
            Instantiate(character.handManagerPrefab, this.transform);
        
        if(gameObject.transform.Find("Leap Service Providers Game Object(Clone)") == null)
            Instantiate(character.leapServicesPrefab, this.transform);
    }

    void SetUpCharacterHand()
    {
        leftHand = bipedIK.references.leftHand;
        rightHand = bipedIK.references.rightHand;

        //Set Up left Hand
        if (leftHand.gameObject.GetComponent<HandBinder>() == null)
            leftHand.gameObject.AddComponent<HandBinder>();

        if (leftHand.gameObject.GetComponent<HandUpdateManager>() == null)
        {
            var leftHandUpdate = leftHand.gameObject.AddComponent<HandUpdateManager>();
            leftHandUpdate.AssignVariables();
            leftHandUpdate.SetHandType(true);
        }

        //Set Up Right Hand
        if (rightHand.gameObject.GetComponent<HandBinder>() == null)
            rightHand.gameObject.AddComponent<HandBinder>();

        if (rightHand.gameObject.GetComponent<HandUpdateManager>() == null)
        {
            var rightHandUpdate = rightHand.gameObject.AddComponent<HandUpdateManager>();
            rightHandUpdate.AssignVariables();
            rightHandUpdate.SetHandType(false);
        }
    }

    void SetUpFaceBlendShape()
    {

        if (faceBlendShape != null)
        {
            DebugExt.Log("Face Blend Shape is Already Assigned --> " + faceBlendShape.ToString());
            return;
        }

        foreach (SkinnedMeshRenderer r in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var mesh = r.sharedMesh;

            if (mesh.blendShapeCount < 6)
                continue;

            faceBlendShape = r;
            DebugExt.Log("Face Blend Shape Assigned! --> " + r.ToString());
            return;
        }
    }

    void SetUpHeadEffector()
    {
        if (!gameObject.transform.Find("Head Effector"))
        {
            GameObject go = new GameObject("Head Effector");
            go.transform.parent = gameObject.transform;
            var he = go.AddComponent<FBBIKHeadEffector>();
            he.ik = FindObjectOfType<FullBodyBipedIK>();
        }
    }
}
