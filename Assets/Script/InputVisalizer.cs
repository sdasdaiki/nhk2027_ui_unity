using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] RectTransform rStickCircle;
    [SerializeField] RectTransform rStickDot;

    [Header("色と入力判定")]
    [SerializeField] Color pressedColor = Color.green;
    [SerializeField, Range(0f, 1f)] float triggerPressedThreshold = 0.1f;

    readonly Dictionary<Image, Color> originalColors = new Dictionary<Image, Color>();
    bool missingProviderLogged;

    void Update()
    {
        if (controllerInputProvider == null)
        {
            controllerInputProvider = ControllerInputProvider.Instance;
        }

        if (controllerInputProvider == null)
        {
            if (!missingProviderLogged)
            {
                Debug.LogError("InputVisalizer: ControllerInputProvider is not assigned or available.", this);
                missingProviderLogged = true;
            }
            return;
        }

        SetPressed(aButtonImage, controllerInputProvider.A_Pressed);
        SetPressed(bButtonImage, controllerInputProvider.B_Pressed);
        SetPressed(xButtonImage, controllerInputProvider.X_Pressed);
        SetPressed(yButtonImage, controllerInputProvider.Y_Pressed);
        SetPressed(lBumperImage, controllerInputProvider.LBumper_Pressed);
        SetPressed(rBumperImage, controllerInputProvider.RBumper_Pressed);
        SetPressed(viewButtonImage, controllerInputProvider.View_Pressed);
        SetPressed(menuButtonImage, controllerInputProvider.Menu_Pressed);
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
        UpdateStickDot(rStickCircle, rStickDot, controllerInputProvider.RStickInputUnprocessed);
    }

    void UpdateStickDot(RectTransform circle, RectTransform dot, Vector2 rawInput)
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

        Vector2 clampedInput = Vector2.ClampMagnitude(rawInput, 1f);
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
