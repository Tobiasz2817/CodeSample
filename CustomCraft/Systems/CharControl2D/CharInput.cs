using Inflowis;
using UnityEngine;

namespace CharControl2D {
    internal class CharInput {
        internal float DirX;

        internal CharInput(CharController ctr, CharView view) {
            /*GameInput.OnMove += moveDir => {
                DirX = moveDir.x;

                if (ctr.CanMove && ctr.Col2D.IsGrounded && !ctr.IsDashing && !ctr.IsJumping) {
                    switch (DirX) {
                        case 0:
                            ctr.RequestIdle();
                            break;
                        default:
                            ctr.RequestMove();
                            break;
                    }
                }
                
                view.UpdateLocomotion(Mathf.Abs(DirX));
            };

            GameInput.OnJump += () => {
                if(ctr.CanJump)
                    ctr.RequestJump();
            };
            
            GameInput.OnCutJump += () => {
                if (ctr.IsJumping)
                    ctr.RequestCutJump();
            };

            GameInput.OnDash += ctr.RequestDash;*/
        }
    }
} 