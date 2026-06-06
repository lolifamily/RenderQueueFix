#define HAS_PRELOADER_PATCHES

#if HAS_PRELOADER_PATCHES

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// DO NOT USE A NAMESPACE HERE!
// CRITICAL: Using a namespace here will prevent Pulsar from finding the Preloader class.

public class Preloader
{
    private const int OriginalFirstLevelCount = 1024;
    private const int NewFirstLevelCount = 4096;

    public static IEnumerable<string> TargetDLLs { get; } =
    [
        "VRage.Render.dll"
    ];

    public static void Initialize()
    {
    }

    public static void Patch(AssemblyDefinition assembly)
    {
        Log($"Patching {assembly.Name.Name}");

        var updateFrameType = assembly.MainModule.Types
            .FirstOrDefault(t => t.Name == "MyUpdateFrame");
        if (updateFrameType == null)
        {
            Log("ERROR: MyUpdateFrame not found");
            return;
        }

        var queueType = updateFrameType.NestedTypes
            .FirstOrDefault(t => t.Name == "MyConcurrentTwoLevelQueue");
        if (queueType == null)
        {
            Log("ERROR: MyConcurrentTwoLevelQueue not found");
            return;
        }

        var ctor = queueType.Methods.FirstOrDefault(m => m.IsConstructor && !m.IsStatic);
        if (ctor == null)
        {
            Log("ERROR: Constructor not found");
            return;
        }

        var messagesField = queueType.Fields.FirstOrDefault(f => f.Name == "m_messages");
        if (messagesField == null)
        {
            Log("ERROR: m_messages field not found");
            return;
        }

        var il = ctor.Body.Instructions;
        Log($"Constructor has {il.Count} IL instructions");

        var patched = false;
        for (var i = 0; i < il.Count - 2; i++)
        {
            if (il[i].OpCode != OpCodes.Ldc_I4 || (int)il[i].Operand != OriginalFirstLevelCount)
                continue;

            if (il[i + 1].OpCode != OpCodes.Newarr)
                continue;

            if (il[i + 2].OpCode != OpCodes.Stfld
                || il[i + 2].Operand is not FieldReference fieldRef
                || fieldRef.FullName != messagesField.FullName)
                continue;

            Log($"Found target at IL_{il[i].Offset:X4}: ldc.i4 {OriginalFirstLevelCount} -> {NewFirstLevelCount}");
            il[i].Operand = NewFirstLevelCount;
            patched = true;
            break;
        }

        Log(patched ? "Patch applied successfully" : "ERROR: Pattern not matched, no patch applied");
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[RenderQueueFix] {message}");
    }

    public static void Finish()
    {
    }
}

#endif
