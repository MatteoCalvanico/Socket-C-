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
            string MyIp;

            //Controlla se la text box del proprio ind. IP è vuota oppure no; se è vuota prende in automatico l'ind. IP della macchina
            if(string.IsNullOrEmpty(txtMyIp.Text) == true || string.IsNullOrWhiteSpace(txtMyIp.Text) == true)
            {
                MyIp = Dns.GetHostName();
                IPAddress[] iphostentry = Dns.GetHostAddresses(MyIp);
                foreach (IPAddress ip in iphostentry)
                {
                    MyIp = ip.ToString();
                }
                txtMyIp.Text = MyIp;
            }
            else
            {
                MyIp = txtMyIp.Text;
            }

            //Controllo textBox ind. IP
            if (string.IsNullOrEmpty(MyIp) == true || string.IsNullOrWhiteSpace(MyIp) == true || controlloIP(MyIp) == false)
            {
                MessageBox.Show("IP sbagliato", "IP sbagliato");
            }else
            {
                IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse(MyIp), 56000);

                //btn per invio abilitato
                btnInvia.IsEnabled = true;
                btnCreaSocket.IsEnabled = false;

                //creazione Thread per controllare la comunicazione
                Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));
                ricezione.Start(sourceSocket);
            }
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
                            lstMsg.Items.Add(message); //Usiamo il Dispatcher per lavorare nel Thread con elemnti WPF e con la lista possiamo vedere tutti i messaggi
                        }));
                    }
                }
            });
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = txtIp.Text;

            if (string.IsNullOrEmpty(ipAddress) == true || string.IsNullOrWhiteSpace(ipAddress) == true || controlloIP(ipAddress) == false)
            {
                MessageBox.Show("IP sbagliato", "IP sbagliato");
            }else
            {
                int port = int.Parse(txtPort.Text);
                SocketSend(IPAddress.Parse(ipAddress), port, txtMsg.Text);
            }
        }

        public void SocketSend(IPAddress dest, int destPort, string message)
        {
            Byte[] byteSend = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destPort);

            s.SendTo(byteSend, remote_endpoint);
        }

        private static bool controlloIP(string ipAddress)
        {
            bool ok = false;

            try
            {
                IPAddress address;
                ok = IPAddress.TryParse(ipAddress, out address);
            }
            catch (Exception ex)
            {

            }

            return ok;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("parte da fare");
        }
    }
}