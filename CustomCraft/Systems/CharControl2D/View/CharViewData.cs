using AnimationView;
using UnityEngine;

namespace CharControl2D {
    [CreateAssetMenu(menuName = "CharControl/Data/View", fileName = "CharView")]
    public class CharViewData : ScriptableObject {
        [Header("References")]
        [SerializeField] RuntimeAnimatorController _controller;
        
        [Header("Animation Names")]
        [StateName] [SerializeField] internal string Locomotion;
        [StateName] [SerializeField] internal string JumpEnd;
        [StateName] [SerializeField] internal string Falling;
        [StateName] [SerializeField] internal string Dash;

        [Header("Animation Datas")]
        [StateData] [SerializeField] internal StateData RollForward; 
        [StateData] [SerializeField] internal StateData RollBackward; 
        [StateData] [SerializeField] internal StateData JumpStart;

        internal readonly int TrackId = 0;
    }
    
#if SPINE
    
    [Serializable]
    internal struct AnimationNames {
        [SpineAnimation(dataField: "SkeletonAnimation")] [SerializeField] internal string Idle;
        [SpineAnimation(dataField: "SkeletonAnimation")] [SerializeField] internal string Move;
        [SpineAnimation(dataField: "SkeletonAnimation")] [SerializeField] internal string JumpStart;
        [SpineAnimation(dataField: "SkeletonAnimation")] [SerializeField] internal string JumpUp;
        [SpineAnimation(dataField: "SkeletonAnimation")] [SerializeField] internal string Flying;
        [SpineAnimation(dataField: "SkeletonAnimation")] [SerializeField] internal string Falling;
        
        internal static AnimationNames Default => new() {
            Idle = "idle",
            Move = "move",
            JumpStart = "0/jump start",
            JumpUp = "0/jump up",
            Flying = "0/jump on air",
            Falling = "0/jump fall",
        };
    }
#endif
}