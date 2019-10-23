using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using System.IO.Ports;
using System.Drawing;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace PEDESTALSU
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            txtKomutCihazNo.Text = "6011691";
            txtKomutYukKrd.Text = "10000";
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timerbtn.Tick += Timerbtn_Tick;
            timerbtn.Interval = TimeSpan.FromMilliseconds(5000);
        }

        private void Timerbtn_Tick(object sender, EventArgs e)
        {
            timerbtn.Stop();
            btnwrap.IsEnabled = true;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            try
            {
                IstemciString = AkimOkuyucu.ReadLine();
                if (IstemciString.Substring(0, 4) == "HATA")
                {
                    loglst.Items.Add(IstemciString);
                }
                else
                {
                    switch (IstemciString.Substring(0, 6))
                    {
                        //KISA OKUMA BİLGİLERİ
                        case "SayTip": { txtSayTip.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtIss": { txtIssue.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtNsa": { txtNSayisi.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "tFlag0": { txtFlag0.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "tFlag2": { txtFlag2.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "KalKre": { txtKalanKredi.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "HarKre": { txtHarcananKredi.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "CihaNo": { txtCihazNo.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txSaat": { txtSaat.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtTar": { txtTarih.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txHGun": { txtHGun.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtNok": { txtNoktaSayisi.Text = IstemciString.Substring(7); timer.Start(); break; }
                        //UZUN OKUMA BİLGİLERİ
                        case "txtKal": { txtKalan.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "HarKrd": { txtHarcananKrd.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtCNo": { txtCNo.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtHar": { txtHarcanan.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "HarTer": { txtHarcananTers.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKri": { txtKritik.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtThS": { txtTarihSaat.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtRes": { txtResetT.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtCTT": { txtCT.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtATT": { txtAT.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "lblIss": { lblIssue.Text = IstemciString.Substring(7); txtIssue.Text = lblIssue.Text; timer.Start(); break; }
                        case "lblNNN": { lblN.Text = IstemciString.Substring(7); txtNoktaSayisi.Text = lblN.Text; timer.Start(); break; }
                        case "txtNSa": { txtNSayisi.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "lblNok": { lblNoktaS.Text = IstemciString.Substring(7); Text2.Text = txtHGun.Text; timer.Start(); break; }
                        case "lblG11": { lblG1.Text = IstemciString.Substring(7); txtSayTip.Text = "S"; timer.Start(); break; }
                        case "lblG22": { lblG2.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "lblASE": { lblASEV.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "lblPSS": { lblPS.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "lblPAA": { lblPA.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtDnH": { txtDonemHar.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtDnT": { txtDonemTuk.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtDnm": { txtDonem.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtDnG": { txtDonemGun.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtAKd": { txtAKdm.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKK1": { txtKK11.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKK2": { txtKK21.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKK3": { txtKK31.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKK4": { txtKK41.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKd1": { txtKademe1.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKd2": { txtKademe2.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtKd3": { txtKademe3.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtEkr": { txtEkran.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtEk1": { txtEkran1.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtPVS": { txtPerVanaSaat.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtPVG": { txtPerVanaGun.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtVNP": { txtVanaPer.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtVKT": { txtVKT.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtVAT": { txtVAT.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "txtVKS": { txtVKSay.Text = IstemciString.Substring(7); timer.Start(); break; }
                        case "VanDur":
                            {
                                vanadurum = IstemciString.Substring(7);
                                if (vanadurum == "true")
                                {
                                    ellipse.Fill = Brushes.Lime;
                                    txtvandur.Text = "AÇIK";
                                }
                                else
                                {
                                    ellipse.Fill = Brushes.Red;
                                    txtvandur.Text = "KAPALI";
                                }
                                timer.Start();
                                break;
                            }
                        default: { loglst.Items.Add(IstemciString); break; }
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP HATA", "TCP Paketi OKUMA Hatası");
            }
        }
        public string IstemciString;

        public byte[] IssuE = new byte[2];
        public byte[] BuF = new byte[1024];
        public byte[] OutBuF = new byte[64];
        public string txtKomutIssue = "DX";
        public string txtKomutNoktaSayi = "3";
        public string txtKomutYKKrd = "1000";
        public string txtKomutAyar = "0";
        public string vanadurum;

        public SerialPort cport = new SerialPort();
        public DispatcherTimer timer = new DispatcherTimer();
        public DispatcherTimer timerbtn = new DispatcherTimer();
        public TcpClient Istemci = new TcpClient();
        NetworkStream AgAkimi;
        StreamWriter AkimYazici;
        StreamReader AkimOkuyucu;

        public void Ekrani_Temizle()
        {
            ellipse.Fill = Brushes.Transparent;
            txtFlag0.Text = string.Empty;
            txtFlag2.Text = string.Empty;
            txtSayTip.Text = string.Empty;
            txtIssue.Text = string.Empty;
            txtNoktaSayisi.Text = string.Empty;
            txtHarcananKredi.Text = string.Empty;
            txtCihazNo.Text = string.Empty;
            txtKalanKredi.Text = string.Empty;
            txtSaat.Text = string.Empty;
            txtTarih.Text = string.Empty;
            txtMekanik.Text = string.Empty;
            txtNSayisi.Text = string.Empty;
            txtHGun.Text = string.Empty;
            txtCNo.Text = string.Empty;
            txtKalan.Text = string.Empty;
            txtHarcananKrd.Text = string.Empty;
            txtHarcanan.Text = string.Empty;
            txtHarcananTers.Text = string.Empty;
            txtKritik.Text = string.Empty;
            txtTarihSaat.Text = string.Empty;
            Text2.Text = string.Empty;
            txtCT.Text = string.Empty;
            txtAT.Text = string.Empty;
            txtResetT.Text = string.Empty;
            txtVKT.Text = string.Empty;
            txtVAT.Text = string.Empty;
            txtVKSay.Text = string.Empty;
            s.Text = string.Empty;
            txtPerVanaSaat.Text = string.Empty;
            txtPerVanaGun.Text = string.Empty;
            txtVanaPer.Text = string.Empty;
            txtEkran.Text = string.Empty;
            txtEkran1.Text = string.Empty;
            txtKK11.Text = string.Empty;
            txtKK21.Text = string.Empty;
            txtKK31.Text = string.Empty;
            txtKK41.Text = string.Empty;
            txtKademe1.Text = string.Empty;
            txtKademe2.Text = string.Empty;
            txtKademe3.Text = string.Empty;
            txtDonem.Text = string.Empty;
            txtDonemHar.Text = string.Empty;
            txtDonemTuk.Text = string.Empty;
            txtDonemGun.Text = string.Empty;
            txtAKdm.Text = string.Empty;
            lblG1.Text = string.Empty;
            lblG2.Text = string.Empty;
            lblPA.Text = string.Empty;
            lblPS.Text = string.Empty;
            lblN.Text = string.Empty;
            lblNoktaS.Text = string.Empty;
            lblIssue.Text = string.Empty;
            lblASEV.Text = string.Empty;
            txtvandur.Text = string.Empty;
            loglst.Items.Clear();
        }

        private async void cmdOku_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bagla.IsEnabled != true)
                {
                    btnwrap.IsEnabled = false;
                    if (txtKomutCihazNo.Text != "" || txtKomutCihazNo.Text.Length >= 1)
                    {
                        Ekrani_Temizle();
                        loglst.Items.Add("Kısa Okuma için Haberleşme Başladı");
                        AkimYazici.WriteLine("KO-" + txtKomutCihazNo.Text);
                        AkimYazici.Flush();
                        timer.Start();
                        timerbtn.Start();
                    }
                    else
                    {
                        await this.ShowMessageAsync("Cihaz Seri Numarasını Kontrol Ediniz", "HATA");
                        timerbtn.Start();
                    }
                }
                else
                {
                    txtIP.Focus();
                    await this.ShowMessageAsync("TCP Hata", "Bağlantı Sağlandıktan Sonra Komutları Yollayabilirsiniz.");
                    txtIP.Background = Brushes.OrangeRed;
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP Hata", "Sunucu ile Bağlantı Koptu.");
                timerbtn.Start();
            }
        }

        private async void cmduzunoku_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bagla.IsEnabled != true)
                {
                    btnwrap.IsEnabled = false;
                    if (txtKomutCihazNo.Text != "" || txtKomutCihazNo.Text.Length >= 1)
                    {
                        Ekrani_Temizle();
                        loglst.Items.Add("Uzun Okuma için Haberleşme Başladı");
                        AkimYazici.WriteLine("UO-" + txtKomutCihazNo.Text);
                        AkimYazici.Flush();
                        timer.Start();
                        timerbtn.Start();
                    }
                    else
                    {
                        await this.ShowMessageAsync("Cihaz Seri Numarasını Kontrol Ediniz", "HATA");
                        timerbtn.Start();
                    }
                }
                else
                {
                    txtIP.Focus();
                    await this.ShowMessageAsync("TCP Hata", "Bağlantı Sağlandıktan Sonra Komutları Yollayabilirsiniz.");
                    txtIP.Background = Brushes.OrangeRed;
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP Hata", "Sunucu ile Bağlantı Koptu");
                timerbtn.Start();
            }

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Ekrani_Temizle();
            IssuE[0] = Encoding.Default.GetBytes("D")[0];
            IssuE[1] = Encoding.Default.GetBytes("X")[0];
        }

        private async void vana_ac_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bagla.IsEnabled != true)
                {
                    btnwrap.IsEnabled = false;
                    Ekrani_Temizle();
                    loglst.Items.Add("Haberleşiyor Vana Açılıyor");
                    AkimYazici.WriteLine("VA-" + txtKomutCihazNo.Text);
                    AkimYazici.Flush();
                    timer.Start();
                    timerbtn.Start();
                }
                else
                {
                    txtIP.Focus();
                    await this.ShowMessageAsync("TCP Hata", "Bağlantı Sağlandıktan Sonra Komutları Yollayabilirsiniz.");
                    txtIP.Background = Brushes.OrangeRed;
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP Hata", "Sunucu ile Bağlantı Koptu");
                timerbtn.Start();
            }
        }

        private async void vana_kapa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bagla.IsEnabled != true)
                {
                    btnwrap.IsEnabled = false;
                    Ekrani_Temizle();
                    loglst.Items.Add("Haberleşiyor Vana Kapanıyor");
                    AkimYazici.WriteLine("VK-" + txtKomutCihazNo.Text);
                    AkimYazici.Flush();
                    timer.Start();
                    timerbtn.Start();
                }
                else
                {
                    txtIP.Focus();
                    await this.ShowMessageAsync("TCP Hata", "Bağlantı Sağlandıktan Sonra Komutları Yollayabilirsiniz.");
                    txtIP.Background = Brushes.OrangeRed;
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP Hata", "Sunucu ile Bağlantı Koptu");
                timerbtn.Start();
            }
        }

        public async void kredi_yukle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bagla.IsEnabled != true)
                {
                    btnwrap.IsEnabled = false;
                    Ekrani_Temizle();
                    loglst.Items.Add("Haberleşiyor Kredi Yükleme");
                    AkimYazici.WriteLine("KY-" + txtKomutCihazNo.Text + txtKomutYukKrd.Text);
                    AkimYazici.Flush();
                    timer.Start();
                    timerbtn.Start();
                }
                else
                {
                    txtIP.Focus();
                    await this.ShowMessageAsync("TCP Hata", "Bağlantı Sağlandıktan Sonra Komutları Yollayabilirsiniz.");
                    txtIP.Background = Brushes.OrangeRed;
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP Hata", "Sunucu ile Bağlantı Koptu");
                timerbtn.Start();
            }
        }

        public async void kredi_iade_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bagla.IsEnabled != true)
                {
                    btnwrap.IsEnabled = false;
                    Ekrani_Temizle();
                    loglst.Items.Add("Haberlesiyor Kredi Iade Komut Gonder");
                    AkimYazici.WriteLine("KI-" + txtKomutCihazNo.Text);
                    AkimYazici.Flush();
                    timer.Start();
                    timerbtn.Start();
                }
                else
                {
                    txtIP.Focus();
                    await this.ShowMessageAsync("TCP Hata", "Bağlantı Sağlandıktan Sonra Komutları Yollayabilirsiniz.");
                    txtIP.Background = Brushes.OrangeRed;
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("TCP Hata", "Sunucu ile Bağlantı Koptu");
                timerbtn.Start();
            }
        }

        private void ekran_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow_Loaded(this, null);
        }

        private async void bagla_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtIP.Text.Length > 7)
                {
                    Istemci = new TcpClient();
                    Istemci.ReceiveTimeout = 8000;
                    Istemci.SendTimeout = 8000;
                    Istemci.Connect(IPAddress.Parse(txtIP.Text), 1234);
                    AgAkimi = Istemci.GetStream();
                    AkimYazici = new StreamWriter(AgAkimi);
                    AkimOkuyucu = new StreamReader(AgAkimi);
                    baglankes.IsEnabled = true;
                    bagla.IsEnabled = false;
                    await this.ShowMessageAsync("TCP/IP", "IP Bağlantısı Başarılı");
                    txtIP.Background = Brushes.White;
                }
                else
                    await this.ShowMessageAsync("TCP/IP HATA", "Geçersiz IP Adresi");
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("HATA", "TCP Paketi Yollama Hatası\n" + ex.ToString());
                timerbtn.Start();
                bagla.IsEnabled = true;
            }
        }

        private void baglankes_Click(object sender, RoutedEventArgs e)
        {
            baglankes.IsEnabled = false;
            bagla.IsEnabled = true;
            AgAkimi.Close();
        }
    }
}
