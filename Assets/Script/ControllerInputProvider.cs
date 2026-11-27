using UnityEngine;
using System;
using System.Collections.Generic;

public class ControllerInputProvider : MonoBehaviour
{
    [Serializable]
    private class ButtonMap
    {
        public int a = 0;
        public int b = 1;
        public int x = 2;
        public int y = 3;
        public int leftBumper = 4;
        public int rightBumper = 5;
        public int view = 6;
        public int menu = 7;
        public int leftStick = 8;
        public int rightStick = 9;
    }

    public static ControllerInputProvider instance;
    public static ControllerInputProvider Instance { get { return instance; } }
    public static event Action<Vector2> LStickEvent;
    public static event Action<Vector2> RStickEvent;
    public static event Action RStickU_DownEvent;
    public static event Action RStickD_DownEvent;
    public static event Action RStickL_DownEvent;
    public static event Action RStickR_DownEvent;
    public static event Action DpadU_DownEvent;
    public static event Action DpadL_DownEvent;
    public static event Action DpadR_DownEvent;
    public static event Action DpadD_DownEvent;

    public static event Action A_DownEvent;
    public static event Action B_DownEvent;
    public static event Action X_DownEvent;
    public static event Action Y_DownEvent;
    public static event Action LBumper_DownEvent;
    public static event Action RBumper_DownEvent;
    public static event Action View_DownEvent;
    public static event Action Menu_DownEvent;
    public static event Action LStick_DownEvent;
    public static event Action RStick_DownEvent;

    float m_LStickDeadzone = 0f;
    float m_LStickDeadzoneSquared = 0f;
    float m_RStickDeadzone = 0f;
    float m_RStickDeadzoneSquared = 0f;
    Vector2 m_LStickInput = Vector2.zero;
    public Vector2 LStickInput { get { return m_LStickInput; } }
    Vector2 m_RStickInput = Vector2.zero;
    public Vector2 RStickInput { get { return m_RStickInput; } }
    float m_LTriggerInput = 0f;
    public float LTriggerInput { get { return m_LTriggerInput; } }
    float m_RTriggerInput = 0f;
    public float RTriggerInput { get { return m_RTriggerInput; } }


    Vector2 lastLStickInputUnprocessed = Vector2.zero;
    Vector2 m_LStickInputUnprocessed = Vector2.zero;
    public Vector2 LStickInputUnprocessed { get { return m_LStickInputUnprocessed; } }
    Vector2 lastRStickInputUnprocessed = Vector2.zero;
    Vector2 m_RStickInputUnprocessed = Vector2.zero;
    public Vector2 RStickInputUnprocessed { get { return m_RStickInputUnprocessed; } }
    float m_LTriggerInputUnprocessed = 0f;
    public float LTriggerInputUnprocessed { get { return m_LTriggerInputUnprocessed; } }
    float m_RTriggerInputUnprocessed = 0f;
    public float RTriggerInputUnprocessed { get { return m_RTriggerInputUnprocessed; } }

    bool m_LStickPressed = false;
    public bool LStickPressed { get { return m_LStickPressed; } }
    bool m_RStickPressed = false;
    public bool RStickPressed { get { return m_RStickPressed; } }

    Vector2 m_DpadInput = Vector2.zero;
    public Vector2 DpadVecInput { get { return m_DpadInput; } }

    bool m_A_Pressed = false;
    public bool A_Pressed { get { return m_A_Pressed; } }
    bool m_B_Pressed = false;
    public bool B_Pressed { get { return m_B_Pressed; } }
    bool m_X_Pressed = false;
    public bool X_Pressed { get { return m_X_Pressed; } }
    bool m_Y_Pressed = false;
    public bool Y_Pressed { get { return m_Y_Pressed; } }
    bool m_LBumper_Pressed = false;
    public bool LBumper_Pressed { get { return m_LBumper_Pressed; } }
    bool m_RBumper_Pressed = false;
    public bool RBumper_Pressed { get { return m_RBumper_Pressed; } }
    bool m_G_Pressed = false;
    public bool G_Pressed { get { return m_G_Pressed; } }
    bool m_S_Pressed = false;
    public bool S_Pressed { get { return m_S_Pressed; } }




    public static event Action DebugEventF;
    public static event Action DebugEventJ;

    [SerializeField] TMPro.TextMeshProUGUI debugText2;
    [SerializeField] TMPro.TextMeshProUGUI debugText3;

    [Header("Android joystick button numbers (Unity JoystickButtonN)")]
    [SerializeField] ButtonMap androidButtonMap = new ButtonMap
    {
        view = 10,
        menu = 11
    };

    float inputLogTimer;

    static KeyCode JoystickButton(int buttonNumber)
    {
        return (KeyCode)((int)KeyCode.JoystickButton0 + buttonNumber);
    }

    void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        StickInput();
        DpadInput();
        ButtonInput();
#if UNITY_EDITOR
        DebugInput();
#endif
// 値出力
        inputLogTimer += Time.deltaTime;
        if (inputLogTimer >= 1f)
        {
            inputLogTimer = 0f;
            Debug.Log($"LStick: {m_LStickInput}, RStick: {m_RStickInput}, " +
                      $"DPad: {m_DpadInput}, LTrigger: {m_LTriggerInput:F2}, " +
                      $"RTrigger: {m_RTriggerInput:F2}");
        }
    }
    void StickInput()
    {
        m_LStickInputUnprocessed = new Vector2(
            Input.GetAxis("AndroidLStickX"),
            Input.GetAxis("AndroidLStickY"));
        m_RStickInputUnprocessed = new Vector2(
            Input.GetAxis("AndroidRStickX"),
            Input.GetAxis("AndroidRStickY"));
        m_LTriggerInputUnprocessed = Input.GetAxis("AndroidLTrigger");
        m_RTriggerInputUnprocessed = Input.GetAxis("AndroidRTrigger");
        m_LTriggerInput = m_LTriggerInputUnprocessed;
        m_RTriggerInput = m_RTriggerInputUnprocessed;
        m_LStickInput = ProcessStick(m_LStickInputUnprocessed, 
                                    m_LStickDeadzone, m_LStickDeadzoneSquared, LstickDeadZoneConst);
        m_RStickInput = ProcessStick(m_RStickInputUnprocessed, 
                                    m_RStickDeadzone, m_RStickDeadzoneSquared, RstickDeadZoneConst);

        LStickEvent?.Invoke(m_LStickInput);
        RStickEvent?.Invoke(m_RStickInput);


        // Rスティックの上下左右の入力を検出してイベント発火
        // if (lastRStickInputUnprocessed.x <= 0.8f && m_RStickInputUnprocessed.x > 0.8f)
        // {
        //     RStickR_DownEvent?.Invoke(); // Repeater登録済み
        // }
        // if (lastRStickInputUnprocessed.x >= -0.8f && m_RStickInputUnprocessed.x < -0.8f)
        // {
        //     RStickL_DownEvent?.Invoke(); // Repeater登録済み
        // }


        lastLStickInputUnprocessed = m_LStickInputUnprocessed;
        lastRStickInputUnprocessed = m_RStickInputUnprocessed;
    }

    Vector2 ProcessStick(Vector2 rawInput, float deadzone, float sqrDeadzone, float deadzoneConst)
    {
        float sqrMag = rawInput.sqrMagnitude;
        if (sqrMag < sqrDeadzone) return Vector2.zero;

        // 0除算防止のため sqrMag が極小の場合は zero を返す
        if (sqrMag < 1e-6f) return Vector2.zero;

        float multiplier = (1f - (deadzone / Mathf.Sqrt(sqrMag))) * deadzoneConst;
        return rawInput * multiplier;
    }
    float previousDpadX = 0f;
    float previousDpadY = 0f;
    void DpadInput()
    {
        float currentDpadX = Input.GetAxisRaw("AndroidDPadX");
        float currentDpadY = Input.GetAxisRaw("AndroidDPadY");

        m_DpadInput.x = currentDpadX;
        m_DpadInput.y = currentDpadY;

        if (previousDpadX != currentDpadX)
        {
            if (currentDpadX > 0.5f)
            {
                DpadR_DownEvent?.Invoke();
            }
            else if (currentDpadX < -0.5f)
            {
                DpadL_DownEvent?.Invoke();
            }
        }

        if (previousDpadY != currentDpadY)
        {
            if (currentDpadY > 0.5f)
            {
                DpadU_DownEvent?.Invoke();
            }
            else if (currentDpadY < -0.5f)
            {
                DpadD_DownEvent?.Invoke();
            }
        }

        previousDpadX = currentDpadX;
        previousDpadY = currentDpadY;
    }

    void ButtonInput()
    {
        ButtonMap map = androidButtonMap;
        ReadButton(map.a, A_DownEvent, ref m_B_Pressed);
        ReadButton(map.b, B_DownEvent, ref m_A_Pressed);
        ReadButton(map.x, X_DownEvent, ref m_Y_Pressed);
        ReadButton(map.y, Y_DownEvent, ref m_X_Pressed);
        ReadButton(map.leftBumper, LBumper_DownEvent, ref m_LBumper_Pressed);
        ReadButton(map.rightBumper, RBumper_DownEvent, ref m_RBumper_Pressed);
        ReadButton(map.view, View_DownEvent, ref m_S_Pressed);
        ReadButton(map.menu, Menu_DownEvent, ref m_G_Pressed);
        ReadButton(map.leftStick, LStick_DownEvent, ref m_LStickPressed);
        ReadButton(map.rightStick, RStick_DownEvent, ref m_RStickPressed);
    }

    static void ReadButton(int buttonNumber, Action downEvent, ref bool pressed)
    {
        KeyCode key = JoystickButton(buttonNumber);
        if (Input.GetKeyDown(key))
        {
            downEvent?.Invoke();
            pressed = true;
        }
        else if (!Input.GetKey(key))
        {
            pressed = false;
        }
    }

    float LstickDeadZoneConst = 1f;
    public void SetLStickDeadzone(float deadzone)
    {
        m_LStickDeadzone = deadzone;
        m_LStickDeadzoneSquared = deadzone * deadzone;
        LstickDeadZoneConst = 1f / (1f - deadzone);
    }
    float RstickDeadZoneConst = 1f;
    public void SetRStickDeadzone(float deadzone)
    {
        m_RStickDeadzone = deadzone;
        m_RStickDeadzoneSquared = deadzone * deadzone;
        RstickDeadZoneConst = 1f / (1f - deadzone);
    }
#if UNITY_EDITOR
    void DebugInput()
    {
        ButtonMap map = androidButtonMap;
        if (!Input.GetKey(JoystickButton(map.leftStick)))
        {
            m_LStickPressed = false;
        }
        if (!Input.GetKey(JoystickButton(map.rightStick)))
        {
            m_RStickPressed = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            DebugEventF?.Invoke();
        }
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     DebugEventJ?.Invoke();
        //     RStickL_DownEvent?.Invoke();
        // }
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     RStickR_DownEvent?.Invoke();
        // }
        if (Input.GetKeyDown(KeyCode.I))
        {
            RStickU_DownEvent?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            RStickD_DownEvent?.Invoke();
        }



        if (Input.GetKey(KeyCode.F))
        {
            m_LStickInputUnprocessed.x = 1f;
            m_LStickInput.x = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_LStickInputUnprocessed.x = -1f;
            m_LStickInput.x = -1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            m_LStickInputUnprocessed.y = 1f;
            m_LStickInput.y = 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_LStickInputUnprocessed.y = -1f;
            m_LStickInput.y = -1f;
        }

        if (Input.GetKey(KeyCode.J))
        {
            m_RStickInputUnprocessed.x = -1f;
            m_RStickInput.x = -1f;
        }
        if (Input.GetKey(KeyCode.L))
        {
            m_RStickInputUnprocessed.x = 1f;
            m_RStickInput.x = 1f;
        }
        if (Input.GetKey(KeyCode.I))
        {
            m_RStickInputUnprocessed.y = 1f;
            m_RStickInput.y = 1f;
        }
        if (Input.GetKey(KeyCode.K))
        {
            m_RStickInputUnprocessed.y = -1f;
            m_RStickInput.y = -1f;
        }

        if (Input.GetKey(KeyCode.R))
        {
            m_LTriggerInputUnprocessed = 1f;
            m_LTriggerInput = 1f;
        }
        if (Input.GetKey(KeyCode.U))
        {
            m_RTriggerInputUnprocessed = 1f;
            m_RTriggerInput = 1f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DpadL_DownEvent?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            DpadR_DownEvent?.Invoke();
        }
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     DpadU_DownEvent?.Invoke();
        // }
        // if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     DpadD_DownEvent?.Invoke();
        // }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_DpadInput.x = -1f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_DpadInput.x = 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_DpadInput.y = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_DpadInput.y = -1f;
        }


        if (Input.GetKey(KeyCode.G))
        {
            m_LStickPressed = true;
            LStick_DownEvent?.Invoke();
        }
        if (Input.GetKey(KeyCode.H))
        {
            m_RStickPressed = true;
            RStick_DownEvent?.Invoke();
        }

        if (m_LStickInput != Vector2.zero)
        {
            LStickEvent?.Invoke(m_LStickInput);
        }
        if (m_RStickInput != Vector2.zero)
        {
            RStickEvent?.Invoke(m_RStickInput);
        }

        // button debug
        if (Input.GetKeyDown(KeyCode.Q))
        {
            A_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.Q))
        {
            m_A_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            B_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.B))
        {
            m_B_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            X_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.X))
        {
            m_X_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Y_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.Y))
        {
            m_Y_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            LBumper_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.LeftShift))
        {
            m_LBumper_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            RBumper_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.RightShift))
        {
            m_RBumper_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            View_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.Tab))
        {
            m_G_Pressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Menu_DownEvent?.Invoke();
        } else if (Input.GetKey(KeyCode.Return))
        {
            m_S_Pressed = true;
        }

    }
#endif

}

