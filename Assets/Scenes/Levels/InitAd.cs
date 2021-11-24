using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class InitAd : MonoBehaviour
{
    private void Awake() {
        MobileAds.Initialize(initStatus => { });
    }
}
