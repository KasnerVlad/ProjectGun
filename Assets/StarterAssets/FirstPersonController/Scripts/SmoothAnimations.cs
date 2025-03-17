using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using CTSCancelLogic;
namespace SmoothAnimationLogic
{
    public static class ChangeTransformsValueLogic
    {
        private static async Task SmoothRotChanging(Quaternion pos, GameObject target, CancellationToken cancellationToken, float duration)
        {
            while (target.transform.localRotation != pos && 
                   !cancellationToken.IsCancellationRequested && 
                   Application.isPlaying)
            {
                target.transform.localRotation = Quaternion.RotateTowards(
                    target.transform.localRotation, 
                    pos, 
                    duration
                );
                await Task.Yield();
            }
        }
        private static async Task SmoothPosChanging(Vector3 pos, GameObject target, CancellationToken cancellationToken, float duration, float rateBeforeScroll)
        {
            while (target.transform.localPosition != pos && 
                  !cancellationToken.IsCancellationRequested && 
                  Application.isPlaying)
            {
                target.transform.localPosition = Vector3.MoveTowards(
                    target.transform.localPosition, 
                    pos, 
                    rateBeforeScroll * duration * 60
                );
                await Task.Yield();
            }
        }
        public static void StartSmoothPositionChange(Vector3 pos, GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens, float duration, float rateBeforeScroll)
        {
            _ = SmoothPosChanging(pos, target, CancelAndRestartTokens.GetCancellationToken(target, cancellationTokens).Token, duration, rateBeforeScroll);
        }
        public static void StartSmoothRotationChange(Quaternion pos, GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens, float duration)
        {
            _ = SmoothRotChanging(pos, target, CancelAndRestartTokens.GetCancellationToken(target, cancellationTokens).Token, duration);
        }
    }
}