using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using CInvoke;
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
        public int currentHotBarSlot{get; private set;}
        private bool open=true;
        private bool canScroll = true;
        [SerializeField] private float rateBeforeScroll = 0.1f;
        [SerializeField] private float rateBeforeScroll2 = 0.1f;
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
        [Tooltip("Add 2 sprite, in first slot drag non Selected Slot Sprites in second drag Selected Slot Sprites")]
        [SerializeField] private List<Sprite> slotSprites= new List<Sprite>();
        private List<Image> slotsImages = new List<Image>();
        public void Initialize(List<InventorySlots> _slots)=>slots = _slots;
        private void LoadCurrentHotBarSlot(int num){currentHotBarSlot=num;}

        private void Awake()
        {
            InventoryEvents.OnInit += Init;
            InventoryEvents.OnLoad += (() => {               
                UpdateSlotsPositions();
                UpdateSlotsSprites();
                UpdateArmItem(); });
        }

        private void Init()
        {
            SaveManager._GameSaveManager.InitializeHotBarManager(this, LoadCurrentHotBarSlot);
        }
        private void OnDisable() => SaveManager._GameSaveManager.SaveCurrentHotBarSlot();
        private void Start() => StartLogic();
        private void StartLogic()
        {
            for (int i = 0; i < HotBarsSlotsCount; i++) { hotBarsChilds.Add(hotBar.transform.GetChild(i).gameObject); }
            for (int i = 0; i < HotBarsSlotsCount; i++) { hotBarsChildsPositions.Add(hotBarsChilds[i].transform.localPosition); }
            for(int i = 0; i < HotBarsSlotsCount; i++) {slotsImages.Add(hotBarsChilds[i].transform.gameObject.GetComponent<Image>());}
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
            
            UpdateSlotsSprites();
            UpdateArmItem();
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[0]);
            _=CustomInvoke.Invoke(()=>
            {
                UpdateSlotsPositions();
                UpdateSlotsSprites();
                UpdateArmItem();
            }, 1100);
            
            _=CustomInvoke.Invoke(ToggleOpenHotBar, 1000*3);
            InventoryEvents.OnSlotsItemChanged += UpdateArmItem;
        }
        private void OnDestroy(){InventoryEvents.OnSlotsItemChanged-=UpdateArmItem;InventoryEvents.OnInit -= Init;}
        private void Update() => UpdateLogic();
        private void UpdateLogic()
        {
            if (Input2.Scroll * scrollSensitivity > minScrollStep && canScroll) { ScrollMethodInUpdate(1); _wasMomentBeforeOpenValueTrue = true; }
            else if (Input2.Scroll * scrollSensitivity < minScrollStep * -1 && canScroll) { ScrollMethodInUpdate(-1); _wasMomentBeforeOpenValueTrue = true; }
            else if(open && _wasMomentBeforeOpenValueTrue&&!inventory.activeSelf) {
                _ = CustomInvoke.Invoke(()=>
                {
                    ToggleOpenHotBar();
                    if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                }, 2000, _cts);
                _wasMomentBeforeOpenValueTrue = false;
            }
            if(Input2.MiddleClick&&!open&&!inventory.activeSelf) {
                _cts = new CancellationTokenSource();
                ToggleOpenHotBar(); 
                _= CustomInvoke.Invoke(()=>
                {
                    ToggleOpenHotBar();
                    if (!_cts.IsCancellationRequested) { _cts.Cancel(); }
                    
                }, 4000, _cts);
            }
            else if(Input2.MiddleClick&&open&&!inventory.activeSelf) { if (!_cts.IsCancellationRequested) { _cts.Cancel(); } _=CustomInvoke.Invoke(ToggleOpenHotBar, 100); }
            if(inventory.activeSelf&&!open) { ToggleOpenHotBar(); _justInventoryOpened = true; }
            if(!inventory.activeSelf&&open&&_justInventoryOpened){ToggleOpenHotBar();_justInventoryOpened = false;}
        }
        private void UpdateSlotsSprites(){
            for (int i = 0; i < HotBarsSlotsCount; i++)
            {
                ChangeSlotSprite(i==currentHotBarSlot ? slotSprites[1] : slotSprites[0] , slotsImages[i]);
            }
        }
        private void ChangeSlotSprite(Sprite sprite, Image slot){slot.sprite = sprite; }
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

            _=CustomInvoke.Invoke(SwapPositionsLogic,duration: (int)(!_justHotBarOpened? rateBeforeScroll*1000:0),param: num) ;
            _=CustomInvoke.Invoke(UpdateArmItem,duration: (int)(!_justHotBarOpened? rateBeforeScroll*1000+1 : 0));
            _=CustomInvoke.Invoke(UpdateSlotsSprites, (int)(!_justHotBarOpened?rateBeforeScroll*1000*2:rateBeforeScroll*1000));
            _=CustomInvoke.Invoke(ScrollTrue, (int)(rateBeforeScroll*1000+2));
            if(!_justHotBarOpened) _justHotBarOpened = true;
        }
        private void ScrollTrue()=>canScroll = true;
        private void SwapPositionsLogic(int num)
        {
            currentHotBarSlot = (currentHotBarSlot + num + HotBarsSlotsCount) % HotBarsSlotsCount;
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[0]);
            UpdateSlotsPositions();
        }
    
        private void UpdateSlotsPositions()
        {
            for (int i = 0; i < hotBarsChilds.Count; i++)
            {
                int newI = (i - currentHotBarSlot + HotBarsSlotsCount) % HotBarsSlotsCount;
                SmoothChangeValueLogic.StartSmoothPositionChange(hotBarsChildsPositions[newI], hotBarsChilds[i], CancellationTokensSourseDictionary[0], rateBeforeScroll2, speedScrolling);
            }

        }   
        private void UpdateHotBarState()
        {
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[0]);
            if (open) 
            {
                UpdateSlotsPositions();
                SmoothChangeValueLogic.StartSmoothPositionChange(_hotBarOpenPos, hotBar, CancellationTokensSourseDictionary[1], rateBeforeScroll2, speedScrolling);
                SmoothChangeValueLogic.StartSmoothRotationChange(hiderOpenRot, hotBarsHider, CancellationTokensSourseDictionary[1],speedHiderPosChanging);
            }
            else
            {
                foreach(var child in hotBarsChilds)
                {
                    SmoothChangeValueLogic.StartSmoothPositionChange(Vector3.zero, child, CancellationTokensSourseDictionary[0], rateBeforeScroll2, speedScrolling);
                }
                SmoothChangeValueLogic.StartSmoothPositionChange(_hotBarLockedPos, hotBar, CancellationTokensSourseDictionary[1], rateBeforeScroll2, speedScrolling);
                SmoothChangeValueLogic.StartSmoothRotationChange(hiderLockedRot, hotBarsHider, CancellationTokensSourseDictionary[1], speedHiderPosChanging);
            }
        }
        private void ToggleOpenHotBar()
        {
            open = !open;
            CancelAndRestartTokens.CancelAllAnimations(CancellationTokensSourseDictionary[1]);
            UpdateHotBarState();
        }

        private void UpdateArmItem()
        {
            foreach (var slot in hotBarSlots)
            {
                if (slot == hotBarSlots[currentHotBarSlot])
                {
                    //There need to be a check of what item there are, and take item logic. But I'm not realized this function yet(I'm about take item logic).
                    ItemChooseManager.instance.UpdateChosenItems(hotBarSlots.IndexOf(slot), hotBarSlots);
                }
            }
        }
    }
}