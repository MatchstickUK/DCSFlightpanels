﻿using System.Threading;
using NonVisuals.Interfaces;

namespace NonVisuals.StreamDeck
{
    public enum LayerNavType
    {
        SwitchToSpecificLayer = 0,
        Back = 1,
        Home = 2
    }

    public class ActionTypeLayer : IStreamDeckButtonTypeBase, IStreamDeckButtonAction
    {
        public EnumStreamDeckActionType ActionType => EnumStreamDeckActionType.LayerNavigation;
        public bool IsRepeatable() => false;
        private volatile bool _isRunning;
        private EnumStreamDeckButtonNames _streamDeckButtonName;
        public LayerNavType NavigationType;
        public string TargetLayer;
        private string _streamDeckInstanceId;


        public string Description { get => "Layer Navigation"; }

        public bool IsRunning()
        {
            return _isRunning;
        }


        public void Execute(CancellationToken threadCancellationToken)
        {
            _isRunning = true;
            Navigate(threadCancellationToken);
            _isRunning = false;
        }
        public EnumStreamDeckButtonNames StreamDeckButtonName
        {
            get => _streamDeckButtonName;
            set => _streamDeckButtonName = value;
        }

        public void Navigate(CancellationToken threadCancellationToken)
        {
            switch (NavigationType)
            {
                case LayerNavType.Home:
                {
                    StreamDeckPanel.GetInstance(_streamDeckInstanceId).ShowHomeLayer();
                    break;
                }
                case LayerNavType.Back:
                {
                    StreamDeckPanel.GetInstance(_streamDeckInstanceId).ShowPreviousLayer();
                    break;
                }
                case LayerNavType.SwitchToSpecificLayer:
                {
                    StreamDeckPanel.GetInstance(_streamDeckInstanceId).ActiveLayer = TargetLayer;
                    break;
                }
            }
        }

        
        public string StreamDeckInstanceId
        {
            get => _streamDeckInstanceId;
            set => _streamDeckInstanceId = value;
        }
    }
}
