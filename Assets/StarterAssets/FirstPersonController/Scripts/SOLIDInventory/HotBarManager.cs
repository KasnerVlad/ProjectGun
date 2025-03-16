using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomInvoke;
using SmoothAnimationLogic;
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
    public static class CancelAndRestartTokens
    {
        public static CancellationTokenSource GetCancellationToken(GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            if (cancellationTokens.TryGetValue(target, out var cts))
            {
                cts.Cancel();
                cancellationTokens.Remove(target);
            }
            var newCts = new CancellationTokenSource();
            cancellationTokens[target] = newCts;
            return newCts;
        }
        public static void CancelAllAnimations(Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            foreach (var cts in cancellationTokens.Values)
            {
                cts.Cancel();
            }
            cancellationTokens.Clear();
        }
    }
}

namespace CustomInvoke
{
    public static class CastomInvoke
    {
        public static async Task Invoke(Action t, int duration)
        {
            await Task.Delay(duration);
            t.Invoke(); 
        }

        public static async Task Invoke<T>(Action<T> t, int duration, T param)
        {
            await Task.Delay(duration);
            t.Invoke(param);
        }        
        public static async Task Invoke(Action t, int duration,CancellationTokenSource ct)
        {
            await Task.Delay(duration);
            if (!ct.IsCancellationRequested) { t.Invoke(); }
        }
        public static async Task Invoke<T>(Action<T> t, int duration, T param, CancellationTokenSource ct)
        {
            await Task.Delay(duration);
            if(!ct.IsCancellationRequested){ t.Invoke(param);}
        }
        
    }
}
namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class HotBarManager : MonoBehaviour
    {
        [SerializeField] private GameObject hotBar;
        [SerializeField] private float scrollSensitivity = 1.0f;
        [SerializeField] private float minScrollStep = 0.1f;
        [SerializeField] private int HotBarsSlotsCount = 4;
        [SerializeField] private InventorySystem2 inventorySystem2Contains;
        
        private List<GameObject> hotBarsChilds = new List<GameObject>();
        [SerializeField] private List<Vector3> hotBarsChildsPositions = new List<Vector3>();
        [SerializeField] private List<InventorySlots> hotBarSlots = new List<InventorySlots>();
        [SerializeField]private int currentHotBarSlot;
        private bool open=true;
        private bool canScroll = true;
        [SerializeField] private float rateBeforeScroll = 0.1f;
        [SerializeField] private float speedScrolling = 1f;
        private List<Dictionary<GameObject, CancellationTokenSource>> CancellationTokensSourseDictionary = new List<Dictionary<GameObject, CancellationTokenSource>>();
        [SerializeField] private GameObject hotBarsHider;
        [SerializeField] private Vector3 _hotBarLockedPos;
        [SerializeField] private Vector3 _hotBarOpenPos;
        [SerializeField] private Quaternion hiderLockedRot;
        [SerializeField] private Quaternion hiderOpenRot;
        [SerializeField] private float speedHiderPosChanging;
        private readonly int _dictinaryAmount = 2;
        private bool _justOpened;
        private bool _wasMomentBeforeOpenValueTrue;
        private CancellationTokenSource _cts;
        
        private void Start()
        {
            for (int i = 0; i < HotBarsSlotsCount; i++)
            {
                hotBarsChilds.Add(hotBar.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < HotBarsSlotsCount; i++)
            {
                Vector3 position = hotBarsChilds[i].transform.localPosition;
                hotBarsChildsPositions.Add(new Vector3(position.x, position.y, position.z));
            }
            for (int i = 0; i < HotBarsSlotsCount; i++)
            {
                for (int j = 0; j < inventorySystem2Contains.slots.Count; j++)
                {
                    if (inventorySystem2Contains.slots[j].Slot==hotBarsChilds[i])
                    {
                        hotBarSlots.Add(inventorySystem2Contains.slots[i]);
                    }
                }
            }
            _hotBarOpenPos = hotBar.transform.localPosition;
            hiderLockedRot = hotBar.transform.localRotation;
            hotBarsHider.transform.SetAsLastSibling();
            hotBarsHider.transform.localRotation = hiderOpenRot;
            for (int i = 0; i < _dictinaryAmount; i++)
            {
                CancellationTokensSourseDictionary.Add(new Dictionary<GameObject, CancellationTokenSource>());
            }
            _cts = new CancellationTokenSource();
            ToggleOpenHotBar();
        }
        private void Update()
        {
            UpdateLogic();
        }
        private void UpdateLogic()
        {
            if (InventoryInput.Scroll * scrollSensitivity > minScrollStep && canScroll)
            {
                ScrollMethodInUpdate(1); 
                _wasMomentBeforeOpenValueTrue = true;
            }
            else if (InventoryInput.Scroll * scrollSensitivity < minScrollStep * -1 && canScroll) 
            {
                ScrollMethodInUpdate(-1);
                _wasMomentBeforeOpenValueTrue = true;
            }
            else if(open && _wasMomentBeforeOpenValueTrue)
            {
                _ = CastomInvoke.Invoke(()=>
                {
                    ToggleOpenHotBar();
                    if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                }, 2000, _cts);

                _wasMomentBeforeOpenValueTrue = false;
            }
            
            if(Input.GetMouseButtonDown(2)&&!open)
            {
                _cts = new CancellationTokenSource();
                ToggleOpenHotBar(); 
                _= CastomInvoke.Invoke(()=>
                {
                    ToggleOpenHotBar();
                    if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                    
                }, 4000, _cts);
            }
            else if(Input.GetMouseButtonDown(2)&&open)
            {
                if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                _=CastomInvoke.Invoke(ToggleOpenHotBar, 100);
            }
        }
        private void ScrollMethodInUpdate(int num)
        {
            if(!_cts.IsCancellationRequested){_cts.Cancel();}
            _cts = new CancellationTokenSource();
            canScroll = false;
            if (!open) 
            {             
                ToggleOpenHotBar();
                _justOpened = false; 
            }
            _=CastomInvoke.Invoke<int>(UpdateArmItem, (int)(!_justOpened? rateBeforeScroll*1000 : 0), num);
            _=CastomInvoke.Invoke<int>(SwapPositionsLogic, (int)(!_justOpened? rateBeforeScroll*1000*2:0), num) ;
            _=CastomInvoke.Invoke(ScrollTrue, (int)(rateBeforeScroll*1000));
            if(!_justOpened) _justOpened = true;
        }
        private void ScrollTrue()=>canScroll = true;
        private void SwapPositionsLogic(int num)
        {
            currentHotBarSlot = (currentHotBarSlot - num + HotBarsSlotsCount) % HotBarsSlotsCount;
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[0]);
            UpdateSlotsPositions();
        }
    
        private void UpdateSlotsPositions()
        {
            for (int i = 0; i < hotBarsChilds.Count; i++)
            {
                int newI = (i + currentHotBarSlot) % HotBarsSlotsCount;
                ChangeTransformsValueLogic.StartSmoothPositionChange(hotBarsChildsPositions[newI], hotBarsChilds[i], CancellationTokensSourseDictionary[0], rateBeforeScroll, speedScrolling);
            }
        }
        private void UpdateHotBarState()
        {
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[0]);
            if (open) 
            {
                UpdateSlotsPositions();
                ChangeTransformsValueLogic.StartSmoothPositionChange(_hotBarOpenPos, hotBar, CancellationTokensSourseDictionary[1], rateBeforeScroll, speedScrolling);
                ChangeTransformsValueLogic.StartSmoothRotationChange(hiderOpenRot, hotBarsHider, CancellationTokensSourseDictionary[1],speedHiderPosChanging);
            }
            else
            {
                foreach(var child in hotBarsChilds)
                {
                    ChangeTransformsValueLogic.StartSmoothPositionChange(Vector3.zero, child, CancellationTokensSourseDictionary[0], rateBeforeScroll, speedScrolling);
                }
                ChangeTransformsValueLogic.StartSmoothPositionChange(_hotBarLockedPos, hotBar, CancellationTokensSourseDictionary[1], rateBeforeScroll, speedScrolling);
                ChangeTransformsValueLogic.StartSmoothRotationChange(hiderLockedRot, hotBarsHider, CancellationTokensSourseDictionary[1], speedHiderPosChanging);
            }
        }
        private void ToggleOpenHotBar()
        {
            open = !open;
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[1]);
            UpdateHotBarState();
        }

        private void UpdateArmItem(int num)
        {
            foreach (var slot in hotBarsChilds)
            {
                if (slot.transform.position == hotBarsChildsPositions[0])
                {
                    //There need to be a check of what item there are, and take item logic. But I'm not realized this function yet(I'm about take item logic).
                }
            }
        }
    }
}