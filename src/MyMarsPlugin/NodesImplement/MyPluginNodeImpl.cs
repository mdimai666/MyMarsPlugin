using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyMarsPlugin.Front.Nodes;
using MyMarsPlugin.Services;

namespace MyMarsPlugin.FrontImplement;

public class MyPluginNodeImpl : INodeImplement<MyPluginNode>, INodeImplement
{
    private readonly ILogger<MyPluginNodeImpl> _logger;

    public MyPluginNode Node { get; }
    public IRED RED { get; set; }
    Node INodeImplement<Node>.Node => Node;

    public MyPluginNodeImpl(MyPluginNode node, IRED red)
    {
        Node = node;
        RED = red;

        Node.Config = RED.GetConfig(node.Config);
        _logger = RED.ServiceProvider.GetRequiredService<ILogger<MyPluginNodeImpl>>();
    }

    public Task Execute(NodeMsg input, ExecuteAction callback)
    {
        _logger.LogTrace("Execute");

        var myService = RED.ServiceProvider.GetRequiredService<MyPluginService>();
        var message = myService.GetValue(input.Payload.ToString()!);

        input.Payload = message;
        callback(input);

        RED.Status(new NodeStatus(Random.Shared.Next(1, 10).ToString()));
        RED.DebugMsg(DebugMessage.NodeMessage(Node.Id, message));

        return Task.CompletedTask;
    }
}
