using System.ComponentModel.DataAnnotations;
using Mars.Core.Attributes;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes;

namespace MyMarsPlugin.Front.Nodes;

[FunctionApiDocument("./_plugin/MyMarsPlugin/nodes/docs/MyPluginNode/MyPluginNode{.lang}.md")]
[Display(GroupName = "other")]
public class MyPluginNode : Node
{
    public InputConfig<MyPluginConfigNode> Config { get; set; }

    public string InputString1 { get; set; } = "";

    public MyPluginNode()
    {
        HaveInput = true;
        Color = "#3fc9af";
        Outputs = new List<NodeOutput> { new NodeOutput() };
        Icon = "/_plugin/MyMarsPlugin/nodes/img/icon.png";
    }
}
