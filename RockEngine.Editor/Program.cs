using RockEngine.Common.Utils;
using RockEngine.Editor;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

string name = "RockEngineEditor";
if(args.Length > 0)
{
    name = args[0];
}
using var app = new Editor(name);
{
    app.Start();
}

static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var message = e.ExceptionObject + "\n";
    Logger.AddError(message);
    Console.WriteLine(message);
}
