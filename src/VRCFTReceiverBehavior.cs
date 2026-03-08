using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using uOSC;
using Warudo.Core.Data;
using Warudo.Core.Data.Models;
using Warudo.Core.Utils;

namespace Rikia
{
    public class VRCFTReceiverBehavior : MonoBehaviour
    {
        private uOscServer server;
        public Dictionary<string, float> LatestBlendShapes { get; } = new Dictionary<string, float>(100);
        public float LastReceivedTime { get; private set; }

        private void Awake()
        {
            if (VMCReceiverBehavior.boneNameToIndex == null)
            {
                VMCReceiverBehavior.boneNameToIndex = new Dictionary<string, int>();
                foreach (object obj in Enum.GetValues(typeof(HumanBodyBones)))
                    VMCReceiverBehavior.boneNameToIndex.Add(obj.ToString(), (int)obj);
            }
            this.server = this.gameObject.GetOrAddComponent<uOscServer>();
            this.server.autoStart = false;
        }

        public bool IsServerRunning() => this.server.isRunning;

        public void StartServer(int port)
        {
            this.server.port = port;
            this.server.StartServer();
            this.server.onDataReceived.AddListener(new UnityAction<Message>(this.OnDataReceived));
        }

        public void StopServer() => this.server.StopServer();

        public void OnDestroy()
        {
            this.server.StopServer();
            UnityEngine.Object.Destroy((UnityEngine.Object)this.server);
        }
        public void OnDataReceived(Message message)
        {
            this.ProcessMessage(ref message);
            this.LastReceivedTime = Time.realtimeSinceStartup;
        }

        public void ProcessMessage(ref Message message)
        {
            if (message.address == null || message.values == null)
                return;
            Debug.Log(message.values);
        }

    }
}