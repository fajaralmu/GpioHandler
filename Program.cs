// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;
using GpioHandler;
using GpioHandler.Pin;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using GpioPin = GpioHandler.Pin.GpioPin;

System.Console.WriteLine("Starting....");

Logger logger = ConfigureNLog( "nlog.config" ).GetCurrentClassLogger();
DefaultLogger.SetLoggerFactory( new NLogLoggerFactory() );

System.Console.WriteLine("Logger has been set");

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile( "appsettings.json", optional: false).Build();

AppConfig appSettings = new AppConfig();
config.GetSection( nameof( AppConfig ) ).Bind( appSettings );

System.Console.WriteLine("Config has been setup");

logger.Log( LogLevel.Info, "Starting GPIO Handler" );

IList<IGpioPin> pins = new List<IGpioPin>();
foreach ( PinHandler handler in appSettings.PinHandlers )
{
  var pin = new GpioPin( handler.PinNumber, PinMode.Input );
  pin.Initialize();
  pins.Add( pin );
}

Task[] tasks = pins.Select( pin => pin.RunAsync( CancellationToken.None ) ).ToArray();

Console.WriteLine( "Pin tasks: {0}", tasks.Length );

await Task.WhenAll( tasks );

logger.Log( LogLevel.Info, "Stopping GPIO Handler" );