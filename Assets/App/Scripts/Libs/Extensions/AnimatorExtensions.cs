using UnityEngine;

namespace Chsopoly.Libs.Extensions
{
    public static class AnimatorExtensions
    {
        public static bool IsPlaying (this Animator animator, string stateName)
        {
            // Returns a next animation if transiting.
            if (animator.IsInTransition (0))
            {
                return animator.GetNextAnimatorStateInfo (0).IsName (stateName);
            }
            return animator.GetCurrentAnimatorStateInfo (0).IsName (stateName);
        }
    }
}