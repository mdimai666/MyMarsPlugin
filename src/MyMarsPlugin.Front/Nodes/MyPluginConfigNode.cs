using System.ComponentModel.DataAnnotations;
using Mars.Core.Attributes;
using Mars.Nodes.Core.Nodes;

namespace MyMarsPlugin.Front.Nodes;

[FunctionApiDocument("./_plugin/MyMarsPlugin/nodes/docs/MyNodeConfigNode/MyNodeConfigNode{.lang}.md")]
[Display(GroupName = "other")]
public class MyPluginConfigNode : ConfigNode
{
    [Required]
    public string Value1 { get; set; } = "";

}
