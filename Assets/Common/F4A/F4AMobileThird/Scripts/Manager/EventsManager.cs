namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EventsManager : SingletonMono<EventsManager>
    {
        public void LogEvent(string nameEvent, Dictionary<string, string> events)
        {
            FirebaseManager.Instance.LogEvent(nameEvent, events);
            AppsFlyerManager.Instance.EventCustom(nameEvent, events);
        }
    }
}