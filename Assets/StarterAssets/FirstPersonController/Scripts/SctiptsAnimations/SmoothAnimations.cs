using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using CTSCancelLogic;
using UnityEngine.UI;
namespace SmoothAnimationLogic
{
    public static class SmoothChangeValueLogic
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
                    duration*Time.deltaTime
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
                    rateBeforeScroll * duration * 60*Time.deltaTime
                );
                await Task.Yield();
            }
        }
        private static async Task AlphaImageAnim(float value, GameObject targetG,float colorSpeed, CancellationToken cts)
        {
            Image target = targetG.GetComponent<Image>();
            SpriteRenderer spr = targetG.GetComponent<SpriteRenderer>();
            if (target != null)
            {
                while (target.color.a != value&&!cts.IsCancellationRequested)
                {
                    target.color = new Color(target.color.r, target.color.b, target.color.g, Mathf.MoveTowards(target.color.a, value, colorSpeed * Time.deltaTime));
                    if(target.color.a ==0){target.gameObject.SetActive(false); return;}
                    await Task.Yield();
                }
            }
            else if (spr != null)
            {
                while (spr.color.a != value&&!cts.IsCancellationRequested)
                {
                    spr.color = new Color(spr.color.r, spr.color.b, spr.color.g, Mathf.MoveTowards(spr.color.a, value, colorSpeed * Time.deltaTime));
                    if(spr.color.a ==0){spr.gameObject.SetActive(false); return;}
                    await Task.Yield();
                }
            }
        }
        private static async Task SliderAnim(float value, GameObject sliderG,float speed, CancellationToken token)
        {
            Slider slider = sliderG.GetComponent<Slider>();
            while (slider.value != value&&!token.IsCancellationRequested)
            {
                slider.value = Mathf.MoveTowards(slider.value, value, speed * Time.deltaTime);
                await Task.Yield();
            }
        }
        public static void StartSmoothPositionChange(Vector3 pos, GameObject target,
            Dictionary<GameObject, CancellationTokenSource> cancellationTokens, float duration, float rateBeforeScroll)
        {
            _ = SmoothPosChanging(pos, target, CancelAndRestartTokens.GetCancellationToken(target, cancellationTokens).Token, duration, rateBeforeScroll);
        }
        public static void StartSmoothRotationChange(Quaternion pos, GameObject target,
            Dictionary<GameObject, CancellationTokenSource> cancellationTokens, float duration)
        {
            _ = SmoothRotChanging(pos, target, CancelAndRestartTokens.GetCancellationToken(target, cancellationTokens).Token, duration);
        }

        public static void StartSmoothColorAlphaChange(float a, GameObject target,
            Dictionary<GameObject, CancellationTokenSource> cancellationTokens, float duration)
        {
            _ = AlphaImageAnim(a, target, duration,
                CancelAndRestartTokens.GetCancellationToken(target, cancellationTokens).Token);
        }
        public static void StartSmoothSliderValueChange(float pos, GameObject target,
            Dictionary<GameObject, CancellationTokenSource> cancellationTokens, float duration)
        {
            _=SliderAnim(pos, target, duration, CancelAndRestartTokens.GetCancellationToken(target, cancellationTokens).Token);
        }
    }
}