using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace DashboardCarUI
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 1,
        ACCENT_ENABLE_GRADIENT = 0,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private Car nextCar;

        public MainWindow()
        {
            InitializeComponent();
            AccuBatteryController accubatteryController = new AccuBatteryController();
            PintuController Pintunya = new PintuController();

            nextCar = new Car();
            engineState.Visibility = Visibility.Hidden;

            var bc = new BrushConverter();

            nextCar.setAccubaterryController(accubatteryController);
            nextCar.SetPintuController(Pintunya);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
        public async void Sembunyikan()
        {
            await Task.Delay(2000);
            engineState.Visibility = Visibility.Hidden;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            Boolean powerIsOn = nextCar.powerIsReady();
            Boolean powerIsOff = nextCar.powerIsNotReady();
            Boolean PintuTerkunci = nextCar.PintuIsLocked();
            Boolean PintuTerbuka = nextCar.PintuIsUnLocked();

            if (powerIsOn && PintuTerkunci && this.buttonStart.Content == "START Engine!")
            {
                var bc = new BrushConverter();
                this.buttonStart.Content = "STOP Engine!";
                //Red
                this.buttonStart.Background = (Brush)bc.ConvertFrom("#99FF0000");
                engineState.Visibility = Visibility.Visible;
                engineState.Content = "Accu and Door are still On!";
                Sembunyikan();

                this.nextCar.turnOnPower();
                this.accuState.Content = "Have Power";
                this.buttonAccu.Content = "ON";
                //Green
                this.buttonAccu.Background = (Brush)bc.ConvertFrom("#990AC758");

                this.nextCar.lockPintu();
                this.doorState.Content = "Pintu Terkunci";
                this.buttonDoor.Content = "ON";
                //Green
                this.buttonDoor.Background = (Brush)bc.ConvertFrom("#990AC758");
            }
            else if (powerIsOn && PintuTerkunci && this.buttonStart.Content == "STOP Engine!")
            {
                var bc = new BrushConverter();

                this.buttonStart.Content = "START Engine!";
                //Green
                this.buttonStart.Background = (Brush)bc.ConvertFrom("#990AC758");
                engineState.Visibility = Visibility.Visible;
                engineState.Content = "Accu and Door are still On!";
                Sembunyikan();

                this.nextCar.turnOfPower();
                this.accuState.Content = "No Power";
                this.buttonAccu.Content = "OFF";
                //Red
                this.buttonAccu.Background = (Brush)bc.ConvertFrom("#99FF0000");

                this.nextCar.unlockPintu();
                this.doorState.Content = "Pintu Terbuka";
                this.buttonDoor.Content = "OFF";
                //Red
                this.buttonDoor.Background = (Brush)bc.ConvertFrom("#99FF0000");
            }
            else
            {
                var bc = new BrushConverter();
                this.buttonStart.Content = "START Engine!";
                //Green
                this.buttonStart.Background = (Brush)bc.ConvertFrom("#990AC758");
                engineState.Visibility = Visibility.Visible;
                engineState.Content = "Accu and Door Are Not Ready!";
                Sembunyikan();
            }

            Console.WriteLine("button start");
        }

        private void buttonAccu_Click(object sender, RoutedEventArgs e)
        {
            Boolean powerIsOn = nextCar.powerIsReady();
            Boolean powerIsOff = nextCar.powerIsNotReady();
            var bc = new BrushConverter();

            if (powerIsOn)
            {
                this.nextCar.turnOfPower();
                this.accuState.Content = "No Power";
                this.buttonAccu.Content = "OFF";
                //Red
                this.buttonAccu.Background = (Brush)bc.ConvertFrom("#99FF0000");
            }
            else
            {
                this.nextCar.turnOnPower();
                this.accuState.Content = "Have Power";
                this.buttonAccu.Content = "ON";
                //Green
                this.buttonAccu.Background = (Brush)bc.ConvertFrom("#990AC758");
            }
            if (powerIsOff && buttonStart.Content == "STOP Engine!")
            {
                this.nextCar.unlockPintu();
                this.doorState.Content = "Pintu Terbuka";
                this.buttonDoor.Content = "OFF";
                //Red
                this.buttonDoor.Background = (Brush)bc.ConvertFrom("#99FF0000");

                this.nextCar.turnOfPower();
                this.accuState.Content = "No Power";
                this.buttonAccu.Content = "OFF";
                //Red
                this.buttonAccu.Background = (Brush)bc.ConvertFrom("#99FF0000");

                this.buttonStart.Content = "START Engine!";
                //Green
                this.buttonStart.Background = (Brush)bc.ConvertFrom("#990AC758");

            }

            Console.WriteLine("button aki");
        }

        private void buttonDoor_Click(object sender, RoutedEventArgs e)
        {
            Boolean PintuTerkunci = nextCar.PintuIsLocked();
            Boolean PintuTerbuka = nextCar.PintuIsLocked();
            var bc = new BrushConverter();

            if (PintuTerkunci)
            {
                this.nextCar.unlockPintu();
                this.doorState.Content = "Pintu Terbuka";
                this.buttonDoor.Content = "OFF";
                //Red
                this.buttonDoor.Background = (Brush)bc.ConvertFrom("#99FF0000");
            }
            else
            {
                this.nextCar.lockPintu();
                this.doorState.Content = "Pintu Terkunci";
                this.buttonDoor.Content = "ON";
                //Green
                this.buttonDoor.Background = (Brush)bc.ConvertFrom("#990AC758");
            }
            if (PintuTerbuka && buttonStart.Content == "STOP Engine!")
            {
                this.nextCar.unlockPintu();
                this.doorState.Content = "Pintu Terbuka";
                this.buttonDoor.Content = "OFF";
                //Red
                this.buttonDoor.Background = (Brush)bc.ConvertFrom("#99FF0000");
                this.nextCar.turnOfPower();
                this.accuState.Content = "No Power";
                this.buttonAccu.Content = "OFF";
                //Red
                this.buttonAccu.Background = (Brush)bc.ConvertFrom("#99FF0000");
                this.buttonStart.Content = "START Engine!";
                //Green
                this.buttonStart.Background = (Brush)bc.ConvertFrom("#990AC758");


            }

            Console.WriteLine("button pintu");
        }
    }
}
