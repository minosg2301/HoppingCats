using Doozy.Engine;
using Doozy.Engine.Progress;
using UnityEngine;

namespace moonNest
{
    public class BootProgressNode : MonoBehaviour
    {
        public string progressEvent;
        public Progressor progressor;

        void Start()
        {
            Message.AddListener<ProgressMessage>(progressEvent, OnProgressUpdate);
        }

        void OnDestroy()
        {
            Message.RemoveListener<ProgressMessage>(progressEvent, OnProgressUpdate);
        }

        void OnProgressUpdate(ProgressMessage msg)
        {
            progressor.gameObject.SetActive(true);
            progressor.SetProgress(msg.progress);
        }
    }
}