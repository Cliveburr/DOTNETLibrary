using CSharpWeb.Kernel.Components;
using CSharpWeb.Kernel.Nodes.Builder;

namespace CSharpWeb.Main;

public class App : ComponentBase
{
    public override Task Build(RenderBuilder builder)
    {
        builder
            .Title("testando");
            //.H1("primeiro teste")
            //.Button("Hook test", HookTest);
        //    .Button("Increase", Increase)
        //    .H1(id: "valueH1", $"Value: {_value}")
        //    .Div(style: "content", b =>
        //     {
        //         b.H2("dentro do content");
        //     });

        //  .Routing()
        //    .To<Home>("/")
        //    .To<About>("/about");

        return Task.CompletedTask;
    }
}
