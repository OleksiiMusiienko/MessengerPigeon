using CommandDLL;
using Microsoft.Win32;
using NAudio.Wave;
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
using NAudio;
using System.Runtime.Serialization.Json;
using MessengerModel;
using NAudio.Utils;
using System.Drawing.Printing;
using System.Printing;
using static System.Net.Mime.MediaTypeNames;
//using System.Windows.Forms;

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
                    //Avatar.ImageSource = null;
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
            Avatar.ImageSource = null;
        }

        private void Button_Remove(object sender, RoutedEventArgs e)
        {
            PopUpSettings.IsOpen = false;
            Avatar.ImageSource = null;
        }

        private void Button_Redaction(object sender, RoutedEventArgs e)
        {
            PopUpSettings.IsOpen = false;
        }

        WaveIn waveIn;
        WaveFileWriter writer;
        string outputFilename = null;
        MemoryStream streamAudio;
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
             writer.Write(e.Buffer.ToArray(), 0, e.BytesRecorded);
             writer.Flush();
        }                
        private void Voice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                streamAudio = new MemoryStream();
                waveIn = new WaveIn();
                waveIn.DeviceNumber=0;
                outputFilename = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss").Trim() + "Audio.wav";
                waveIn.DataAvailable+= new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);
                waveIn.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(waveIn.DeviceNumber).Channels);
                writer = new WaveFileWriter(streamAudio, waveIn.WaveFormat);
                waveIn.StartRecording();
            }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
}
        private void Voice_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                waveIn.StopRecording();
                ((MessengerViewModel)Resources["ViewModel"]).MesAudio = streamAudio.ToArray();
                ((MessengerViewModel)Resources["ViewModel"]).MesAudioUri = outputFilename;
                waveIn.Dispose();
                waveIn = null;
                writer.Close();
                writer = null;
                streamAudio.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент: " + ex.Message);
            }
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    Wrapper wrapper = new Wrapper();
                    wrapper.commands = Wrapper.Commands.Exit;
                    wrapper.user = ((MessengerViewModel)Resources["ViewModel"]).MyUser;
                    if (wrapper.user != null) 
                    {
                        ((MessengerViewModel)Resources["ViewModel"]).MyUser.Online = false;
                        var jsonFormatter = new DataContractJsonSerializer(typeof(Wrapper));
                        jsonFormatter.WriteObject(stream, wrapper);
                        byte[] msg = stream.ToArray();
                        await ((MessengerViewModel)Resources["ViewModel"]).netstream.WriteAsync(msg, 0, msg.Length); // записываем данные в NetworkStream. MemoryStream stream1 = new MemoryStream();

                        MemoryStream stream1 = new MemoryStream();
                        Message mes1 = new Message();
                        mes1.Mes = "ExitOnline";
                        mes1.Date_Time = DateTime.Now;
                        var jsonFormatter1 = new DataContractJsonSerializer(typeof(Message));
                        jsonFormatter1.WriteObject(stream1, mes1);
                        byte[] msg1 = stream1.ToArray();
                        await ((MessengerViewModel)Resources["ViewModel"]).netstreamMessage.WriteAsync(msg1, 0, msg1.Length); // записываем данные в NetworkStream.
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
            this.Close();
        }
        private void MyMediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            string hr = GetHresultFromErrorMessage(e);
        }
        private string GetHresultFromErrorMessage(ExceptionRoutedEventArgs e)
        {
            string hr = string.Empty;
            string token = "HREZULT - ";
            const int hrLength = 10;
            int tokenPos = e.ErrorException.Message.IndexOf(token, StringComparison.Ordinal);
            if (tokenPos != -1)
            {
                hr = e.ErrorException.Message.Substring(tokenPos +token.Length, hrLength);
            }
            return hr;
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            List<Message> messages = ((MessengerViewModel)Resources["ViewModel"]).Messages.ToList();
            if (messages.Count == 0)
                return;
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                TextBlock textBlock = new TextBlock();// создаем визуальный элемент
                textBlock.Margin = new Thickness(30, 10, 10, 10); // устанавливаем поля
                textBlock.TextWrapping = TextWrapping.Wrap; //перенос текста
                textBlock.LayoutTransform = new ScaleTransform(2, 2); //увеличение текста
                if (printDialog.ShowDialog() == true)
                {
                    for (int i = 0; i < messages.Count; i++)
                    {
                        textBlock.Text +=messages[i].Mes + "\n\n";
                    }
                    printDialog.PrintVisual(textBlock, "Печать");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string user = ((MessengerViewModel)Resources["ViewModel"]).Nick;
            try
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "(.txt)|*.txt";
                List<Message> messages = ((MessengerViewModel)Resources["ViewModel"]).Messages.ToList();
                TextBlock textBlock = new TextBlock();
                if (sf.ShowDialog() == true)
                {
                    for (int i = 0; i < messages.Count; i++)
                    {
                        textBlock.Text += messages[i].Mes + "\n\n";
                    }
                    StreamWriter sw = new StreamWriter(sf.FileName, false, Encoding.Default);                    
                    sw.WriteLine(textBlock.Text);
                    sw.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ChatingHistory_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(e.OriginalSource is ScrollViewer scrollViewer && 
                Math.Abs(e.ExtentHeightChange) > 0.0)
            { scrollViewer.ScrollToEnd(); }
        }
    }
}