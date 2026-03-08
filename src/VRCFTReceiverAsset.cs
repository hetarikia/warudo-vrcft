using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using Warudo.Core;
using Warudo.Core.Attributes;
using Warudo.Core.Localization;
using Warudo.Core.Utils;
using Warudo.Plugins.Core;
using Warudo.Plugins.Core.Assets.MotionCapture;
using Warudo.Plugins.Core.Utils;
using Rikia;

namespace Rikia
{
    [AssetType(Id = "319e02f3-8b21-4412-8221-e8c36b4965f1", Title = "VRCFT Reciever", Category = "CATEGORY_MOTION_CAPTURE")]
    public class VRCFTReceiverAsset : GenericTrackerAsset
    {
        [Markdown(-5000, false, false)]
        public string Status = "RECEIVER_NOT_STARTED".Localized();

        [DataInput(-999)]
        [Label("PORT")]
        public int Port = 9000;
        private VRCFTReceiverBehavior receiver;
        public override bool UseHeadIK => false;
        public override bool UseCharacterDaemon => false;
        public override bool CanCalibrate => false;

        public override List<string> InputBlendShapes
        {
            get
            {
                return ((IEnumerable<string>)BlendShapes.ARKitBlendShapeNames).ToList<string>();
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.receiver = new GameObject("VMC Receiver").AddComponent<VMCReceiverBehavior>();
            this.Watch("Port", new Action(this.ResetReceiver));
            this.ResetReceiver();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!((UnityEngine.Object)this.receiver != (UnityEngine.Object)null))
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object)this.receiver.gameObject);
            this.receiver = (VMCReceiverBehavior)null;
        }

        public async void ResetReceiver()
        {
            VMCReceiverAsset vmcReceiverAsset = this;
            vmcReceiverAsset.IsStarted = false;
            vmcReceiverAsset.SetActive(false);
            vmcReceiverAsset.receiver.StopServer();
            try
            {
                await Context.PluginManager.GetPlugin<CorePlugin>().BeforeListenToPort();
                vmcReceiverAsset.receiver.StartServer(vmcReceiverAsset.Port);
                Context.PluginManager.GetPlugin<CorePlugin>().AfterListenToPort();
                vmcReceiverAsset.IsStarted = true;
                vmcReceiverAsset.Status = "RECEIVER_STARTED_ON_PORT".Localized((object)vmcReceiverAsset.Port, (object)string.Join(", ", Networking.GetLocalIPAddresses().Select<IPAddress, string>((Func<IPAddress, string>)(it => it.ToString()))));
                vmcReceiverAsset.SetActive(true);
            }
            catch (Exception ex)
            {
                Log.UserError("Failed to start VRCFT receiver on port " + vmcReceiverAsset.Port.ToString(), ex);
                vmcReceiverAsset.Status = $"{"FAILED_TO_START_RECEIVER_ANOTHER_PROCESS_IS_ALREADY_LISTENING_ON_THIS_PORT".Localized((object)0)}\n\n{ex.PrettyPrint()}";
            }
            vmcReceiverAsset.BroadcastDataInput("Status");
        }

        protected override bool UpdateRawData()
        {
            if ((UnityEngine.Object)this.receiver == (UnityEngine.Object)null || (double)Time.realtimeSinceStartup - (double)this.receiver.LastReceivedTime > 0.5)
                return false;
            this.RawBlendShapes.Clear();
            this.RawBlendShapes.CopyFrom<string, float>(this.receiver.LatestBlendShapes);
            return true;
        }

    }
}