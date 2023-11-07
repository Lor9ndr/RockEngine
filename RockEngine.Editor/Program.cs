using RockEngine.Utils;
using RockEngine.Editor;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

string name = "RockEngineEditor";
if(args.Length > 0)
{
    name = args[0];
}
var window = new Editor(name);

window.Start();
window.Dispose();
static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var message = e.ExceptionObject + "\n";
    Logger.AddError(message);
    Console.WriteLine(message);
}