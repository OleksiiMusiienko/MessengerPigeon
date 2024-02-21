using CommandDLL;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessengerPigeon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public SynchronizationContext uiContext;

        public MainWindow()
        {
            InitializeComponent();
            //uiContext = SynchronizationContext.Current;
        }
        
        private /*async*/ void Button_Click(object sender, RoutedEventArgs e)
        {
            //await Task.Run(async () =>
            //{
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Image files(*.png;*.jpeg;*JPG)|*.png;*.jpeg;*.JPG|All files(*.*)|*.*";
                    ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (ofd.ShowDialog() == true)
                {
                    BitmapImage myImage = new BitmapImage();
                    myImage.BeginInit();
                    myImage.UriSource = new Uri(ofd.FileName);
                    myImage.DecodePixelWidth= 150;
                    myImage.EndInit();
                    /*uiContext.Send(d => */
                    Avatar.ImageSource = myImage /*, null)*/;
                    // конвертируем фото в байтовый массив                
                    MemoryStream stream = new MemoryStream();
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)Avatar.ImageSource));
                    encoder.Save(stream);
                    // байтовый массив присваиваем публичному свойству вьюмодели
                    ((MessengerViewModel)Resources["ViewModel"]).Avatar = stream.ToArray();
                    stream.Close();
                    PopUpSettings.IsOpen = true;
                }
            }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            //});
        }

        private void Smile_Click(object sender, RoutedEventArgs e)
        {
            if (!MyPopUp.IsOpen) { MyPopUp.IsOpen = true; }
            else { MyPopUp.IsOpen = false; }
        }

        private void Smile_Happy_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F60a";
        }
        private void Smile_Cool_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F60e";     
        }
        private void Smile_Love_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F60d";
        }
        private void Smile_Confused_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F615";
        }
        private void Smile_Smiley_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F627";
        }
        private void Smile_Smiling_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F600";
        }
        private void Smile_Wink_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F609";
        }
        private void Smile_crossFinger_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F91E";
        }
        private void Smile_maloik_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F91F";
        }
        private void Smile_pointingUp_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0000261D";
        }
        private void Smile_thumb_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F44E";
        }
        private void Smile_thumbUp_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Message.Text = TextBox_Message.Text + "\U0001F44D";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!PopUpSettings.IsOpen) { PopUpSettings.IsOpen = true; }
            else { PopUpSettings.IsOpen = false; }
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            PopUpSettings.IsOpen = false;
        }

        private void Button_Remove(object sender, RoutedEventArgs e)
        {
            PopUpSettings.IsOpen = false;
        }

        private void Button_Redaction(object sender, RoutedEventArgs e)
        {
            PopUpSettings.IsOpen = false;
        }
    }
}