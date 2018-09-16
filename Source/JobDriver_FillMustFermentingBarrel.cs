using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace RwWineMod
{
  public class JobDriver_FillMustFermentingBarrel : JobDriver
  {
    protected Building_FermentingBarrel Barrel
    {
      get
      {
        return (Building_FermentingBarrel)this.job.GetTarget(TargetIndex.A).Thing;
      }
    }
    
    protected Thing Must
    {
      get
      {
        return this.job.GetTarget(TargetIndex.B).Thing;
      }
    }
    
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
      Pawn pawn = this.pawn;
      LocalTargetInfo target = this.Barrel;
      Job job = this.job;
      bool result;
      if (pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
      {
        pawn = this.pawn;
        target = this.Must;
        job = this.job;
        result = pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
      }
      else
      {
        result = false;
      }
      return result;
    }
    
    protected override IEnumerable<Toil> MakeNewToils()
    {
      this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
      this.FailOnBurningImmobile(TargetIndex.A);
      base.AddEndCondition(() => (this.Barrel.SpaceLeftForWort > 0) ? JobCondition.Ongoing : JobCondition.Succeeded);
      yield return Toils_General.DoAtomic(delegate
      {
        this.job.count = this.Barrel.SpaceLeftForWort;
      });
      Toil reserveMust = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
      yield return reserveMust;
      yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
      yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
      yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveMust, TargetIndex.B, TargetIndex.None, true, null);
      yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
      yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
      yield return new Toil
      {
        initAction = delegate ()
        {
          this.Barrel.AddWort(this.Must);
        },
        defaultCompleteMode = ToilCompleteMode.Instant
      };
      yield break;
    }
    
    private const TargetIndex BarrelInd = TargetIndex.A;
    
    private const TargetIndex MustInd = TargetIndex.B;
    
    private const int Duration = 200;
  }
}
