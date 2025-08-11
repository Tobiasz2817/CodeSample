using AnimationView;

namespace CharControl2D {
    internal class CharView {
        CharModel _model;
        CharViewData ViewData => _model.ViewData;
        
        int TrackID => ViewData.TrackId;
        
        internal CharView(CharModel model) => _model = model;

        internal void EndJump() => AnimationService.SetAnimation(TrackID, ViewData.JumpEnd, false);
        internal void ViewJump() => AnimationService.SetAnimation(TrackID, ViewData.JumpStart.Name, false);
        internal void ViewFalling() => AnimationService.SetAnimation(TrackID, ViewData.Falling, false);
        internal void ViewRollForward() => AnimationService.SetAnimation(TrackID, ViewData.RollForward.Name, false);
        internal void ViewRollBackward() => AnimationService.SetAnimation(TrackID, ViewData.RollBackward.Name, false);

        internal void UpdateLocomotion(float value) => AnimationService.SetFloat(ViewData.Locomotion, value);
        internal float GetLocomotion() => AnimationService.GetFloat(ViewData.Locomotion);
    }
}