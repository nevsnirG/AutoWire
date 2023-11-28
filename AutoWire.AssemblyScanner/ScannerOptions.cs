namespace AutoWire.AssemblyScanner;
public sealed class ScannerOptions
{
    /// <summary>
    /// Scan all assemblies in the current <see cref="AppDomain"/>.
    /// </summary>
    public bool ScanAppDomainAssemblies { get; set; } = true;
    /// <summary>
    /// Scan the file system for assemblies.
    /// </summary>
    public bool ScanFileSystemAssemblies { get; set; } = true;
    /// <summary>
    /// If <see cref="ScanFileSystemAssemblies"> is set to true, only assemblies in the top directory are scanned.
    /// </summary>
    public bool ScanFileSystemAssembliesRecursively { get; set; } = true;

    public static ScannerOptions Default => new();
}
