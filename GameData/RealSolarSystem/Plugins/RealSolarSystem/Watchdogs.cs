using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
    // Checks to make sure useLegacyAtmosphere didn't get munged with
    // Could become a general place to prevent RSS changes from being reverted when our back is turned.
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class RSSWatchDog : MonoBehaviour
    {
        CelestialBody body = FlightGlobals.getMainBody();

        public void Start()
        {
            ConfigNode RSSSettings = null;
            bool UseLegacyAtmosphere;
            float ftmp;

            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEM"))
                RSSSettings = node;

            if (RSSSettings != null)
            {
                if (RSSSettings.HasNode(body.GetName()))
                {
                    ConfigNode node = RSSSettings.GetNode(body.GetName());
                    print("*RSS* checking useLegacyAtmosphere for " + body.GetName());
                    if (node.HasValue("useLegacyAtmosphere"))
                    {
                        bool.TryParse(node.GetValue("useLegacyAtmosphere"), out UseLegacyAtmosphere);
                        print("*RSSWatchDog* " + body.GetName() + ".useLegacyAtmosphere = " + body.useLegacyAtmosphere.ToString());
                        if (UseLegacyAtmosphere != body.useLegacyAtmosphere)
                        {
                            print("*RSSWatchDog* resetting useLegacyAtmosphere to " + UseLegacyAtmosphere.ToString());
                            body.useLegacyAtmosphere = UseLegacyAtmosphere;
                        }
                    }
                    if (node.HasNode("AtmosphereFromGround"))
                    {
                        ConfigNode agnode = node.GetNode("AtmosphereFromGround");
                        foreach (AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                        {
                            if (ag != null && ag.planet != null)
                            {
                                // generalized version of Starwaster's code. Thanks Starwaster!
                                if (ag.planet.name.Equals(node.name))
                                {
                                    if (agnode.HasValue("outerRadius"))
                                    {
                                        if (float.TryParse(agnode.GetValue("outerRadius"), out ftmp))
                                            ag.outerRadius = ftmp * ScaledSpace.InverseScaleFactor;
                                    }
                                    ag.outerRadius2 = ag.outerRadius * ag.outerRadius;
                                    ag.innerRadius2 = ag.innerRadius * ag.innerRadius;
                                    ag.scale = 1f / (ag.outerRadius - ag.innerRadius);
                                    ag.scaleDepth = -0.25f;
                                    ag.scaleOverScaleDepth = ag.scale / ag.scaleDepth;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
