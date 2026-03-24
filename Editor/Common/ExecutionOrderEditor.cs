using Framework.Module;

using System;

using UnityEditor;

namespace Framework.Editor
{
    public class ExecutionOrderEditor : UnityEditor.Editor
    {
        const int order = -10000;
        static Type targetType = typeof(ModuleEntry);

        [MenuItem("Framework/Set Execution Order")]
        public static void SetExecutionOrder()
        {
            foreach (MonoScript script in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (script.GetClass() == targetType && MonoImporter.GetExecutionOrder(script) != order)
                {
                    MonoImporter.SetExecutionOrder(script, order);
                }
            }
        }
    }
}