#if UNITY_IPHONE
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlistMod : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 999999999;  }}

    public List<List<string>> skAdnetworkList = new List<List<string>>();

    public void OnPostprocessBuild(BuildReport report)
    {
        /// UNITY
        skAdnetworkList.Add(new List<string>()
        {
            "v79kvwwj4g.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "578prtvx9j.skadnetwork",
            "f38h382jlk.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "294l99pt4k.skadnetwork",
            "3qy4746246.skadnetwork",
            "k674qkevps.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "f7s53z58qe.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "t38b2kh725.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "mp6xlyr22a.skadnetwork",
            "v72qych5uu.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "238da6jt44.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "4468km3ulz.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "zq492l623r.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "lr83yxwka7.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "424m5254lk.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "w9q455wk68.skadnetwork",
            "44jx6755aq.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "a2p9lx4jpn.skadnetwork"
        });

        /// Pubnative
        skAdnetworkList.Add(new List<string>()
        {
            "p78axxw29g.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "v72qych5uu.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "g28c52eehv.skadnetwork",
            "523jb4fst2.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "f38h382jlk.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "4468km3ulz.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "424m5254lk.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "9b89h5y424.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "kbmxgpxpgc.skadnetwork",
            "275upjj5gd.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "qqp299437r.skadnetwork",
            "294l99pt4k.skadnetwork",
            "e5fvkxwrpn.skadnetwork",
            "x5l83yy675.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "74b6s63p6l.skadnetwork",
            "ln5gz23vtd.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "6v7lgmsu45.skadnetwork",
            "z959bm4gru.skadnetwork"
        });

        /// Bigo Ads
        skAdnetworkList.Add(new List<string>()
        {
            "22mmun2rn5.skadnetwork",
            "2U9PT9HC89.skadnetwork",
            "3qy4746246.skadnetwork",
            "3RD42EKR43.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "4468km3ulz.skadnetwork",
            "44jx6755aq.skadnetwork",
            "4FZDC2EVR5.skadnetwork",
            "523jb4fst2.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "737z793b9f.skadnetwork",
            "7953JERFZD.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "7UG5ZH24HU.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "97r2b46745.skadnetwork",
            "9RD848Q2BZ.skadnetwork",
            "9T245VHMPL.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "bvpn9ufa9b.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "cg4yq2srnc.skadnetwork",
            "CJ5566H2GA.skadnetwork",
            "F38H382JLK.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "gvmwg8q7h5.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "KLF5C3L5U5.skadnetwork",
            "M8DBW4SV7C.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "mls7yz5dvl.skadnetwork",
            "mtkv5xtk9e.skadnetwork",
            "n66cz3y3bx.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "nzq8sh4pbs.skadnetwork",
            "p78axxw29g.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "pu4na253f3.skadnetwork",
            "t38b2kh725.skadnetwork",
            "u679fj5vs4.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "v72qych5uu.skadnetwork",
            "W9Q455WK68.skadnetwork",
            "WZMMZ9FP6W.skadnetwork",
            "XY9T38CT57.skadnetwork",
            "YCLNXRL5PM.skadnetwork",
            "z4gj7hsk7h.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "4PFYVQ9L8R.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "578prtvx9j.skadnetwork",
            "8m87ys6875.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "TL55SBB4FM.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "a8cz6cu7e5.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "424m5254lk.skadnetwork",
            "BD757YWX3.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "33r6p7g8nc.skadnetwork",
            "g28c52eehv.skadnetwork",
            "52fl2v3hgk.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "6g9af3uyq4.skadnetwork",
            "275upjj5gd.skadnetwork",
            "rx5hdcabgc.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "2fnua5tdw4.skadnetwork",
            "g69uk9uh2b.skadnetwork",
            "zq492l623r.skadnetwork",
            "9b89h5y424.skadnetwork",
            "bxvub5ada5.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "f38h382jlk.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "kbmxgpxpgc.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "qqp299437r.skadnetwork",
            "294l99pt4k.skadnetwork",
            "74b6s63p6l.skadnetwork",
            "b9bk5wbcq9.skadnetwork",
            "V72QYCH5UU.skadnetwork",
            "44n7hlldy6.skadnetwork",
            "6p4ks3rnbw.skadnetwork",
            "g2y4y55b64.skadnetwork"
        });

        /// MyTarget
        skAdnetworkList.Add(new List<string>()
        {
            "n9x2a789qt.skadnetwork"
        });

        /// Ironsource ALL NETWORK
        skAdnetworkList.Add(new List<string>()
        {
            "su67r6k2v3.skadnetwork",
            "f7s53z58qe.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "v72qych5uu.skadnetwork",
            "44jx6755aq.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "t38b2kh725.skadnetwork",
            "f38h382jlk.skadnetwork",
            "424m5254lk.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "cp8zw746q7.skadnetwork",
            "4468km3ulz.skadnetwork",
            "e5fvkxwrpn.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "3qy4746246.skadnetwork",
            "k674qkevps.skadnetwork",
            "kbmxgpxpgc.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "252b5q8x7y.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "7fmhfwg9en.skadnetwork",
            "6yxyv74ff7.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "mqn7fxpca7.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "klf5c3l5u5.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "mtkv5xtk9e.skadnetwork",
            "6g9af3uyq4.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "u679fj5vs4.skadnetwork",
            "rx5hdcabgc.skadnetwork",
            "g28c52eehv.skadnetwork",
            "cg4yq2srnc.skadnetwork",
            "275upjj5gd.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "qqp299437r.skadnetwork",
            "294l99pt4k.skadnetwork",
            "2fnua5tdw4.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "523jb4fst2.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "r45fhb6rf7.skadnetwork",
            "g2y4y55b64.skadnetwork",
            "n6fk4nfna4.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "74b6s63p6l.skadnetwork",
            "44n7hlldy6.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "84993kbrcf.skadnetwork",
            "pwdxu55a5a.skadnetwork",
            "6964rsfnh4.skadnetwork",
            "a7xqa6mtl2.skadnetwork",
            "c3frkrj4fj.skadnetwork",
            "3qcr597p9d.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "p78axxw29g.skadnetwork",
            "zq492l623r.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "578prtvx9j.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "9b89h5y424.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "ludvb6z3bs.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "6p4ks3rnbw.skadnetwork",
            "737z793b9f.skadnetwork",
            "97r2b46745.skadnetwork",
            "b9bk5wbcq9.skadnetwork",
            "bxvub5ada5.skadnetwork",
            "dzg6xy7pwj.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "hdw39hrw9y.skadnetwork",
            "lr83yxwka7.skadnetwork",
            "mls7yz5dvl.skadnetwork",
            "mp6xlyr22a.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "s69wq72ugq.skadnetwork",
            "w9q455wk68.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "x8uqf25wch.skadnetwork",
            "y45688jllp.skadnetwork",
            "238da6jt44.skadnetwork",
            "x2jnk7ly8j.skadnetwork",
            "v9wttpbfk9.skadnetwork",
            "n38lu8286q.skadnetwork",
            "9g2aggbj52.skadnetwork",
            "wzmmZ9fp6w.skadnetwork",
            "nu4557a4je.skadnetwork",
            "7953jerfzd.skadnetwork",
            "9yg77x724h.skadnetwork",
            "n66cz3y3bx.skadnetwork",
            "v4nxqhlyqp.skadnetwork",
            "24zw6aqk47.skadnetwork",
            "cs644xg564.skadnetwork",
            "9vvzujtq5s.skadnetwork",
            "a8cz6cu7e5.skadnetwork",
            "r26jy69rpl.skadnetwork",
            "dbu4b84rxf.skadnetwork",
            "3l6bd9hu43.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "52fl2v3hgk.skadnetwork",
            "6v7lgmsu45.skadnetwork",
            "89z7zv988g.skadnetwork",
            "8m87ys6875.skadnetwork",
            "hb56zgv37p.skadnetwork",
            "m297p6643m.skadnetwork",
            "m5mvw97r93.skadnetwork",
            "vcra2ehyfk.skadnetwork",
            "ecpz2srf59.skadnetwork",
            "gvmwg8q7h5.skadnetwork",
            "nzq8sh4pbs.skadnetwork",
            "pu4na253f3.skadnetwork",
            "v79kvwwj4g.skadnetwork",
            "yrqqpx2mcb.skadnetwork",
            "z4gj7hsk7h.skadnetwork"
        });

        /// Mintegral
        skAdnetworkList.Add(new List<string>()
        {
            "kbd757ywx3.skadnetwork",
            "mls7yz5dvl.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "cg4yq2srnc.skadnetwork",
            "p78axxw29g.skadnetwork",
            "737z793b9f.skadnetwork",
            "v72qych5uu.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "ludvb6z3bs.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "523jb4fst2.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "f38h382jlk.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "w9q455wk68.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "4468km3ulz.skadnetwork",
            "t38b2kh725.skadnetwork",
            "k674qkevps.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "44jx6755aq.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "klf5c3l5u5.skadnetwork",
            "dzg6xy7pwj.skadnetwork",
            "y45688jllp.skadnetwork",
            "hdw39hrw9y.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "424m5254lk.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "mtkv5xtk9e.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "g28c52eehv.skadnetwork",
            "su67r6k2v3.skadnetwork",
            "rx5hdcabgc.skadnetwork",
            "2fnua5tdw4.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "9b89h5y424.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "kbmxgpxpgc.skadnetwork",
            "275upjj5gd.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "qqp299437r.skadnetwork",
            "294l99pt4k.skadnetwork",
            "74b6s63p6l.skadnetwork",
            "44n7hlldy6.skadnetwork",
            "6p4ks3rnbw.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "e5fvkxwrpn.skadnetwork",
            "97r2b46745.skadnetwork",
            "3qcr597p9d.skadnetwork",
            "578prtvx9j.skadnetwork",
            "n6fk4nfna4.skadnetwork",
            "b9bk5wbcq9.skadnetwork",
            "84993kbrcf.skadnetwork",
            "24zw6aqk47.skadnetwork",
            "pwdxu55a5a.skadnetwork",
            "cs644xg564.skadnetwork",
            "6964rsfnh4.skadnetwork",
            "9vvzujtq5s.skadnetwork",
            "a7xqa6mtl2.skadnetwork",
            "r45fhb6rf7.skadnetwork",
            "c3frkrj4fj.skadnetwork",
            "6g9af3uyq4.skadnetwork",
            "u679fj5vs4.skadnetwork",
            "V72QYCH5UU.skadnetwork",
            "g2y4y55b64.skadnetwork",
            "zq492l623r.skadnetwork",
            "a8cz6cu7e5.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "dbu4b84rxf.skadnetwork"
        });

        /// Admob
        skAdnetworkList.Add(new List<string>()
        {
            "cstr6suwn9.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "2fnua5tdw4.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "p78axxw29g.skadnetwork",
            "v72qych5uu.skadnetwork",
            "ludvb6z3bs.skadnetwork",
            "cp8zw746q7.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "3qy4746246.skadnetwork",
            "f38h382jlk.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "v4nxqhlyqp.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "t38b2kh725.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "vutu7akeur.skadnetwork",
            "y5ghdn5j9k.skadnetwork",
            "n6fk4nfna4.skadnetwork",
            "v9wttpbfk9.skadnetwork",
            "n38lu8286q.skadnetwork",
            "47vhws6wlr.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "eh6m2bh4zr.skadnetwork",
            "a2p9lx4jpn.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "4468km3ulz.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "klf5c3l5u5.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "ecpz2srf59.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "578prtvx9j.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "e5fvkxwrpn.skadnetwork",
            "8c4e2ghe7u.skadnetwork",
            "zq492l623r.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "3qcr597p9d.skadnetwork"
        });

        /// Default
        skAdnetworkList.Add(new List<string>()
        {
            "p78axxw29g.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "6g9af3uyq4.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "vcra2ehyfk.skadnetwork",
            "79w64w269u.skadnetwork",
            "cg4yq2srnc.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "d7g9azk84q.skadnetwork",
            "nzq8sh4pbs.skadnetwork",
            "2q884k2j68.skadnetwork",
            "v72qych5uu.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "ludvb6z3bs.skadnetwork",
            "pd25vrrwzn.skadnetwork",
            "lr83yxwka7.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "g28c52eehv.skadnetwork",
            "3qy4746246.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "252b5q8x7y.skadnetwork",
            "67369282zy.skadnetwork",
            "899vrgt9g8.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "f38h382jlk.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "mp6xlyr22a.skadnetwork",
            "88k8774x49.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "c7g47wypnu.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "52fl2v3hgk.skadnetwork",
            "9vvzujtq5s.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "dr774724x4.skadnetwork",
            "v9wttpbfk9.skadnetwork",
            "n38lu8286q.skadnetwork",
            "t7ky8fmwkd.skadnetwork",
            "fz2k2k5tej.skadnetwork",
            "u679fj5vs4.skadnetwork",
            "w28pnjg2k4.skadnetwork",
            "9g2aggbj52.skadnetwork",
            "vc83br9sjg.skadnetwork",
            "w9q455wk68.skadnetwork",
            "nu4557a4je.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "su67r6k2v3.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "h8vml93bkz.skadnetwork",
            "uzqba5354d.skadnetwork",
            "4468km3ulz.skadnetwork",
            "v79kvwwj4g.skadnetwork",
            "au67k4efj4.skadnetwork",
            "t38b2kh725.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "rx5hdcabgc.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "v4nxqhlyqp.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "v7896pgt74.skadnetwork",
            "5ghnmfs3dh.skadnetwork",
            "275upjj5gd.skadnetwork",
            "627r9wr2y5.skadnetwork",
            "hb56zgv37p.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "cad8qz2s3j.skadnetwork",
            "eh6m2bh4zr.skadnetwork",
            "97r2b46745.skadnetwork",
            "238da6jt44.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "44jx6755aq.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "2tdux39lx8.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "klf5c3l5u5.skadnetwork",
            "hdw39hrw9y.skadnetwork",
            "y45688jllp.skadnetwork",
            "dzg6xy7pwj.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "9wsyqb3ku7.skadnetwork",
            "424m5254lk.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "44n7hlldy6.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "ecpz2srf59.skadnetwork",
            "f7s53z58qe.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "6964rsfnh4.skadnetwork",
            "gvmwg8q7h5.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "578prtvx9j.skadnetwork",
            "bvpn9ufa9b.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "mtkv5xtk9e.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "r45fhb6rf7.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "5mv394q32t.skadnetwork",
            "2fnua5tdw4.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "n6fk4nfna4.skadnetwork",
            "e5fvkxwrpn.skadnetwork",
            "zq492l623r.skadnetwork",
            "3qcr597p9d.skadnetwork",
            "hjevpa356n.skadnetwork",
            "k674qkevps.skadnetwork",
            "y2ed4ez56y.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "523jb4fst2.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "9b89h5y424.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "737z793b9f.skadnetwork",
            "mls7yz5dvl.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "r26jy69rpl.skadnetwork",
            "8m87ys6875.skadnetwork",
            "9yg77x724h.skadnetwork",
            "n66cz3y3bx.skadnetwork",
            "pu4na253f3.skadnetwork",
            "yrqqpx2mcb.skadnetwork",
            "z4gj7hsk7h.skadnetwork",
            "7953jerfzd.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "8c4e2ghe7u.skadnetwork",
            "a2p9lx4jpn.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "zq492l623r.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "w9q455wk68.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "4468km3ulz.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "ppxm28t8ap.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "f38h382jlk.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "lr83yxwka7.skadnetwork",
            "v72qych5uu.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "v79kvwwj4g.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "3qy4746246.skadnetwork",
            "k674qkevps.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "44jx6755aq.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "238da6jt44.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "424m5254lk.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "t38b2kh725.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "f7s53z58qe.skadnetwork",
            "294l99pt4k.skadnetwork",
            "mp6xlyr22a.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "578prtvx9j.skadnetwork"
        });

        // Read plist
        var plistPath = Path.Combine(report.summary.outputPath, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Update value
        PlistElementDict rootDict = plist.root;
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSCalendarsUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSLocationAlwaysUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSLocationWhenInUseUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["FacebookAutoLogAppEventsEnabled"].AsBoolean());

        rootDict.SetString("NSCalendarsUsageDescription", "This app needs to access your calendar");
        rootDict.SetString("NSLocationAlwaysUsageDescription", "This app needs to access your location");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "This app needs to access your location");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "This app needs to access your photos for taking selfies");
        rootDict.SetString("NSCameraUsageDescription", "This app needs to access your camera for taking selfies");
        rootDict.SetString("NSMotionUsageDescription ", "This app needs to access motion usage for interactive ads controls");
        rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://appsflyer-skadnetwork.com/");
        rootDict.SetBoolean("FacebookAutoLogAppEventsEnabled", true);
        //rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-3078899387421921~3060732326");
        // rootDict.SetBoolean("NSAllowsArbitraryLoads", true);
        //rootDict.SetBoolean("NSAllowsArbitraryLoadsForMedia", true);
        //rootDict.SetBoolean("NSAllowsArbitraryLoadsInWebContent", true);

        if (rootDict["NSAppTransportSecurity"] == null)
        {
            rootDict.CreateDict("NSAppTransportSecurity");
        }

        if(rootDict["NSAppTransportSecurity"].AsDict()["NSAllowsArbitraryLoadsInWebContent"] != null)
        {
            rootDict["NSAppTransportSecurity"].AsDict()["NSAllowsArbitraryLoadsInWebContent"] = null;
        }

		// Set encryption usage boolean
		string encryptKey = "ITSAppUsesNonExemptEncryption";
			rootDict.SetBoolean(encryptKey, false);

		// remove exit on suspend if it exists.
		string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
		if(rootDict.values.ContainsKey(exitsOnSuspendKey))
		{
			rootDict.values.Remove(exitsOnSuspendKey);
		}

		//LSApplicationQueriesSchemes AdColony
		if (rootDict["LSApplicationQueriesSchemes"] == null)
        {
            PlistElementArray array = rootDict.CreateArray("LSApplicationQueriesSchemes");
			array.AddString("fb");
            array.AddString("fb-messenger");
			array.AddString("instagram");
			array.AddString("tumblr");
			array.AddString("twitter");
        }

        // iOS 14 for Ironsource Mediation
        rootDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");
		

        PlistElementArray SKAdNetworkItems = null;
        if (rootDict.values.ContainsKey("SKAdNetworkItems"))
        {
            try
            {
                SKAdNetworkItems = rootDict.values["SKAdNetworkItems"] as PlistElementArray;
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("Could not obtain SKAdNetworkItems PlistElementArray: {0}", e.Message));
            }
        }

        if (SKAdNetworkItems == null)
        {
            SKAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
        }

        string plistContent = File.ReadAllText(plistPath);
        List<string> finalSKAdnetwork = new List<string>();
        foreach(List<string> list in skAdnetworkList)
        {
            foreach(string thisString in list)
            {
                if(!finalSKAdnetwork.Contains(thisString.ToLower()))
                {
                    finalSKAdnetwork.Add(thisString.ToLower());
                }
            }
        }
        foreach (string item in finalSKAdnetwork)
        {
            if(item != "" && !plistContent.Contains(item))
            {
                PlistElementDict newDict = SKAdNetworkItems.AddDict();
                newDict.SetString("SKAdNetworkIdentifier", item);
            }
        }

        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSCalendarsUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSLocationAlwaysUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSLocationWhenInUseUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["FacebookAutoLogAppEventsEnabled"].AsBoolean());

        // Write plist
        File.WriteAllText(plistPath, plist.WriteToString());

        //plist.ReadFromFile(plistPath);
        //rootDict = plist.root;

        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSCalendarsUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSLocationAlwaysUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["NSLocationWhenInUseUsageDescription"].AsString());
        //Debug.Log("PHONG - OnPostprocessBuild " + rootDict["FacebookAutoLogAppEventsEnabled"].AsBoolean());

        //Debug.Log("PHONG - OnPostprocessBuild ");
    }
}
#endif