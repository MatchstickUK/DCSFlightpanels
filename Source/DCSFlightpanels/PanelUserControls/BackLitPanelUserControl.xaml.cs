﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ClassLibraryCommon;
using DCSFlightpanels.Properties;
using DCSFlightpanels.Windows;
using NonVisuals;
using NonVisuals.Interfaces;
using NonVisuals.Saitek;

namespace DCSFlightpanels.PanelUserControls
{
    /// <summary>
    /// Interaction logic for BackLitPanelUserControl.xaml
    /// </summary>
    public partial class BackLitPanelUserControl : UserControlBase, IGamingPanelListener, IProfileHandlerListener, IGamingPanelUserControl
    {

        private readonly BacklitPanelBIP _backlitPanelBIP;
        private readonly TabItem _parentTabItem;
        private string _parentTabItemHeader;
        private readonly List<Image> _colorImages = new List<Image>();
        private readonly List<Image> _configurationIndicatorImages = new List<Image>();
        private readonly BitmapImage _blackImage = new BitmapImage(new Uri("pack://application:,,,/DCSFlightpanels;component/Images/black.png"));
        private readonly BitmapImage _redImage = new BitmapImage(new Uri("pack://application:,,,/DCSFlightpanels;component/Images/red.png"));
        private readonly BitmapImage _greenImage = new BitmapImage(new Uri("pack://application:,,,/DCSFlightpanels;component/Images/green.png"));
        private readonly BitmapImage _yellowImage = new BitmapImage(new Uri("pack://application:,,,/DCSFlightpanels;component/Images/yellow1.png"));
        private PanelLEDColor _lastToggleColor = PanelLEDColor.DARK;
        private bool _loaded;
        private readonly IGlobalHandler _globalHandler;

        public BackLitPanelUserControl(TabItem parentTabItem, IGlobalHandler globalHandler, HIDSkeleton hidSkeleton)
        {
            InitializeComponent();
            _parentTabItem = parentTabItem;
            _parentTabItemHeader = _parentTabItem.Header.ToString();
            _backlitPanelBIP = new BacklitPanelBIP(Settings.Default.BIPLedStrength, hidSkeleton);

            _backlitPanelBIP.Attach((IGamingPanelListener)this);
            globalHandler.Attach(_backlitPanelBIP);
            _globalHandler = globalHandler;
        }

        private void BackLitPanelUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            Init();
            _loaded = true;
            SetContextMenuClickHandlers();
            SetAllBlack();
            ShowGraphicConfiguration();
        }

        public void BipPanelRegisterEvent(object sender, BipPanelRegisteredEventArgs e)
        {
        }

        public GamingPanel GetGamingPanel()
        {
            return _backlitPanelBIP;
        }

        public string GetName()
        {
            return GetType().Name;
        }

        public void SelectedAirframe(object sender, AirframeEventArgs e) { }
        

        public void UpdatesHasBeenMissed(object sender, DCSBIOSUpdatesMissedEventArgs e) { }

        public void PanelSettingsReadFromFile(object sender, SettingsReadFromFileEventArgs e) { }

        public void SwitchesChanged(object sender, SwitchesChangedEventArgs e) { }

        public void SettingsCleared(object sender, PanelEventArgs e)
        {
            try
            {
                ShowGraphicConfiguration();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2009, ex);
            }
        }

        public void LedLightChanged(object sender, LedLightChangeEventArgs e)
        {
            try
            {
                if (!_loaded)
                {
                    return;
                }
                Dispatcher?.BeginInvoke((Action)(ShowGraphicConfiguration));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2033, ex);
            }
        }

        public void PanelDataAvailable(object sender, PanelDataToDCSBIOSEventEventArgs e) { }

        public void SettingsApplied(object sender, PanelEventArgs e)
        {
            try
            {
                if (!_loaded)
                {
                    return;
                }
                Dispatcher?.BeginInvoke((Action)(ShowGraphicConfiguration));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2015, ex);
            }
        }

        public void PanelSettingsChanged(object sender, PanelEventArgs e)
        {
            try
            {
                if (!_loaded)
                {
                    return;
                }
                Dispatcher?.BeginInvoke((Action)(ShowGraphicConfiguration));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2040, ex);
            }
        }

        public void DeviceAttached(object sender, PanelEventArgs e)
        {
            try
            {
                if (e.GamingPanelEnum == GamingPanelEnum.BackLitPanel && e.UniqueId.Equals(_backlitPanelBIP.InstanceId))
                {
                    //Dispatcher?.BeginInvoke((Action)(() => _parentTabItem.Header = _parentTabItemHeader + " (connected)"));
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2045, ex);
            }
        }

        public void DeviceDetached(object sender, PanelEventArgs e)
        {
            try
            {
                if (e.GamingPanelEnum == GamingPanelEnum.BackLitPanel && e.UniqueId.Equals(_backlitPanelBIP.InstanceId))
                {
                    //Dispatcher?.BeginInvoke((Action)(() => _parentTabItem.Header = _parentTabItemHeader + " (disconnected)"));
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2062, ex);
            }
        }

        private void Init()
        {
            var imageList = Common.FindVisualChildren<Image>(this);
            foreach (var image in imageList)
            {
                if (image.Name.StartsWith("ImagePosition"))
                {
                    _colorImages.Add(image);
                }
                if (image.Name.StartsWith("ImageConfigFound"))
                {
                    _configurationIndicatorImages.Add(image);
                }
            }
        }

        private void ShowGraphicConfiguration()
        {
            try
            {
                if (!_loaded)
                {
                    return;
                }
                HideAllConfigurationExistsImages();
                var bipPositions = CommonClassLibraryJD.EnumEx.GetValues<BIPLedPositionEnum>();
                foreach (var position in bipPositions)
                {
                    SetLEDImage(position, _backlitPanelBIP.GetColor(position));
                    SetConfigExistsImageVisibility(_backlitPanelBIP.HasConfiguration(position), position);
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2066, ex);
            }
        }


        private void SetAllBlack()
        {
            foreach (var image in _colorImages)
            {
                image.Source = _blackImage;
            }
        }

        private void MenuItem_ContextConfigureBIPLED_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var contextMenu = (ContextMenu)menuItem.Parent;
                var imageName = contextMenu.Tag.ToString();
                var position = GetLedPosition(imageName);

                var ledConfigsWindow = new LEDConfigsWindow(_globalHandler.GetAirframe(), "Set configuration for LED : " + position, new SaitekPanelLEDPosition(position), _backlitPanelBIP.GetLedDcsBiosOutputs(position), _backlitPanelBIP);
                if (ledConfigsWindow.ShowDialog() == true)
                {
                    //must include position because if user has deleted all entries then there is nothing to go after regarding position
                    _backlitPanelBIP.SetLedDcsBiosOutput(position, ledConfigsWindow.ColorOutputBindings);
                }
                ShowGraphicConfiguration();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2067, ex);
            }
        }

        private BIPLedPositionEnum GetLedPosition(string imageName)
        {
            Common.DebugP("BIP image name clicked is : " + imageName);
            var result = BIPLedPositionEnum.Position_1_1;
            //ImagePosition3_4
            var str = imageName.Remove(0, 13);
            //3_4
            var row = int.Parse(str.Substring(0, 1));
            var index = int.Parse(str.Substring(2, 1));
            try
            {
                switch (row)
                {
                    case 1:
                        {
                            switch (index)
                            {
                                case 1:
                                    {
                                        result = BIPLedPositionEnum.Position_1_1;
                                        break;
                                    }
                                case 2:
                                    {
                                        result = BIPLedPositionEnum.Position_1_2;
                                        break;
                                    }
                                case 3:
                                    {
                                        result = BIPLedPositionEnum.Position_1_3;
                                        break;
                                    }
                                case 4:
                                    {
                                        result = BIPLedPositionEnum.Position_1_4;
                                        break;
                                    }
                                case 5:
                                    {
                                        result = BIPLedPositionEnum.Position_1_5;
                                        break;
                                    }
                                case 6:
                                    {
                                        result = BIPLedPositionEnum.Position_1_6;
                                        break;
                                    }
                                case 7:
                                    {
                                        result = BIPLedPositionEnum.Position_1_7;
                                        break;
                                    }
                                case 8:
                                    {
                                        result = BIPLedPositionEnum.Position_1_8;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 2:
                        {
                            switch (index)
                            {
                                case 1:
                                    {
                                        result = BIPLedPositionEnum.Position_2_1;
                                        break;
                                    }
                                case 2:
                                    {
                                        result = BIPLedPositionEnum.Position_2_2;
                                        break;
                                    }
                                case 3:
                                    {
                                        result = BIPLedPositionEnum.Position_2_3;
                                        break;
                                    }
                                case 4:
                                    {
                                        result = BIPLedPositionEnum.Position_2_4;
                                        break;
                                    }
                                case 5:
                                    {
                                        result = BIPLedPositionEnum.Position_2_5;
                                        break;
                                    }
                                case 6:
                                    {
                                        result = BIPLedPositionEnum.Position_2_6;
                                        break;
                                    }
                                case 7:
                                    {
                                        result = BIPLedPositionEnum.Position_2_7;
                                        break;
                                    }
                                case 8:
                                    {
                                        result = BIPLedPositionEnum.Position_2_8;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 3:
                        {
                            switch (index)
                            {
                                case 1:
                                    {
                                        result = BIPLedPositionEnum.Position_3_1;
                                        break;
                                    }
                                case 2:
                                    {
                                        result = BIPLedPositionEnum.Position_3_2;
                                        break;
                                    }
                                case 3:
                                    {
                                        result = BIPLedPositionEnum.Position_3_3;
                                        break;
                                    }
                                case 4:
                                    {
                                        result = BIPLedPositionEnum.Position_3_4;
                                        break;
                                    }
                                case 5:
                                    {
                                        result = BIPLedPositionEnum.Position_3_5;
                                        break;
                                    }
                                case 6:
                                    {
                                        result = BIPLedPositionEnum.Position_3_6;
                                        break;
                                    }
                                case 7:
                                    {
                                        result = BIPLedPositionEnum.Position_3_7;
                                        break;
                                    }
                                case 8:
                                    {
                                        result = BIPLedPositionEnum.Position_3_8;
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2068, ex);
            }
            return result;
        }

        private void SetContextMenuClickHandlers()
        {
            if (!Common.IsOperationModeFlagSet(OperationFlag.DCSBIOSOutputEnabled))
            {
                return;
            }
            foreach (var image in Common.FindVisualChildren<Image>(this))
            {
                if (image.Name.StartsWith("ImagePosition"))
                {
                    image.ContextMenu = (ContextMenu)Resources["BIPLEDContextMenu"];
                    image.ContextMenu.Tag = image.Name;
                }
            }
        }

        private void ImagePosition_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var newColor = PanelLEDColor.DARK;
                var image = (Image)sender;
                var currentColor = (PanelLEDColor)(Enum.Parse(typeof(PanelLEDColor), ((string)image.Tag)));
                if (currentColor == PanelLEDColor.DARK)
                {
                    image.Source = _greenImage;
                    newColor = PanelLEDColor.GREEN;
                    image.Tag = "GREEN";
                }
                else if (currentColor == PanelLEDColor.GREEN)
                {
                    image.Source = _yellowImage;
                    newColor = PanelLEDColor.YELLOW;
                    image.Tag = "YELLOW";
                }
                else if (currentColor == PanelLEDColor.YELLOW)
                {
                    image.Source = _redImage;
                    newColor = PanelLEDColor.RED;
                    image.Tag = "RED";
                }
                else if (currentColor == PanelLEDColor.RED)
                {
                    image.Source = _blackImage;
                    newColor = PanelLEDColor.DARK;
                    image.Tag = "DARK";
                }
                SetLEDImage(image, newColor);
                SetPhysicalLED(image, newColor);
            }
        }

        private void SetLEDImage(BIPLedPositionEnum bipLedPositionEnum, PanelLEDColor newColor)
        {
            Image image = null;
            foreach (var tmpImage in _colorImages)
            {
                if (tmpImage.Name.Contains(bipLedPositionEnum.ToString()))
                {
                    image = tmpImage;
                }
            }
            if (image != null)
            {
                SetLEDImage(image, newColor);
            }
        }

        private void SetLEDImage(Image image, PanelLEDColor newColor)
        {
            if (newColor == PanelLEDColor.DARK)
            {
                image.Source = _blackImage;
                image.Tag = "DARK";
            }
            else if (newColor == PanelLEDColor.GREEN)
            {
                image.Source = _greenImage;
                image.Tag = "GREEN";
            }
            else if (newColor == PanelLEDColor.YELLOW)
            {
                image.Source = _yellowImage;
                image.Tag = "YELLOW";
            }
            else if (newColor == PanelLEDColor.RED)
            {
                image.Source = _redImage;
                image.Tag = "RED";
            }
        }


        private void HideAllConfigurationExistsImages()
        {
            foreach (var configurationIndicatorImage in _configurationIndicatorImages)
            {
                configurationIndicatorImage.Visibility = Visibility.Collapsed;
            }
        }

        private void SetConfigExistsImageVisibility(bool show, BIPLedPositionEnum bipLedPositionEnum)
        {
            var posString = _backlitPanelBIP.GetPosString(bipLedPositionEnum);
            foreach (var configurationIndicatorImage in _configurationIndicatorImages)
            {
                if (configurationIndicatorImage.Name.Contains(posString))
                {
                    if (show)
                    {
                        configurationIndicatorImage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        configurationIndicatorImage.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void SetPhysicalLED(Image image, PanelLEDColor newColor)
        {
            var position = GetLedPosition(image.Name);
            _backlitPanelBIP.SetLED(position, newColor);
        }

        private void ToggleAll_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_lastToggleColor)
                {
                    case PanelLEDColor.DARK:
                        {
                            _lastToggleColor = PanelLEDColor.GREEN;
                            break;
                        }
                    case PanelLEDColor.GREEN:
                        {
                            _lastToggleColor = PanelLEDColor.YELLOW;
                            break;
                        }
                    case PanelLEDColor.YELLOW:
                        {
                            _lastToggleColor = PanelLEDColor.RED;
                            break;
                        }
                    case PanelLEDColor.RED:
                        {
                            _lastToggleColor = PanelLEDColor.DARK;
                            break;
                        }
                }
                foreach (var image in _colorImages)
                {
                    SetLEDImage(image, _lastToggleColor);
                    SetPhysicalLED(image, _lastToggleColor);
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2069, ex);
            }
        }

        private void LEDIncrease_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _backlitPanelBIP.LEDBrightnessIncrease();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2070, ex);
            }
        }

        private void LEDDecrease_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _backlitPanelBIP.LEDBrightnessDecrease();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2071, ex);
            }
        }


        private void ButtonGetId_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_backlitPanelBIP != null)
                {
                    TextBoxLogBIP.Text = "";
                    TextBoxLogBIP.Text = _backlitPanelBIP.InstanceId;
                    Clipboard.SetText(_backlitPanelBIP.InstanceId);
                    MessageBox.Show("The Instance Id for the panel has been copied to the Clipboard.");
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2080, ex);
            }
        }

        private void ButtonGetHash_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_backlitPanelBIP != null)
                {
                    Clipboard.SetText(Common.GetMd5Hash(_backlitPanelBIP.InstanceId));
                    MessageBox.Show("The MD5 hash for the panel has been copied to the Clipboard.\nUse this value when you connect switches to B.I.P. lights.");
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(2080, ex);
            }
        }
    }
}
