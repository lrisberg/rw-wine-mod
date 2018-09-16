using System;
using Verse;
using RimWorld;

namespace RwWineMod
{
  [DefOf]
  public static class WineJobDefOf
  {
    static WineJobDefOf()
    {
      DefOfHelper.EnsureInitializedInCtor(typeof(WineJobDefOf));
    }

    public static JobDef FillMustFermentingBarrel;
  }
}