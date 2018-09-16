using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace RwWineMod
{
  // Token: 0x02000144 RID: 324
  public class WorkGiver_FillMustFermentingBarrel : WorkGiver_Scanner
  {
    // Token: 0x17000103 RID: 259
    // (get) Token: 0x060006BC RID: 1724 RVA: 0x0003E6B4 File Offset: 0x0003CAB4
    public override ThingRequest PotentialWorkThingRequest
    {
      get
      {
        return ThingRequest.ForDef(ThingDefOf.FermentingBarrel);
      }
    }

    // Token: 0x17000104 RID: 260
    // (get) Token: 0x060006BD RID: 1725 RVA: 0x0003E6C0 File Offset: 0x0003CAC0
    public override PathEndMode PathEndMode
    {
      get
      {
        return PathEndMode.Touch;
      }
    }

    // Token: 0x060006BE RID: 1726 RVA: 0x0003E6C3 File Offset: 0x0003CAC3
    public static void ResetStaticData()
    {
      WorkGiver_FillMustFermentingBarrel.TemperatureTrans = "BadTemperature".Translate().ToLower();
      WorkGiver_FillMustFermentingBarrel.NoMustTrans = "NoMust".Translate();
    }

    // Token: 0x060006BF RID: 1727 RVA: 0x0003E6E8 File Offset: 0x0003CAE8
    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      Building_FermentingBarrel building_FermentingBarrel = t as Building_FermentingBarrel;
      if (building_FermentingBarrel == null || building_FermentingBarrel.Fermented || building_FermentingBarrel.SpaceLeftForWort <= 0)
      {
        return false;
      }
      float ambientTemperature = building_FermentingBarrel.AmbientTemperature;
      CompProperties_TemperatureRuinable compProperties = building_FermentingBarrel.def.GetCompProperties<CompProperties_TemperatureRuinable>();
      if (ambientTemperature < compProperties.minSafeTemperature + 2f || ambientTemperature > compProperties.maxSafeTemperature - 2f)
      {
        JobFailReason.Is(WorkGiver_FillMustFermentingBarrel.TemperatureTrans, null);
        return false;
      }
      if (!t.IsForbidden(pawn))
      {
        LocalTargetInfo target = t;
        if (pawn.CanReserve(target, 1, -1, null, forced))
        {
          if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
          {
            return false;
          }
          if (this.FindMust(pawn, building_FermentingBarrel) == null)
          {
            JobFailReason.Is(WorkGiver_FillMustFermentingBarrel.NoMustTrans, null);
            return false;
          }
          return !t.IsBurning();
        }
      }
      return false;
    }

    // Token: 0x060006C0 RID: 1728 RVA: 0x0003E7D8 File Offset: 0x0003CBD8
    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      Building_FermentingBarrel barrel = (Building_FermentingBarrel)t;
      Thing t2 = this.FindMust(pawn, barrel);
      return new Job(WineJobDefOf.FillMustFermentingBarrel, t, t2);
    }
    
    private Thing FindMust(Pawn pawn, Building_FermentingBarrel barrel)
    {
      Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false);
      IntVec3 position = pawn.Position;
      Map map = pawn.Map;
      ThingRequest thingReq = ThingRequest.ForDef(WineThingDefOf.Must);
      PathEndMode peMode = PathEndMode.ClosestTouch;
      TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
      Predicate<Thing> validator = predicate;
      return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
    }
    
    private static string TemperatureTrans;
    
    private static string NoMustTrans;
  }
}
