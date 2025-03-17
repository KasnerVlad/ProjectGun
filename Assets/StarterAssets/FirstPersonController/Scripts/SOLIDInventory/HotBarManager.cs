using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using CustomInvoke;
using SmoothAnimationLogic;
using CTSCancelLogic;
namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class HotBarManager : MonoBehaviour
    {
        [SerializeField] private GameObject hotBar;
        [SerializeField] private float scrollSensitivity = 1.0f;
        [SerializeField] private float minScrollStep = 0.1f;
        [SerializeField] private int HotBarsSlotsCount = 4;
        private List<InventorySlots> slots;
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
        private bool _justHotBarOpened;
        private bool _wasMomentBeforeOpenValueTrue;
        private CancellationTokenSource _cts;
        [SerializeField] private GameObject inventory;
        private bool _justInventoryOpened;
        public void Initialize(List<InventorySlots> _slots)=>slots = _slots;
        private void Start()
        {
            for (int i = 0; i < HotBarsSlotsCount; i++) { hotBarsChilds.Add(hotBar.transform.GetChild(i).gameObject); }
            for (int i = 0; i < HotBarsSlotsCount; i++) { hotBarsChildsPositions.Add(hotBarsChilds[i].transform.localPosition); }
            for (int i = 0; i < HotBarsSlotsCount; i++)
            {
                for (int j = 0; j < slots.Count; j++)
                {
                    if (slots[j].Slot==hotBarsChilds[i])
                    {
                        hotBarSlots.Add(slots[i]);
                    }
                }
            }
            _hotBarOpenPos = hotBar.transform.localPosition;
            hiderLockedRot = hotBar.transform.localRotation;
            hotBarsHider.transform.SetAsLastSibling();
            hotBarsHider.transform.localRotation = hiderOpenRot;
            for (int i = 0; i < _dictinaryAmount; i++) { CancellationTokensSourseDictionary.Add(new Dictionary<GameObject, CancellationTokenSource>()); }
            _cts = new CancellationTokenSource();
            ToggleOpenHotBar();
        }
        private void Update() => UpdateLogic();
        private void UpdateLogic()
        {
            if (InventoryInput.Scroll * scrollSensitivity > minScrollStep && canScroll) { ScrollMethodInUpdate(1); _wasMomentBeforeOpenValueTrue = true; }
            else if (InventoryInput.Scroll * scrollSensitivity < minScrollStep * -1 && canScroll) { ScrollMethodInUpdate(-1); _wasMomentBeforeOpenValueTrue = true; }
            else if(open && _wasMomentBeforeOpenValueTrue&&!inventory.activeSelf) {
                _ = CastomInvoke.Invoke(()=>
                {
                    ToggleOpenHotBar();
                    if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                }, 2000, _cts);
                _wasMomentBeforeOpenValueTrue = false;
            }
            if(Input.GetMouseButtonDown(2)&&!open&&!inventory.activeSelf) {
                _cts = new CancellationTokenSource();
                ToggleOpenHotBar(); 
                _= CastomInvoke.Invoke(()=>
                {
                    ToggleOpenHotBar();
                    if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                    
                }, 4000, _cts);
            }
            else if(Input.GetMouseButtonDown(2)&&open&&!inventory.activeSelf) { if (!_cts.IsCancellationRequested) { _cts.Cancel(); } _=CastomInvoke.Invoke(ToggleOpenHotBar, 100); }
            if(inventory.activeSelf&&!open) { ToggleOpenHotBar(); _justInventoryOpened = true; }
            if(!inventory.activeSelf&&open&&_justInventoryOpened){ToggleOpenHotBar();_justInventoryOpened = false;}
        }
        private void ScrollMethodInUpdate(int num)
        {
            if(!_cts.IsCancellationRequested){_cts.Cancel();}
            _cts = new CancellationTokenSource();
            canScroll = false;
            if (!open) 
            {             
                ToggleOpenHotBar();
                _justHotBarOpened = false; 
            }
            _=CastomInvoke.Invoke<int>(UpdateArmItem,duration: (int)(!_justHotBarOpened? rateBeforeScroll*1000 : 0),param: num);
            _=CastomInvoke.Invoke<int>(SwapPositionsLogic,duration: (int)(!_justHotBarOpened? rateBeforeScroll*1000*2:0),param: num) ;
            _=CastomInvoke.Invoke(ScrollTrue, (int)(rateBeforeScroll*1000));
            if(!_justHotBarOpened) _justHotBarOpened = true;
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