using System.Reflection;

namespace AutoWire.AssemblyScanner;
public interface IWireContainer
{
    void Wire(Assembly assembly);
}
