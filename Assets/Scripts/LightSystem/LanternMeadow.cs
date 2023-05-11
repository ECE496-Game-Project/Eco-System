using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanternMeadow : MonoBehaviour
{
    [SerializeField]
    private int _max;
    private int _cur;

    private RegionType _region;

    public UnityEvent<int> OnCurrentGrassChange;

    

    public int Current
    {
        get { return _cur; }
        set
        {
            _cur = value;
            if (_cur < 0)
            {
                _cur = 0;
            }else if(_cur > _max) 
            {
                _cur = _max;
            }
            OnCurrentGrassChange?.Invoke(_cur);


        }
    }
    [SerializeField]
    [Tooltip("how long does it take decrease the number of 灯笼草 by 1")]
    private float _dyingRate;
    private Coroutine _dyingCoroutine;

    public void Awake()
    {
        LightRegion lightRegion = GetComponent<LightRegion>();
        lightRegion.OnRegionTypeChange.AddListener(OnRegionChange);
    }

    public void OnRegionChange(RegionType type)
    {
        _region = type;
        if (type == RegionType.Bright)
        {
            if (_dyingCoroutine != null) StopCoroutine(_dyingCoroutine);
            Current = _max;
        }
        else
        {
            _dyingCoroutine = StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {

        while (Current > 0)
        {
            yield return new WaitForSeconds(_dyingRate);
            Current--;
        }

        _dyingCoroutine = null;
    }

    public bool Eaten(int amount)
    {
        if (Current < amount)  return false;
        Current -= amount;

        if (_region == RegionType.Bright) Current = _max;
        return true;
    }
}
