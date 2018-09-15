using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace RwWineMod
{
  // Token: 0x02000068 RID: 104
  public class JobDriver_FillMustFermentingBarrel : JobDriver
  {
    // Token: 0x17000097 RID: 151
    // (get) Token: 0x060002E7 RID: 743 RVA: 0x0001C770 File Offset: 0x0001AB70
    protected Building_FermentingBarrel Barrel
    {
      get
      {
        return (Building_FermentingBarrel)this.job.GetTarget(TargetIndex.A).Thing;
      }
    }

    // Token: 0x17000098 RID: 152
    // (get) Token: 0x060002E8 RID: 744 RVA: 0x0001C798 File Offset: 0x0001AB98
    protected Thing Must
    {
      get
      {
        return this.job.GetTarget(TargetIndex.B).Thing;
      }
    }

    // Token: 0x060002E9 RID: 745 RVA: 0x0001C7BC File Offset: 0x0001ABBC
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

    // Token: 0x060002EA RID: 746 RVA: 0x0001C824 File Offset: 0x0001AC24
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

    // Token: 0x0400020C RID: 524
    private const TargetIndex BarrelInd = TargetIndex.A;

    // Token: 0x0400020D RID: 525
    private const TargetIndex MustInd = TargetIndex.B;

    // Token: 0x0400020E RID: 526
    private const int Duration = 200;
  }
}
