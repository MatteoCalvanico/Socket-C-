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
     //New using
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ES_Socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse("10.73.0.5"),56000);


            //btn per invio abilitato
            btnInvia.IsEnabled = true;
            btnCreaSocket.IsEnabled = false;

            //creazione Thread per controllare la comunicazione
            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));
            ricezione.Start(sourceSocket);
        }

        public async void SocketReceive(object sockSource)
        {
            //Variabili messaggi
            Byte[] byteRicevuti = new Byte[256];
            string message;
            int contaCaratteri = 0;


            //Creazione e associazione socket
            IPEndPoint ipEndP = (IPEndPoint)sockSource;
            Socket t = new Socket(ipEndP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            t.Bind(ipEndP);


            //Non blocca l'interfaccia quando riceve
            await Task.Run(() =>
            {
                while (true)
                {
                    if (t.Available > 0) //Controlla se riceve qualcosa
                    {
                        message = "";
                        contaCaratteri = t.Receive(byteRicevuti, byteRicevuti.Length, 0); //Quello che riceve lo mette nell'array e conta i byte ricevuti
                        message += Encoding.ASCII.GetString(byteRicevuti, 0, contaCaratteri); //Mette ciò che ha ricevuto nel messaggio

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblRicevi.Content = message; //Usiamo il Dispatcher per lavorare nel Thread con elemnti WPF
                        }));
                    }
                }
            });
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = txtIp.Text;
            if (txtIp.Text == "" || txtIp.Text == " " || txtIp.Text == null)
            {
                MessageBox.Show("Errore");
            }
            //TODO => aggungere controllo ip e porta !
            int port = int.Parse(txtPort.Text);

            SocketSend(IPAddress.Parse(ipAddress), port, txtMsg.Text);
        }

        public void SocketSend(IPAddress dest, int destPort, string message)
        {
            Byte[] byteSend = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destPort);

            s.SendTo(byteSend, remote_endpoint);
        }
    }
}