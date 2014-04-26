﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class TimeWarpFixer : MonoBehaviour
    {
        public double lastTime = 0;
        public double currentTime = 0;
        public static bool fixedTimeWarp = false;

        public void Start()
        {
            fixedTimeWarp = false;
        }

        public void Update()
        {
            if (ScaledSpace.Instance == null)
                return;

            // Fix Timewarp
            if (!fixedTimeWarp && TimeWarp.fetch != null)
            {
                fixedTimeWarp = true;
                ConfigNode twNode = null;
                foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS"))
                    twNode = node.GetNode("timeWarpRates");
                float ftmp;
                if (twNode != null)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (twNode.HasValue("rate" + i))
                            if (float.TryParse(twNode.GetValue("rate" + i), out ftmp))
                                TimeWarp.fetch.warpRates[i] = ftmp;
                    }
                }
            }
        }
    }
}
