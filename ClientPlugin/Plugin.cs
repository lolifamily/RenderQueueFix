using VRage.Plugins;

// Set the assembly version manually if compiled by Pulsar (it won't create what was in AssemblyInfo.cs before)
#if !DEV_BUILD
using System.Reflection;
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#endif

namespace ClientPlugin;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public void Init(object gameInstance)
    {
    }

    public void Dispose()
    {
    }

    public void Update()
    {
    }
}