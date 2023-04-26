using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RegionController : MonoBehaviour
{
    private static RegionController _instance;
    public static RegionController Instance { get => _instance; }

    private List<LightRegion> _regionList;

    [Tooltip("The duration between each time the region changes")]
    [SerializeField]
    private float _regionChangeTime;


    public void Start()
    {
        // set up singleton 
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        // find all the light region
        var list = GameObject.FindObjectsOfType<LightRegion>();
        _regionList = new List<LightRegion>(list);


        StartCoroutine(ChangeRegion());
    }

    private IEnumerator ChangeRegion()
    {
		
		while (true){
            yield return new WaitForSeconds(_regionChangeTime);
            ChangeRegionLogic();
			
		}
		
	}
	
	private void ChangeRegionLogic()
    {
        foreach(LightRegion region in _regionList)
        {
            if (region.LightRegionType == RegionType.Bright)
            {
                region.LightRegionType = RegionType.Dark;
            }
            else
            {
                region.LightRegionType = RegionType.Bright;
            }
        }
    }
}