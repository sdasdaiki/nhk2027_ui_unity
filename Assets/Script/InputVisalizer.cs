using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputVisalizer : MonoBehaviour
{
    [Header("入力元")]
    [SerializeField] ControllerInputProvider controllerInputProvider;

    [Header("ボタン画像")]
    [SerializeField] Image aButtonImage;
    [SerializeField] Image bButtonImage;
    [SerializeField] Image xButtonImage;
    [SerializeField] Image yButtonImage;
    [SerializeField] Image lBumperImage;
    [SerializeField] Image rBumperImage;
    [SerializeField] Image viewButtonImage;
    [SerializeField] Image menuButtonImage;
    [SerializeField] Image lStickButtonImage;
    [SerializeField] Image rStickButtonImage;
    [SerializeField] Image lTriggerImage;
    [SerializeField] Image rTriggerImage;

    [Header("十字キー画像")]
    [SerializeField] Image dpadUpImage;
    [SerializeField] Image dpadDownImage;
    [SerializeField] Image dpadLeftImage;
    [SerializeField] Image dpadRightImage;

    [Header("スティック表示領域と点")]
    [SerializeField] RectTransform lStickCircle;
    [SerializeField] RectTransform lStickDot;
    [SerializeField] RectTransform lStickProcessedDot;
    [SerializeField] RectTransform rStickCircle;
    [SerializeField] RectTransform rStickDot;
    [SerializeField] RectTransform rStickProcessedDot;

    [Header("スティック値表示")]
    [SerializeField] TextMeshProUGUI lStickXYText;
    [SerializeField] TextMeshProUGUI rStickXYText;

    [Header("色と入力判定")]
    [SerializeField] Color pressedColor = Color.green;
    [SerializeField, Range(0f, 1f)] float triggerPressedThreshold = 0.1f;

    readonly Dictionary<Image, Color> originalColors = new Dictionary<Image, Color>();

    void Start()
    {
        if (controllerInputProvider == null)
        {
            controllerInputProvider = ControllerInputProvider.Instance;
        }

        if (controllerInputProvider == null)
        {
            Debug.LogError("InputVisalizer: ControllerInputProvider is not assigned or available.", this);
        }
    }

    void Update()
    {
        if (controllerInputProvider == null)
        {
            return;
        }

        SetPressed(aButtonImage, controllerInputProvider.A_Pressed);
        SetPressed(bButtonImage, controllerInputProvider.B_Pressed);
        SetPressed(xButtonImage, controllerInputProvider.X_Pressed);
        SetPressed(yButtonImage, controllerInputProvider.Y_Pressed);
        SetPressed(lBumperImage, controllerInputProvider.LBumper_Pressed);
        SetPressed(rBumperImage, controllerInputProvider.RBumper_Pressed);
        SetPressed(viewButtonImage, controllerInputProvider.G_Pressed);
        SetPressed(menuButtonImage, controllerInputProvider.S_Pressed);
        SetPressed(lStickButtonImage, controllerInputProvider.LStickPressed);
        SetPressed(rStickButtonImage, controllerInputProvider.RStickPressed);
        SetPressed(lTriggerImage, controllerInputProvider.LTriggerInput > triggerPressedThreshold);
        SetPressed(rTriggerImage, controllerInputProvider.RTriggerInput > triggerPressedThreshold);

        Vector2 dpadInput = controllerInputProvider.DpadVecInput;
        SetPressed(dpadUpImage, dpadInput.y > 0.5f);
        SetPressed(dpadDownImage, dpadInput.y < -0.5f);
        SetPressed(dpadLeftImage, dpadInput.x < -0.5f);
        SetPressed(dpadRightImage, dpadInput.x > 0.5f);

        UpdateStickDot(lStickCircle, lStickDot, controllerInputProvider.LStickInputUnprocessed);
        UpdateStickDot(lStickCircle, lStickProcessedDot, controllerInputProvider.LStickInput);
        UpdateStickDot(rStickCircle, rStickDot, controllerInputProvider.RStickInputUnprocessed);
        UpdateStickDot(rStickCircle, rStickProcessedDot, controllerInputProvider.RStickInput);

        UpdateStickText(lStickXYText, controllerInputProvider.LStickInput);
        UpdateStickText(rStickXYText, controllerInputProvider.RStickInput);
    }

    void UpdateStickText(TextMeshProUGUI text, Vector2 stickInput)
    {
        if (text != null)
        {
            text.text = $"X: {stickInput.x:F2}, Y: {stickInput.y:F2}, " +
                        $"Magnitude: {stickInput.magnitude:F2}";
        }
    }

    void UpdateStickDot(RectTransform circle, RectTransform dot, Vector2 stickInput)
    {
        if (circle == null || dot == null)
        {
            return;
        }

        Rect circleRect = circle.rect;
        float scaleX = Mathf.Abs(dot.lossyScale.x / circle.lossyScale.x);
        float scaleY = Mathf.Abs(dot.lossyScale.y / circle.lossyScale.y);
        float dotHalfWidth = dot.rect.width * scaleX * 0.5f;
        float dotHalfHeight = dot.rect.height * scaleY * 0.5f;
        float dotRadius = Mathf.Sqrt(dotHalfWidth * dotHalfWidth + dotHalfHeight * dotHalfHeight);
        float radius = Mathf.Max(0f, Mathf.Min(circleRect.width, circleRect.height) * 0.5f - dotRadius);

        Vector2 clampedInput = Vector2.ClampMagnitude(stickInput, 1f);
        Vector2 localPosition = circleRect.center + clampedInput * radius;
        dot.position = circle.TransformPoint(localPosition);
    }

    void SetPressed(Image image, bool isPressed)
    {
        if (image == null)
        {
            return;
        }

        Color originalColor;
        if (!originalColors.TryGetValue(image, out originalColor))
        {
            originalColor = image.color;
            originalColors.Add(image, originalColor);
        }

        image.color = isPressed ? pressedColor : originalColor;
    }
}
