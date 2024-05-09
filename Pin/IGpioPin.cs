namespace GpioHandler.Pin;

public interface IGpioPin
{
  event EventHandler<bool>? OnChange;
  void Initialize();
  int ReadValue();
  void TriggerInternal();
  Task RunAsync( CancellationToken token );
}