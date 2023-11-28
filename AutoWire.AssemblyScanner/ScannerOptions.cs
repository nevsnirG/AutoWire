namespace AutoWire.AssemblyScanner;
public sealed class ScannerOptions
{
    public bool ScanAppDomainAssemblies { get; set; } = true;
    public bool ScanFileSystemAssemblies { get; set; } = true;
    public bool ScanDependencies { get; set; } = true; //TODO - Implement
}
