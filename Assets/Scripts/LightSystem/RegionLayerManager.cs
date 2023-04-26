using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionLayerManager : MonoBehaviour
{
    private LightRegion _region;
    private LanternMeadow _meadow;

    private RegionType _regionType;
    private int _cur;

    private void Start()
    {
        _region = GetComponent<LightRegion>();
        _meadow = GetComponent<LanternMeadow>();
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
        
        if (_cur > 0 || _regionType == RegionType.Bright)
        {

            gameObject.layer = LayerMask.NameToLayer("BrightRegion");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("DarkRegion");
        }
        
    }
}
