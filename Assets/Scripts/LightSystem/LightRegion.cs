using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RegionType
{
    Bright, Dark
}
public class LightRegion: MonoBehaviour
{
    [SerializeField]
    public static RegionType DefaultRegionType { get; set; }

    [SerializeField]
    private RegionType _regionType;
	public RegionType LightRegionType
    {
        get { return _regionType; } 
		set
        {
            _regionType = value;
            ChangeColor();
            OnRegionTypeChange?.Invoke(_regionType);
        }
    }

    public UnityEvent<RegionType> OnRegionTypeChange;


    private SpriteRenderer _spriteRenderer;

   
    private void Start()
    {
        
        _spriteRenderer = GetComponent<SpriteRenderer>();

        LightRegionType = _regionType;
    }

    private void ChangeColor ()
    {
        if (_regionType == RegionType.Bright)
        {
            // change to yellow color
            _spriteRenderer.color = new Color(1f, 0.8855017f, 0, 0.4980392f);
        }
        else
        {
            // change to gray color
            _spriteRenderer.color = new Color(0.6981132f, 0.6981132f, 0.6981132f, 0.7450981f);
        }
        
    }

}