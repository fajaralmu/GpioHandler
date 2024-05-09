namespace GpioHandler.Pin;

public interface IPushButton
{
  event EventHandler? OnChange;
  void Initialize();
  Task RunAsync( CancellationToken token );
}