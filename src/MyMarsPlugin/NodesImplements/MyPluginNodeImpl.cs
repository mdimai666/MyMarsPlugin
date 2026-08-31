using Mars.Nodes.Abstractions;
using Mars.Nodes.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyMarsPlugin.Front.Nodes;
using MyMarsPlugin.Services;

namespace MyMarsPlugin.NodesImplements;

// Lifetime scope is Singletone
public class MyPluginNodeImpl : INodeImplement<MyPluginNode>
{
    private readonly ILogger<MyPluginNodeImpl> _logger;

    public MyPluginNode Node { get; }
    public IRuntimeNodeScope RNS { get; set; }
    Node INodeImplement.Node => Node;

    public MyPluginNodeImpl(MyPluginNode node, IRuntimeNodeScope rns, ILogger<MyPluginNodeImpl> logger) // Singletone Services only
    {
        Node = node;
        RNS = rns;
        _logger = logger;

        Node.Config = RNS.GetConfig(node.Config);
    }

    public Task Execute(NodeMsg input, ExecuteAction callback, ExecutionParameters parameters)
    {
        _logger.LogTrace("Execute");

        var myService = RNS.ServiceProvider.GetRequiredService<MyPluginService>();
        var message = myService.GetValue(input.Payload.ToString()!);

        input.Payload = message;
        callback(input);

        RNS.Status(new NodeStatus(Random.Shared.Next(1, 10).ToString()));
        RNS.DebugMsg(DebugMessage.NodeMessage(Node.Id, message));

        return Task.CompletedTask;
    }
}
