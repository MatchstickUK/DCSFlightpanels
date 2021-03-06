﻿using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NonVisuals;
using NonVisuals.StreamDeck;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;

namespace DCSFlightpanels.Bills
{
    public class BillStreamDeckFace : BillBaseOutput
    {
        public EnumStreamDeckButtonNames StreamDeckButtonName;
        private ActionTypeLayer _streamDeckTargetLayer;
        private BIPLinkStreamDeck _bipLinkStreamDeck;
        public StreamDeckButton Button;
        private Font _textFont = Constants.DefaultStreamDeckFont;
        private Color _fontColor = Color.Black;
        private Color _backgroundColor = ColorTranslator.FromHtml(Constants.COLOR_DEFAULT_WHITE);
        private bool _isSelected = false;
        public BitmapImage SelectedImage { get; set; }
        public BitmapImage DeselectedImage { get; set; }
        private DCSBIOSDecoder _dcsbiosDecoder;
        private string _imageFilePath;

        public override bool IsEmpty()
        {
            return (_bipLinkStreamDeck == null || _bipLinkStreamDeck.BIPLights.Count == 0) && 
                   _streamDeckTargetLayer == null;
        }
        
        public bool ContainsTextFace()
        {
            return _textFont != null && !string.IsNullOrEmpty(TextBox.Text); 
        }

        public bool ContainsImageFace()
        {
            return !string.IsNullOrEmpty(ImageFilePath);
        }

        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                TextBox.FontFamily = new System.Windows.Media.FontFamily(_textFont.Name);
                TextBox.FontWeight = _textFont.Bold ? FontWeights.Bold : FontWeights.Regular;
                TextBox.FontSize = _textFont.Size * 96.0 / 72.0;
                TextBox.FontStyle = _textFont.Italic ? FontStyles.Italic : FontStyles.Normal;
                var textDecorationCollection = new TextDecorationCollection();
                if (_textFont.Underline) textDecorationCollection.Add(TextDecorations.Underline);
                if (_textFont.Strikeout) textDecorationCollection.Add(TextDecorations.Strikethrough);
                TextBox.TextDecorations = textDecorationCollection;
            }
        }
        
        public override bool ContainsDCSBIOS()
        {
            return _dcsbiosDecoder != null;
        }

        public bool ContainsStreamDeckLayer()
        {
            return _streamDeckTargetLayer != null;
        }

        public override bool ContainsBIPLink()
        {
            return _bipLinkStreamDeck != null && _bipLinkStreamDeck.BIPLights.Count > 0;
        }

        public Color FontColor
        {
            get => _fontColor;
            set
            {
                _fontColor = value;
                TextBox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_fontColor.A, _fontColor.R, _fontColor.G, _fontColor.B)); ;
            }
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                TextBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_backgroundColor.A, _backgroundColor.R, _backgroundColor.G, _backgroundColor.B)); ;
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }


        public int OffsetX { get; set; } = 10;
        public int OffsetY { get; set; } = 24;
        

        public int ButtonNumber()
        {
            if (StreamDeckButtonName == EnumStreamDeckButtonNames.BUTTON0_NO_BUTTON)
            {
                return 0;
            }
            
            return int.Parse(StreamDeckButtonName.ToString().Replace("BUTTON", ""));
            
        }

        public bool IsClean => OffsetX == 0 && OffsetY == 0 && BackgroundColor == ColorTranslator.FromHtml(Constants.COLOR_DEFAULT_WHITE) && FontColor == Color.Black && TextFont.Name == Constants.DEFAULT_FONT;
        
        public string BackgroundHex => "#" + _backgroundColor.R.ToString("X2") + _backgroundColor.G.ToString("X2") + _backgroundColor.B.ToString("X2");

        public override void Clear()
        {
            _dcsbiosDecoder = null;
            _bipLinkStreamDeck = null;
            if (TextBox != null)
            {
                TextBox.Background = Brushes.LightSteelBlue;
                TextBox.Text = "";
            }
            _imageFilePath = "";
        }

        public DCSBIOSDecoder DCSBIOSDecoder
        {
            get => _dcsbiosDecoder;
            set
            {
                _dcsbiosDecoder = value;
                TextBox.Text = _dcsbiosDecoder.DCSBIOSOutput.ControlId;
            }
        }


        public string ImageFilePath
        {
            get => _imageFilePath;
            set
            {
                _imageFilePath = value;
                TextBox.Text = _imageFilePath;
            }
        }
    }

}
