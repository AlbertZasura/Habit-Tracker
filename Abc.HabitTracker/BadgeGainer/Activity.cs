using System;

namespace Abc.HabitTracker.BadgeGainer
{
    public class ActivityDominating : IBadgeGainer
    {
        public Badge Gain(){
            return new Badge("Dominating","4+ streak");
        }
    }

    public class ActivityWorkaholic : IBadgeGainer
    {
        public Badge Gain(){
            return new Badge("Workaholic","Doing someworks on daysoff");
        }
    }

    public class ActivityEpicComeback : IBadgeGainer
    {
        public Badge Gain(){
            return new Badge("EpicComeback","10 streak after 10 days without logging");
        }
    }
}