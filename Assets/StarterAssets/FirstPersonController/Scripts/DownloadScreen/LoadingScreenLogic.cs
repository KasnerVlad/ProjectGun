using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CustomDelegats;
using System.Threading;
using System.Threading.Tasks;
using SmoothAnimationLogic;
using CTSCancelLogic;
public class LoadingScreenLogic : MonoBehaviour
{
    public static Vm<float, GameObject, Dictionary<GameObject, CancellationTokenSource>, float> endloadingM;
    public static Rm<Slider> sliderReturen;
    public static Rm<float> speedReturen;
    public static LoadingScreenLogic inistate;
    [SerializeField] private Slider slider;
    [SerializeField] private float speed;
    [SerializeField] private float colorSpeed;
    [SerializeField] private List<GameObject> disturbingGameObjects;
    [SerializeField] private List<GameObject> neededObjects;
    public CancellationTokenSource cts{get;private set;}
    private Image img;
    private bool loaded;
    private void Awake()
    {
        endloadingM=SmoothChangeValueLogic.StartSmoothSliderValueChange;
        sliderReturen = GiveSlider;
        speedReturen = SpeedReturn;
        cts=new CancellationTokenSource();
        SetActiveObjectsInCollection(true, neededObjects);
        SetActiveObjectsInCollection(false, disturbingGameObjects);
    }
    private float SpeedReturn() {return speed;}
    private Slider GiveSlider() { return slider; }
    private void SetActiveObjectsInCollection(bool active, List<GameObject> objects)
    {
        foreach (var g in objects)
        {
            g.SetActive(active);
        }
    }
    private void Start()
    {
        if (inistate == null) { inistate = this; }
        else { Destroy(this); }
        slider.value = 0;
        SmoothChangeValueLogic.StartSmoothSliderValueChange(0.9f, slider.gameObject,
            CancelAndRestartTokens.GiveDictinary(new[] { slider.gameObject }, new[] { cts }), speed);
    }
    private void AlphaInCollectionChange(float targetValue, List<GameObject> objects)
    {
        foreach (var g in objects)
        {
            SmoothChangeValueLogic.StartSmoothColorAlphaChange(targetValue, g, CancelAndRestartTokens.GiveDictinary(new[] { g }, new[] {new CancellationTokenSource() }), speed);
        }
    }
    private void Update()
    {
        if (slider.value == 1f && !loaded)
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
            loaded = true;
            SetActiveObjectsInCollection(true, disturbingGameObjects);
            AlphaInCollectionChange(targetValue:0, neededObjects);
            AlphaInCollectionChange(targetValue:1, disturbingGameObjects);
        }
    }


}