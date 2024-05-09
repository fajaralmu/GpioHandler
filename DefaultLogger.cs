using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GpioHandler;

public static class DefaultLogger
{
  private static ILoggerFactory? Factory;

  public static void SetLoggerFactory( ILoggerFactory factory )
  {
    Factory = factory;
  }

  public static ILogger Create<T>()
  {
    return Factory == null ? NullLogger.Instance : Factory.CreateLogger<T>();
  }

  public static ILogger CreateNamed( string name )
  {
    return Factory == null ? NullLogger.Instance : Factory.CreateLogger( name );
  }
}
