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
                    Avatar.DataContext = myImage /*, null)*/;
                    // конвертируем фото в байтовый массив                
                    MemoryStream stream = new MemoryStream();
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)Avatar.DataContext));
                    encoder.Save(stream);
                    // байтовый массив присваиваем публичному свойству вьюмодели
                    ((MessengerViewModel)Resources["ViewModel"]).Avatar = stream.ToArray();
                    stream.Close();
                }
            }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            //});
        }
    }
}