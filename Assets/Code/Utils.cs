using DG.Tweening;

namespace JamSpace
{
    public static class Utils
    {
        public static void TryKill(this Tween t, bool complete = false)
        {
            if (t is { active: true })
                t.Kill(complete);
        }
    }
}