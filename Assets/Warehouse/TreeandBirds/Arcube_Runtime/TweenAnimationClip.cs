using UnityEngine;

namespace Arcube.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimationClip", menuName = "AnimationClip/TweenAnimationClip")]
    public class TweenAnimationClip : AnimationClip
    {
        [TextArea(2, 10)] public string script;
        public Arcube.Animation.PlayMethod playMethod = Arcube.Animation.PlayMethod.All;
    }
}