
using RockEngine.DI;

using RockEngine;

IoC.Setup();
string name = "Engine";
if (args.Length > 0)
{
    name = args[0];
}
var window = new Game(name);

window.Start();
window.Dispose();
