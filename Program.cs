// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;
using System.Diagnostics;
using GpioHandler;
using GpioHandler.Pin;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

System.Console.WriteLine("Starting....");

Logger logger = ConfigureNLog( "nlog.config" ).GetCurrentClassLogger();
DefaultLogger.SetLoggerFactory( new NLogLoggerFactory() );

System.Console.WriteLine("Logger has been set");

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile( "appsettings.json", optional: false).Build();

AppConfig appSettings = new AppConfig();
config.GetSection( nameof( AppConfig ) ).Bind( appSettings );

System.Console.WriteLine("Config has been setup");

logger.Log( LogLevel.Info, "Starting GPIO Handler" );

IList<IPushButton> buttons = new List<IPushButton>();
foreach ( PinHandler handler in appSettings.PinHandlers )
{
  var button = new GpioPushButton( handler.PinNumber, PinMode.Input );
  button.Initialize();
  button.OnChange += ( s, _ ) =>
  {
    Console.WriteLine( "Button {0} pressed", handler.PinNumber );
    Console.WriteLine( "Will execute command: {0}", handler.OnChangeCommand );
    Process p = new Process()
    {
      StartInfo = new ProcessStartInfo()
      {
        FileName = "/bin/bash",
        WorkingDirectory = handler.CommandWorkDir,
        Arguments = handler.OnChangeCommand,
        RedirectStandardOutput = true,
        CreateNoWindow = true,

      }
    };
    var started = p.Start();
    Console.WriteLine( "Process started={0}", started );
    if ( started )
    {
      p.WaitForExit();
      Console.WriteLine( "Process end. Result={0}", p.StandardOutput.ReadToEnd() );
    }
  };
  buttons.Add( button );
}

Task[] tasks = buttons.Select( pin => pin.RunAsync( CancellationToken.None ) ).ToArray();

Console.WriteLine( "Pin tasks: {0}", tasks.Length );

await Task.WhenAll( tasks );

logger.Log( LogLevel.Info, "Stopping GPIO Handler" );