using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld.BaseGen;

namespace RwWineMod
{
  public class SymbolResolver_AddMustToFermentingBarrels : SymbolResolver
  {
    public override void Resolve(ResolveParams rp)
    {
      Map map = BaseGen.globalSettings.map;
      SymbolResolver_AddMustToFermentingBarrels.barrels.Clear();
      CellRect.CellRectIterator iterator = rp.rect.GetIterator();
      while (!iterator.Done())
      {
        List<Thing> thingList = iterator.Current.GetThingList(map);
        for (int i = 0; i < thingList.Count; i++)
        {
          Building_FermentingBarrel building_FermentingBarrel = thingList[i] as Building_FermentingBarrel;
          if (building_FermentingBarrel != null && !SymbolResolver_AddMustToFermentingBarrels.barrels.Contains(building_FermentingBarrel))
          {
            SymbolResolver_AddMustToFermentingBarrels.barrels.Add(building_FermentingBarrel);
          }
        }
        iterator.MoveNext();
      }
      float progress = Rand.Range(0.1f, 0.9f);
      for (int j = 0; j < SymbolResolver_AddMustToFermentingBarrels.barrels.Count; j++)
      {
        if (!SymbolResolver_AdMustToFermentingBarrels.barrels[j].Fermented)
        {
          int num = Rand.RangeInclusive(1, 25);
          num = Mathf.Min(num, SymbolResolver_AddMustToFermentingBarrels.barrels[j].SpaceLeftForWort);
          if (num > 0)
          {
            SymbolResolver_AddMustToFermentingBarrels.barrels[j].AddWort(num);
            SymbolResolver_AddMustToFermentingBarrels.barrels[j].Progress = progress;
          }
        }
      }
      SymbolResolver_AddMustToFermentingBarrels.barrels.Clear();
    }
    
    private static List<Building_FermentingBarrel> barrels = new List<Building_FermentingBarrel>();
  }
}
