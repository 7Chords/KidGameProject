//该脚本放一些全局通用的不局限于某个功能的枚举
namespace KidGame
{
    public enum ControlType
    {
        Keyborad = 0,
        Gamepad = 1,//具体区分类型...
    }

    public enum InputActionType
    {
        Move = 0,
        Interaction = 1,
        Dash = 2,
        Run = 3,
        Bag = 4,
        Use = 5,
        Throw = 6,
        Recycle = 7,
        Switch = 8,
    }


}
