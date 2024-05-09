// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;
using GpioHandler;
using GpioHandler.Pin;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using GpioPin = GpioHandler.Pin.GpioPin;

Logger logger = ConfigureNLog( "nlog.config" ).GetCurrentClassLogger();
DefaultLogger.SetLoggerFactory( new NLogLoggerFactory() );

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile( "appsettings.json", optional: false).Build();

AppConfig appSettings = new AppConfig();
config.GetSection( nameof( AppConfig ) ).Bind( appSettings );

logger.Log( LogLevel.Info, "Starting GPIO Handler" );

IList<IGpioPin> pins = new List<IGpioPin>();
foreach ( PinHandler handler in appSettings.PinHandlers )
{
  var pin = new GpioPin( handler.PinNumber, PinMode.Input );
  pin.Initialize();
  pins.Add( pin );
}

IEnumerable<Task> tasks = pins.Select( pin => pin.RunAsync( CancellationToken.None ) );
await Task.WhenAll( tasks );

logger.Log( LogLevel.Info, "Stopping GPIO Handler" );
