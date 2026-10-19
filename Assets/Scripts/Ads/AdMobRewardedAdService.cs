// Compiled only when the scripting define symbol LASTFLAG_ADMOB is set
// (Project Settings > Player > Other Settings > Scripting Define Symbols) AFTER importing the
// "Google Mobile Ads Unity Plugin" (v8 or newer). See the guide for the steps.
#if LASTFLAG_ADMOB
using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

namespace JollyPanda.LastFlag.Ads
{
    [AddComponentMenu("LastFlag/Ads/AdMob Rewarded Ad Service")]
    public class AdMobRewardedAdService : RewardedAdServiceBase
    {
        [Tooltip("Google's public TEST id. Replace it with your real rewarded ad unit id before release.")]
        [SerializeField] private string androidAdUnitId = "ca-app-pub-3940256099942544/5224354917";

        private RewardedAd ad;
        private bool initialized;
        private bool loading;
        private bool earned;
        private Action<bool> pendingFinish;

        // AdMob calls back on a background thread; everything is moved to the main thread through this queue.
        private readonly Queue<Action> mainThread = new();
        private readonly object gate = new();

        public override bool IsReady => initialized && ad != null && ad.CanShowAd();

        private void Start()
        {
            if (!IsActiveInstance)
                return;

            MobileAds.Initialize(_ => Post(() =>
            {
                initialized = true;
                Load();
            }));
        }

        private void Update()
        {
            while (true)
            {
                Action next;
                lock (gate)
                {
                    if (mainThread.Count == 0)
                        return;
                    next = mainThread.Dequeue();
                }

                next();
            }
        }

        protected override void OnDestroy()
        {
            ad?.Destroy();
            base.OnDestroy();
        }

        private void Post(Action action)
        {
            lock (gate)
            {
                mainThread.Enqueue(action);
            }
        }

        private void Load()
        {
            if (loading)
                return;

            loading = true;

            if (ad != null)
            {
                ad.Destroy();
                ad = null;
            }

            RewardedAd.Load(androidAdUnitId, new AdRequest(), (RewardedAd loaded, LoadAdError error) => Post(() =>
            {
                loading = false;

                if (error != null || loaded == null)
                {
                    Debug.LogWarning("[Ads] Rewarded ad failed to load: " + error);
                    return;
                }

                ad = loaded;
                ad.OnAdFullScreenContentClosed += () => Post(Finish);
                ad.OnAdFullScreenContentFailed += (AdError e) => Post(() =>
                {
                    earned = false;
                    Finish();
                });
            }));
        }

        public override void Show(Action<bool> onFinished)
        {
            if (!IsReady || pendingFinish != null)
            {
                onFinished?.Invoke(false);
                return;
            }

            earned = false;
            pendingFinish = onFinished;
            ad.Show(reward => Post(() => earned = true));
        }

        private void Finish()
        {
            var callback = pendingFinish;
            bool wasEarned = earned;
            pendingFinish = null;
            earned = false;

            Load(); // an ad can be shown only once: preload the next one
            callback?.Invoke(wasEarned);
        }
    }
}
#endif
