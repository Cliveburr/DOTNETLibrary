using CSharpWeb.Kernel.Components;
using CSharpWeb.Kernel.Nodes.Builder;

namespace CSharpWeb.Main;

public class ComponentAttribute : Attribute
{
    public string Template { get; set; }

    public ComponentAttribute(string template)
    {
        
    }
}


[Component(@"
<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <title>TTI Translate</title>
  <base href=""/"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <link rel=""icon"" type=""image/x-icon"" href=""favicon.ico"">
  <link rel=""preconnect"" href=""https://fonts.gstatic.com"">
  <link href=""https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap"" rel=""stylesheet"">
  <link href=""https://fonts.googleapis.com/icon?family=Material+Icons"" rel=""stylesheet"">
</head>
<body class=""mat-typography mat-app-background"">
</body>
    <App />
</html>
")]
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
