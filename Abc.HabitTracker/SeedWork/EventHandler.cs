using System;

using Abc.HabitTracker.BadgeGainer;

namespace Abc.HabitTracker
{
    public abstract class TriggerHandler : IObserver<Trigger>
    {
        protected IBadgeRepository badgeRepository;
        protected IBadgeGainer _gainer;

        public TriggerHandler(IBadgeRepository repo, IBadgeGainer gainer)
        {
            badgeRepository = repo;
            _gainer = gainer;
        }
        public abstract void Update(Trigger e);
    }

    public class DominatingHandler : TriggerHandler
    {
        public DominatingHandler(IBadgeRepository repo, IBadgeGainer gainer) : base(repo, gainer) { }

        public override void Update(Trigger e)
        {
            if (badgeRepository == null) return;

            Dominating ev = e as Dominating;
            if (ev == null) return;

            Badge badge = _gainer.Gain();
            Badge b = badgeRepository.FindByName(badge.Name);
            if(b == null){
                badgeRepository.Create(badge);
                b = badgeRepository.FindByName(badge.Name);
            }
            badgeRepository.AddUser(b, ev.User_id);
        }
    }

    public class WorkaholicHandler : TriggerHandler
    {
        public WorkaholicHandler(IBadgeRepository repo, IBadgeGainer gainer) : base(repo, gainer) { }

        public override void Update(Trigger e)
        {
            if (badgeRepository == null) return;

            Workaholic ev = e as Workaholic;
            if (ev == null) return;

            Badge badge = _gainer.Gain();
            Badge b = badgeRepository.FindByName(badge.Name);
            if(b == null){
                badgeRepository.Create(badge);
                b = badgeRepository.FindByName(badge.Name);
            }
            badgeRepository.AddUser(b, ev.User_id);
        }
    }

    public class EpicComebackHandler : TriggerHandler
    {
        public EpicComebackHandler(IBadgeRepository repo, IBadgeGainer gainer) : base(repo, gainer) { }

        public override void Update(Trigger e)
        {
            if (badgeRepository == null) return;

            EpicComeback ev = e as EpicComeback;
            if (ev == null) return;

            Badge badge = _gainer.Gain();
            Badge b = badgeRepository.FindByName(badge.Name);
            if(b == null){
                badgeRepository.Create(badge);
                b = badgeRepository.FindByName(badge.Name);
            }
            badgeRepository.AddUser(b, ev.User_id);
        }
    }
}