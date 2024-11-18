using UnityEngine.UI;
using UnityEngine;


public class ExpressionKeyManager : MonoBehaviour
{
    public HalfBodyTrackingCharacterSetUp halfBody;
    [SerializeField] Transform buttonExpressionsParent;
    [SerializeField] Transform buttonToggleExpressionParent;
    [SerializeField] GameObject buttonExpressionsPrefab;
    [SerializeField] GameObject buttonResetExpressionPrefab;
    Controller keyInput;
    bool shiftPressed = false;

    private void Awake()
    {
        keyInput = new Controller();
    }

    private void OnEnable()
    {
        keyInput.Enable();
        keyInput.ExpressionKey.Expression1.performed += ctx => halfBody.onFaceChange(0, shiftPressed); 
        keyInput.ExpressionKey.Expression2.performed += ctx => halfBody.onFaceChange(1, shiftPressed);
        keyInput.ExpressionKey.Expression3.performed += ctx => halfBody.onFaceChange(2, shiftPressed);
        keyInput.ExpressionKey.Expression4.performed += ctx => halfBody.onFaceChange(3, shiftPressed);
        keyInput.ExpressionKey.Expression5.performed += ctx => halfBody.onFaceChange(4, shiftPressed);
        keyInput.ExpressionKey.Expression6.performed += ctx => halfBody.onFaceChange(5, shiftPressed);
        keyInput.ExpressionKey.Expression7.performed += ctx => halfBody.onFaceChange(6, shiftPressed);
        keyInput.ExpressionKey.Expression8.performed += ctx => halfBody.onFaceChange(7, shiftPressed);
        keyInput.ExpressionKey.Expression9.performed += ctx => halfBody.onFaceChange(8, shiftPressed);
        keyInput.ExpressionKey.Expression10.performed += ctx => halfBody.onFaceChange(9, shiftPressed);
        keyInput.ExpressionKey.ResetExpression.performed += ctx => halfBody.OnFaceReset();
    }

    private void OnDisable()
    {
        keyInput.ExpressionKey.Expression1.performed -= ctx => halfBody.onFaceChange(0, shiftPressed);
        keyInput.ExpressionKey.Expression2.performed -= ctx => halfBody.onFaceChange(1, shiftPressed);
        keyInput.ExpressionKey.Expression3.performed -= ctx => halfBody.onFaceChange(2, shiftPressed);
        keyInput.ExpressionKey.Expression4.performed -= ctx => halfBody.onFaceChange(3, shiftPressed);
        keyInput.ExpressionKey.Expression5.performed -= ctx => halfBody.onFaceChange(4, shiftPressed);
        keyInput.ExpressionKey.Expression6.performed -= ctx => halfBody.onFaceChange(5, shiftPressed);
        keyInput.ExpressionKey.Expression7.performed -= ctx => halfBody.onFaceChange(6, shiftPressed);
        keyInput.ExpressionKey.Expression8.performed -= ctx => halfBody.onFaceChange(7, shiftPressed);
        keyInput.ExpressionKey.Expression9.performed -= ctx => halfBody.onFaceChange(8, shiftPressed);
        keyInput.ExpressionKey.Expression10.performed -= ctx => halfBody.onFaceChange(9, shiftPressed);
        keyInput.ExpressionKey.ResetExpression.performed -= ctx => halfBody.OnFaceReset();
        keyInput.Disable();
    }

    private void Start()
    {
        InitializeExpressionButtons();
        InitializeResetExpressionButton();
    }

    private void Update()
    {
        shiftPressed = IsUsingShift();
    }

    private void InitializeExpressionButtons()
    {
        for (int i = 0; i < halfBody.character.faceVariations.Length; i++)
        {
            int indexTemp = i;
            GameObject go = Instantiate(buttonExpressionsPrefab, buttonExpressionsParent);
            go.GetComponentInChildren<TMPro.TMP_Text>().text = "Key " + indexTemp + " : " + halfBody.character.faceVariations[indexTemp].variationName;
            go.GetComponent<Button>().onClick.AddListener(() => ImplementFaceChange(indexTemp));
        }

        for (int i = 0; i < halfBody.character.toggleableFaceVariations.Length; i++)
        {
            int indexTemp = i;
            GameObject go = Instantiate(buttonExpressionsPrefab, buttonToggleExpressionParent);
            go.GetComponentInChildren<TMPro.TMP_Text>().text = "Key " + indexTemp + " : " + halfBody.character.toggleableFaceVariations[indexTemp].toggleableVariationName;
            go.GetComponent<Button>().onClick.AddListener(() => ImplementFaceChange(indexTemp, true));
        }
       
    }

    void InitializeResetExpressionButton()
    {
        GameObject rgo = Instantiate(buttonResetExpressionPrefab, buttonExpressionsParent.parent);
        rgo.GetComponent<Button>().onClick.AddListener(halfBody.OnFaceReset);
    }

    void ImplementFaceChange(int index)
    {
        halfBody.OnFaceChange(index, keyInput.ExpressionKey.ShiftExpression.IsPressed());
    }

    void ImplementFaceChange(int index, bool usingShift)
    {
        halfBody.OnFaceChange(index, usingShift);
    }

    bool IsUsingShift()
    {
        return keyInput.ExpressionKey.ShiftExpression.IsPressed();
    }

}
