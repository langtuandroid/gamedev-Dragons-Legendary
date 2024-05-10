using System;
using UnityEngine;

namespace BlendModes.TestMode
{
    public class TestMode : MonoBehaviour
    {
        private void Awake()
        {
            Protocol_Set.Protocol_shop_ad_energy_Req(5, EnergyResponce);
            //Protocol_Set.Protocol_game_chapter_collect_Req(1, 1, OnRewardCollectComplete);
            //Debug.Log(GameInfo.userData.huntersUseInfo.Length);
            
        }
        
        private void EnergyResponce()
        {
            UnityEngine.Debug.Log("GetEnergy TEST Complete !!");
        }
        
        private void OnRewardCollectComplete()
        {
            UnityEngine.Debug.Log("GetJewel TEST Complete !!");
        }
    }
}