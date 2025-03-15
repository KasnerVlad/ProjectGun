using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private Dictionary<GameObject, CancellationTokenSource> cancellationTokens = new Dictionary<GameObject, CancellationTokenSource>();
        private Dictionary<GameObject, CancellationTokenSource> hotbarCancellations = new Dictionary<GameObject, CancellationTokenSource>();
        [SerializeField] private GameObject hotBarsHider;
        [SerializeField] private Vector3 _hotBarLockedPos;
        [SerializeField] private Vector3 _hotBarOpenPos;
        [SerializeField] private Quaternion hiderLockedRot;
        [SerializeField] private Quaternion hiderOpenRot;
        [SerializeField] private float speedHiderPosChanging;
        private Action changePos;
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
            changePos = ChangePosFor;
        }
        private void Update()
        {
            if (open)
            {
                if (InventoryInput.Scroll * scrollSensitivity > minScrollStep&& canScroll)
                {
                    canScroll = false;
                    SwapPositionsLogic(1);
                    UpdateArmItem(1);
                    Invoke(nameof(ScrollTrue), rateBeforeScroll);
                }
                else if (InventoryInput.Scroll * scrollSensitivity < minScrollStep*-1&&canScroll)
                {
                    canScroll = false;
                    SwapPositionsLogic(-1);
                    UpdateArmItem(-1);
                    Invoke(nameof(ScrollTrue), rateBeforeScroll);
                }
            }
            if(Input.GetKeyDown(KeyCode.H)){ToggleOpenHotBar();}
        }

        private void ScrollTrue()
        {
            canScroll = true;
            Debug.Log("Scroll True");
        }
        private void SwapPositionsLogic(int num)
        {
            currentHotBarSlot = (currentHotBarSlot - num + HotBarsSlotsCount) % HotBarsSlotsCount;
            CancelAllAnimations(cancellationTokens);
            UpdateSlotsPositions();
        }
    
        private void UpdateSlotsPositions()
        {
            for (int i = 0; i < hotBarsChilds.Count; i++)
            {
                int newI = (i + currentHotBarSlot) % HotBarsSlotsCount;
                StartSmoothPositionChange(hotBarsChildsPositions[newI], hotBarsChilds[i], cancellationTokens);
            }
        }
        private void ChangePosFor()
        {
            CancelAllAnimations(cancellationTokens);
            if (open) 
            {
                UpdateSlotsPositions();
                StartSmoothPositionChange(_hotBarOpenPos, hotBar, hotbarCancellations);
                StartSmoothRotationChange(hiderOpenRot, hotBarsHider, hotbarCancellations);
            }
            else
            {
                foreach(var child in hotBarsChilds)
                {
                    StartSmoothPositionChange(Vector3.zero, child, cancellationTokens);
                }
                StartSmoothPositionChange(_hotBarLockedPos, hotBar, hotbarCancellations);
                StartSmoothRotationChange(hiderLockedRot, hotBarsHider, hotbarCancellations);
            }
        }
        private async Task SmoothRotChanging(Quaternion pos, GameObject target, CancellationToken cancellationToken)
        {
            while (target.transform.localRotation != pos && 
                   !cancellationToken.IsCancellationRequested && 
                   Application.isPlaying)
            {
                target.transform.localRotation = Quaternion.Lerp(
                    target.transform.localRotation, 
                    pos, 
                    speedHiderPosChanging
                );
                await Task.Yield();
            }
        }

        private async Task SmoothPosChanging(Vector3 pos, GameObject target, CancellationToken cancellationToken)
        {
            while (target.transform.localPosition != pos && 
                  !cancellationToken.IsCancellationRequested && 
                  Application.isPlaying)
            {
                target.transform.localPosition = Vector3.MoveTowards(
                    target.transform.localPosition, 
                    pos, 
                    rateBeforeScroll * speedScrolling * 60
                );
                await Task.Yield();
            }
        }
        private void StartSmoothPositionChange(Vector3 pos, GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            _ = SmoothPosChanging(pos, target, GetCancellationToken(target, cancellationTokens).Token);
        }
        private void StartSmoothRotationChange(Quaternion pos, GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            _ = SmoothRotChanging(pos, target, GetCancellationToken(target, cancellationTokens).Token);
        }
        private CancellationTokenSource GetCancellationToken(GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
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
        private void CancelAllAnimations(Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            foreach (var cts in cancellationTokens.Values)
            {
                cts.Cancel();
            }
            cancellationTokens.Clear();

        }
    
        private void ToggleOpenHotBar()
        {
            open = !open;
            CancelAllAnimations(hotbarCancellations);
            ChangePosFor();
            Debug.Log($"open : {open}");
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