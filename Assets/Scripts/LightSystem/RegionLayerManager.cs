using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RegionLayerManager : MonoBehaviour
{
    private LightRegion _region;
    private LanternMeadow _meadow;


    private RegionType _regionType;
    private Bounds _regionBound;
    private int _cur;

    public UnityEvent<RegionType, Transform> OnRegionTypeChanged;
    private void Awake()
    {
        //find the rigidBody
        _regionBound = GetComponent<Collider2D>().bounds;
        

        _region = GetComponent<LightRegion>();
        _meadow = GetComponent<LanternMeadow>();
        
    }
    private void Start()
    {
        _regionType = _region.LightRegionType;
        _cur = _meadow.Current;
        ChangeLayer();
        
        _region.OnRegionTypeChange.AddListener(OnRegionChange);
        _meadow.OnCurrentGrassChange.AddListener(OnCurrentGrassChange); 
    }

    public void OnRegionChange(RegionType newRegion)
    {
        if (_regionType == newRegion) return;
        _regionType = newRegion;
        ChangeLayer();
    }

    public void OnCurrentGrassChange(int amount)
    {
        if (_cur == amount) return;
        _cur = amount;
        ChangeLayer();
    }

    private void ChangeLayer()
    {
        
        var previousLayer = gameObject.layer;

        RegionType newType;
        if (_cur > 0 || _regionType == RegionType.Bright)
        {

            gameObject.layer = LayerMask.NameToLayer("BrightRegion");
            newType = RegionType.Bright;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("DarkRegion");
            newType= RegionType.Dark;
        }
        
        if (gameObject.layer != previousLayer)
        {
            OnRegionTypeChanged?.Invoke(newType, transform);
            AstarPath.active.UpdateGraphs(new GraphUpdateObject(_regionBound));
        }
    }
}
