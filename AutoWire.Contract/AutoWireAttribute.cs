namespace AutoWire.Core;
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class AutoWireAttribute : Attribute
{
}
