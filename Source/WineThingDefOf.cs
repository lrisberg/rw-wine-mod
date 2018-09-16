using System;
using Verse;
using RimWorld;

namespace RwWineMod
{
  [DefOf]
  public static class WineThingDefOf
  {
    static WineThingDefOf()
    {
      DefOfHelper.EnsureInitializedInCtor(typeof(WineThingDefOf));
    }

    public static ThingDef Must;
  }
}

