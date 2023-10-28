﻿
using RockEngine.DI;

using RockEngine;
using RockEngine.Utils;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

IoC.Setup();
string name = "Engine";
if(args.Length > 0)
{
    name = args[0];
}
var window = new Game(name);

window.Start();
window.Dispose();
static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var message = e.ExceptionObject + "\n";
    Logger.AddError(message);
    Console.WriteLine(message);
}