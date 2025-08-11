using System;

namespace Inflowis {
    [Flags]
    // ReSharper disable InconsistentNaming
    public enum DeviceFull {
        Keyboard = 1 << 0,
        Mouse = 1 << 1,
        Gamepad = 1 << 2,
        
        /* Maybe not needed
        XInputControllerWindows = 1 << 3,
        DualShock4GamepadHID = 1 << 4,
        DualSenseGamepadHID = 1 << 5,
        NintendoSwitchProController = 1 << 6,
        */
    }
    // ReSharper enable InconsistentNaming

    public enum DeviceName {
        Mouse,
        Keyboard,
        JayCon,
        DualSense,
        DualShock,
        XboxController
    }

    public enum InputBind {
        Move,
        Dash,
        Jump,
        Attack,
        StrongAttack,
        Skill1,
        Skill2,
        Skill3,
        Skill4,
        SkillActivate,
        Escape
    }

    public static class InputHelper {
        public static DeviceName? ConvertToDeviceType(string officialDeviceName) {
            return officialDeviceName switch {
                "Keyboard" => DeviceName.Keyboard,
                "Mouse" => DeviceName.Mouse,
                "XInputControllerWindows" => DeviceName.XboxController,
                "DualShock4GamepadHID" => DeviceName.DualShock,
                "DualSenseGamepadHID" => DeviceName.DualSense,
                "Nintendo Switch Pro Controller" => DeviceName.JayCon,
                _ => null
            };
        }
    }
}