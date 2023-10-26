
using RockEngine.DI;

using RockEngine;
using RockEngine.Utils;

AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Logger.AddError("Unhandled exception" + sender + "\n" + e);
}

IoC.Setup();
string name = "Engine";
if (args.Length > 0)
{
    name = args[0];
}
var window = new Game(name);

window.Start();
window.Dispose();
