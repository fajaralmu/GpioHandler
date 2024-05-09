namespace GpioHandler;

public class AppConfig
{
  public IEnumerable<PinHandler> PinHandlers { get; set; }
}

public class PinHandler
{
  public int PinNumber { get; set; }
  public string OnChangeCommand { get; set; }
  public string CommandWorkDir { get; set; }
}