using UnityEngine;

public class ButtonPanelSwitcher : MonoBehaviour
{
    public enum SwitchButton
    {
        A,
        B,
        X,
        Y,
        LBumper,
        RBumper,
        View,
        Menu,
        LStick,
        RStick,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight
    }

    [Header("Input Settings")]
    [SerializeField] private SwitchButton triggerButton = SwitchButton.X;

    [Header("Target Panels")]
    [SerializeField] private GameObject[] panels;

    private int currentIndex = 0;
    private SwitchButton subscribedButton;
    private bool isSubscribed = false;

    private void OnEnable()
    {
        Subscribe(triggerButton);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

#if UNITY_EDITOR
    // Playモード中にInspectorでボタンを変更したときも即座に反映する
    private void OnValidate()
    {
        if (Application.isPlaying && isActiveAndEnabled && subscribedButton != triggerButton)
        {
            Unsubscribe();
            Subscribe(triggerButton);
        }
    }
#endif

    private void Start()
    {
        UpdatePanels();
    }

    public void NextPanel()
    {
        if (panels == null || panels.Length == 0) return;

        currentIndex = (currentIndex + 1) % panels.Length;
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
            {
                panels[i].SetActive(i == currentIndex);
            }
        }
    }

    private void Subscribe(SwitchButton button)
    {
        if (isSubscribed) return;

        switch (button)
        {
            case SwitchButton.A:         ControllerInputProvider.A_DownEvent += NextPanel; break;
            case SwitchButton.B:         ControllerInputProvider.B_DownEvent += NextPanel; break;
            case SwitchButton.X:         ControllerInputProvider.X_DownEvent += NextPanel; break;
            case SwitchButton.Y:         ControllerInputProvider.Y_DownEvent += NextPanel; break;
            case SwitchButton.LBumper:   ControllerInputProvider.LBumper_DownEvent += NextPanel; break;
            case SwitchButton.RBumper:   ControllerInputProvider.RBumper_DownEvent += NextPanel; break;
            case SwitchButton.View:      ControllerInputProvider.View_DownEvent += NextPanel; break;
            case SwitchButton.Menu:      ControllerInputProvider.Menu_DownEvent += NextPanel; break;
            case SwitchButton.LStick:    ControllerInputProvider.LStick_DownEvent += NextPanel; break;
            case SwitchButton.RStick:    ControllerInputProvider.RStick_DownEvent += NextPanel; break;
            case SwitchButton.DpadUp:    ControllerInputProvider.DpadU_DownEvent += NextPanel; break;
            case SwitchButton.DpadDown:  ControllerInputProvider.DpadD_DownEvent += NextPanel; break;
            case SwitchButton.DpadLeft:  ControllerInputProvider.DpadL_DownEvent += NextPanel; break;
            case SwitchButton.DpadRight: ControllerInputProvider.DpadR_DownEvent += NextPanel; break;
        }

        subscribedButton = button;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed) return;

        switch (subscribedButton)
        {
            case SwitchButton.A:         ControllerInputProvider.A_DownEvent -= NextPanel; break;
            case SwitchButton.B:         ControllerInputProvider.B_DownEvent -= NextPanel; break;
            case SwitchButton.X:         ControllerInputProvider.X_DownEvent -= NextPanel; break;
            case SwitchButton.Y:         ControllerInputProvider.Y_DownEvent -= NextPanel; break;
            case SwitchButton.LBumper:   ControllerInputProvider.LBumper_DownEvent -= NextPanel; break;
            case SwitchButton.RBumper:   ControllerInputProvider.RBumper_DownEvent -= NextPanel; break;
            case SwitchButton.View:      ControllerInputProvider.View_DownEvent -= NextPanel; break;
            case SwitchButton.Menu:      ControllerInputProvider.Menu_DownEvent -= NextPanel; break;
            case SwitchButton.LStick:    ControllerInputProvider.LStick_DownEvent -= NextPanel; break;
            case SwitchButton.RStick:    ControllerInputProvider.RStick_DownEvent -= NextPanel; break;
            case SwitchButton.DpadUp:    ControllerInputProvider.DpadU_DownEvent -= NextPanel; break;
            case SwitchButton.DpadDown:  ControllerInputProvider.DpadD_DownEvent -= NextPanel; break;
            case SwitchButton.DpadLeft:  ControllerInputProvider.DpadL_DownEvent -= NextPanel; break;
            case SwitchButton.DpadRight: ControllerInputProvider.DpadR_DownEvent -= NextPanel; break;
        }

        isSubscribed = false;
    }
}