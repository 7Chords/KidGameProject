namespace KidGame
{
    public enum ControlMap
    {
        GameMap = 0,
        UIMap = 1,
    }
    public enum ControlType
    {
        Keyborad = 0,
        Gamepad = 1,
    }

    public enum InputActionType
    {
        Move = 0,
        Interaction = 1,
        Dash = 2,
        Run = 3,
        Bag = 4,
        Use = 5,
        Pick = 6,
        MouseWheel = 7,
        GamePause = 8,
    }
    public enum UseItemType
    {
        trap,
        weapon,
        food,
        Material,
        nothing
    }
}