using System.Collections.Generic;
using UnityEngine;
using Warudo.Core.Attributes;
using Warudo.Core.Graphs;
using Warudo.Core.Scenes;
using Warudo.Plugins.Core.Assets;

namespace Rikia
{
    [NodeType(Id = "69e9c383-04ed-47c8-b6be-aacfec8977dd", Title = "Get VRCFT Receiver Data", Category = "CATEGORY_MOTION_CAPTURE")]
    public class GetVRCFTTrackerDataNode : Node
    {
        [DataInput(13)]
        [Label("RECEIVER")]
        public VRCFTReceiverAsset Receiver;
        public bool IsTracked()
        {
            VRCFTReceiverAsset receiver = this.Receiver;
            return receiver != null && receiver.IsTracked;
        }
        public Dictionary<string, float> BlendShapes() => this.Receiver?.LatestBlendShapes;
    }
}