using System.Reflection;
using System.Runtime.Loader;

namespace AutoWire.AssemblyScanner;
public class Scanner
{
    private readonly ScannerOptions _options;
    private readonly List<string> _scannedAssemblyNames;
    private readonly IEnumerable<IWireContainer> _wireContainers;
    private readonly string _baseDirectoryToScan;

    public Scanner(ScannerOptions options, IEnumerable<IWireContainer> wireContainers) : this(options, wireContainers, AppDomain.CurrentDomain.BaseDirectory)
    {
    }

    public Scanner(ScannerOptions options, IEnumerable<IWireContainer> wireContainers, string baseDirectoryToScan)
    {
        _options = options;
        _scannedAssemblyNames = new();
        _wireContainers = wireContainers;
        _baseDirectoryToScan = baseDirectoryToScan;
    }

    public void Scan()
    {
        if (_options.ScanAppDomainAssemblies)
            ScanAppDomainAssemblies();

        if (_options.ScanFileSystemAssemblies)
            ScanAssembliesFromDirectory();
    }

    private void ScanAppDomainAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            WireContainers(assembly);
        }
    }

    private void ScanAssembliesFromDirectory()
    {
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());

        var directory = new DirectoryInfo(_baseDirectoryToScan);
        var searchOptions = SearchOption.AllDirectories;
        foreach (var file in directory.EnumerateFiles("*.dll", searchOptions))
        {
            if (!context!.Assemblies.Any(a => a.Location.Equals(file.FullName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var assembly = context.LoadFromAssemblyPath(file.FullName);
                WireContainers(assembly);
            }
        }
    }

    private void WireContainers(Assembly assembly)
    {
        var assemblyFullName = assembly.FullName!.ToLowerInvariant();
        if (_scannedAssemblyNames.Contains(assemblyFullName))
            return;

        foreach (var wireContainer in _wireContainers)
            wireContainer.Wire(assembly);

        _scannedAssemblyNames.Add(assemblyFullName);
    }
}
