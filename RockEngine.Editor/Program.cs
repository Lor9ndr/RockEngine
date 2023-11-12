using RockEngine.Utils;
using RockEngine.Editor;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

/*var window1 = WindowManager.CreateWindowInThread("first", 1280, 720, out var t1);
window1.OnRender += Window1_OnRender;

var window2 = WindowManager.CreateWindowInThread("Second", 1280, 720, out var t2);
window2.OnRender += Window2_OnRender;

void Window2_OnRender()
{
    Console.WriteLine("RENDER 2");
}

void Window1_OnRender()
{
    Console.WriteLine("RENDER 1");
}

await Task.WhenAll(t1,t2);

GLFW.Terminate();*/