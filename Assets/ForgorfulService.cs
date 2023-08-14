using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

public class ForgorfulService : MonoBehaviour
{
    private static bool _harmed;
    private void Start()
    {
        Harmony harm = new Harmony("iForgor.BDB");
        System.Reflection.MethodInfo m = ReflectionHelper.FindTypeInGame("BombGenerator").Method("CreateBomb");
        harm.Patch(m, transpiler: new HarmonyMethod(typeof(ForgorfulService).Method("Transpile")));
        _harmed = true;
    }

    private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> il)
    {
        System.Reflection.MethodInfo m = ReflectionHelper.FindTypeInGame("StaticCombiner").Method("CombineStatic");
        foreach (CodeInstruction i in il)
        {
            if (i.Calls(m))
                i.operand = typeof(ForgorfulService).Method("DummyCreateStatic");
            yield return i;
        }
    }

    private static void DummyCreateStatic(GameObject root) { }
}
