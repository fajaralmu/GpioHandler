using System.Device.Gpio;
using Microsoft.Extensions.Logging;

namespace GpioHandler.Pin;

internal class GpioPushButton : IPushButton
{
  private readonly int _number;
  private readonly PinMode _mode;

  private readonly ILogger _logger;

  public GpioPushButton( int number, PinMode mode )
  {
    _number = number;
    _mode = mode;

    _logger = DefaultLogger.CreateNamed( $"{nameof( GpioPushButton )} #{_number}" );
  }

  public event EventHandler? OnChange;
  
  public void Initialize()
  {
    try
    {
      using var controller = new GpioController();
      controller.OpenPin( _number );
      controller.SetPinMode( _number, _mode );
    }
    catch( Exception ex )
    {
      _logger.LogError( ex, "Failed to initialize" );
    }
  }

  public int ReadValue()
  {
    using GpioController controller = new GpioController();
    controller.OpenPin( _number );
    PinValue ret = controller.Read( _number );
    return ret == PinValue.High ? 1 : 0;
  }

  public async Task RunAsync( CancellationToken token )
  {
    _logger.LogInformation( "START Wait for rising event" );
    while( !token.IsCancellationRequested )
    {
      using GpioController controller = new GpioController();
      controller.OpenPin( _number );
      
      // falling
      _logger.LogInformation( "Wait for falling event" );
      WaitForEventResult fallEvt = await controller.WaitForEventAsync( _number, PinEventTypes.Falling, token );
      _logger.LogInformation( "Event: {ev}", fallEvt.EventTypes );

      // OnChange?.Invoke( this, true );
      
      // rising
      _logger.LogInformation( "Wait for rising event" );
      WaitForEventResult riseEvt = await controller.WaitForEventAsync( _number, PinEventTypes.Rising, token );
      _logger.LogInformation( "Event: {ev}", riseEvt.EventTypes );

      OnChangeInternal();
    }
    
    OnChange?.Invoke( this, EventArgs.Empty );
  }

  private void OnChangeInternal()
  {
    OnChange?.Invoke( this, EventArgs.Empty );
  }
}
