using UnityEditor;
using UnityEngine;

/// <summary>
/// 外部链接跳转
/// </summary>
public class ExternalLinkJumpEditor
{
    [MenuItem("其他自定义拓展按钮/策划操作说明文档")]
    public static void OpenExampleOperationLink()
    {
        Application.OpenURL("https://dql9nbunh21.feishu.cn/wiki/TpLYwVyDNiRZbjkPQKmcjuBunbF?from=from_copylink");
    }
}
