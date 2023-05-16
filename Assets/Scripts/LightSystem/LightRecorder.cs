using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightRecorder : MonoBehaviour
{
    //singleton
    public static LightRecorder Instance { get; private set; }


    private Dictionary<int, RegionType> _regionStatus;

    private Dictionary<int, Transform> _regionTransform;

    private HashSet<int> _brightRegion;

    private HashSet<int> _darkRegion;

    public int BrightRegionsCount { get { return _brightRegion.Count; } }
    public int DarkRegionCount { get { return _darkRegion.Count; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Use this for initialization
    void Start()
    {
        _brightRegion = new HashSet<int>();
        _darkRegion = new HashSet<int>();
        _regionStatus = new Dictionary<int, RegionType>();
        _regionTransform = new Dictionary<int, Transform>();
        
        RegionLayerManager[] regions = GameObject.FindObjectsOfType<RegionLayerManager>();

        foreach (var region in regions)
        {
            int id = region.gameObject.GetInstanceID();
            if (region.Type == RegionType.Bright)
            {
                _brightRegion.Add(id);
            }
            else if (region.Type == RegionType.Dark)
            {
                _darkRegion.Add(id);
            }
            _regionStatus.Add(id, region.Type);
            _regionTransform.Add(id, region.transform);
            region.OnRegionTypeChanged.AddListener(OnRegionChanged);
        }
    }

    private void OnRegionChanged(RegionType type, Transform t)
    {
        int id = t.gameObject.GetInstanceID();

        if (!_regionStatus.ContainsKey(id)) return;

        var previousType = _regionStatus[id];
        if (previousType == RegionType.Bright) _brightRegion.Remove(id);
        else if (previousType == RegionType.Dark) _darkRegion.Remove(id);

        if (type == RegionType.Bright) _brightRegion.Add(id);
        else if (type == RegionType.Dark) _darkRegion.Add(id);

        _regionStatus[id] = type;
        Test();
    }

    public Transform GetClosestBrightRegion(Transform t)
    {
        if (t == null) return null;

        int result = -1;
        float minDis = float.PositiveInfinity;

        foreach (var region in _brightRegion)
        {
            var location = region;

            float dis = Vector3.Distance(_regionTransform[region].position, t.position);

            if (dis < minDis)
            {
                result = region;
                minDis = dis;
            }
        }

        if (result == -1) return null;
        else return _regionTransform[result];
    }
    
    private void Test()
    {
        Debug.Log("Region Status");
        foreach (var region in _regionStatus)
        {
            
            Debug.Log($"{region.Key}: {region.Value}");
        }

        Debug.Log("Bright Region");
        foreach (var region in _brightRegion)
        {
            Debug.Log(_regionTransform[region].name);
        }

        Debug.Log("Dark Region");
        foreach (var region in _darkRegion)
        {
            Debug.Log(_regionTransform[region].name);
        }

        var closest = GetClosestBrightRegion(transform);
        Debug.Log($"The closest one is {closest.transform.name}");
    }
}
