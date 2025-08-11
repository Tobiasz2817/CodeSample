using System;

namespace Inflowis {
    [Flags]
    public enum InputDevice {
        Keyboard = 1 << 0,
        XboxController = 1 << 1,
        PlayStationController = 1 << 2
    }
        
    public enum MapType {
        Gameplay,
        Interface,
        UI
    }
}