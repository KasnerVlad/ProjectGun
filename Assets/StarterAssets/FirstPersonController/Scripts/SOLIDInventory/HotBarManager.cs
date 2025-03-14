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
        [SerializeField] private GameObject hotBarsHider;
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
            hotBarsHider.transform.SetAsLastSibling();
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
            CancelAllAnimations();
            UpdateSlotsPositions();
        }
    
        private void UpdateSlotsPositions()
        {
            for (int i = 0; i < hotBarsChilds.Count; i++)
            {
                int newI = (i + currentHotBarSlot) % HotBarsSlotsCount;
                StartSmoothPositionChange(hotBarsChildsPositions[newI], hotBarsChilds[i]);
            }
        }
    
        private void ChangePosFor(Vector3 pos, bool open)
        {
            CancelAllAnimations();
            if (open) 
            {
                UpdateSlotsPositions();
            }
            else
            {
                foreach(var child in hotBarsChilds)
                {
                    StartSmoothPositionChange(pos, child);
                }
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
    
        private void StartSmoothPositionChange(Vector3 pos, GameObject target)
        {
            if (cancellationTokens.TryGetValue(target, out var cts))
            {
                cts.Cancel();
                cancellationTokens.Remove(target);
            }
    
            var newCts = new CancellationTokenSource();
            cancellationTokens[target] = newCts;
            
            _ = SmoothPosChanging(pos, target, newCts.Token);
        }
    
        private void CancelAllAnimations()
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
            CancelAllAnimations();
            ChangePosFor(Vector3.zero, open);
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