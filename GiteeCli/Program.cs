using ConsoleAppFramework;
using GiteeCli;

var app = ConsoleApp.Create();
app.Add<Commands>();
await app.RunAsync(args);
