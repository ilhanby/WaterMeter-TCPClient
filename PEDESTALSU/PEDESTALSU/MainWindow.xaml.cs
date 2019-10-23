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

namespace PEDESTALSU
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            cport.BaudRate = 9600;
            cport.Parity = Parity.None;
            cport.DataBits = 8;
            cport.StopBits = StopBits.One;
            cport.Handshake = Handshake.None;
            cport.DiscardNull = false;
            cport.RtsEnable = false;
            cport.DtrEnable = true;
            cport.ParityReplace.ToString("?");
            cport.ReadBufferSize = 1024;
            cport.WriteBufferSize = 1024;
            txtKomutCihazNo.Text = "6011691";

            comcmb.ItemsSource = SerialPort.GetPortNames();

            txtKomutIssue.Text = "DX";
            txtKomutNoktaSayi.Text = "3";
            txtKomutYKKrd.Text = "1000";
            txtKomutAyar.Text = "0";
            txtKomutYukKrd.Text = "1000";
            IssuE[0] = Encoding.Default.GetBytes("D")[0];
            IssuE[1] = Encoding.Default.GetBytes("X")[0];

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(4000);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            btnwrap.IsEnabled = true;
            timer.Stop();
        }

        public long Is_Ok;
        public long Is_Ht;
        public byte Basar;
        public string SfR;
        public long SayCNo;
        public byte[] IssuE = new byte[2];
        public byte[] Gel_Numeric = new byte[1025];
        public string GelData1, GelData2;
        public string SDurum;
        public byte SHata;
        public long d_lensu_Kisa, d_lensu_Uzun;
        public int[] MaP_Other = new int[16];
        public int GPPort;
        public string StartChar;
        public byte[] BuF = new byte[1024];
        public byte[] OutBuF = new byte[64];
        public long CS;
        public long NBytE;
        public long GelDatSay;

        public SerialPort cport = new SerialPort();
        DispatcherTimer timer = new DispatcherTimer();

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
            SHata = 0;
            loglst.Items.Clear();
        }

        public async void BaglantiKontrol()
        {
            try
            {
                if (comcmb.SelectedItem != null)
                {
                    if (!cport.IsOpen)
                    {
                        cport.Open();
                    }
                }
                else
                {
                    await this.ShowMessageAsync("HATA", "Communication Seçiniz");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("HATA", "Bağlantı Hatası");
                return;
            }
        }

        public void Uyari_Ver(string sf)
        {
            if (SHata == 0)
            {
                Is_Ok = Is_Ok + 1;
                loglst.Items.Add(sf + " TAMAM");
            }
            else
            {
                Is_Ht = Is_Ht + 1;
                loglst.Items.Add(SDurum);
            }
        }

        private async void cmdOku_Click(object sender, RoutedEventArgs e)
        {
            BaglantiKontrol();
            if (!cport.IsOpen)
            {
                return;
            }
            btnwrap.IsEnabled = false;
            if (txtKomutCihazNo.Text != "" || txtKomutCihazNo.Text.Length >= 1)
            {
                Ekrani_Temizle();
                loglst.Items.Add("Kısa Okuma için Haberleşme Başladı");

                UyandirmaGonder(false);

                if (SHata == 0)
                {
                    Thread.Sleep(500);
                    Komut_Gonder("O", 0, "", 32, 5000);
                }
                if (SHata == 0)
                {
                    GelenKisaBilgiDoldur();
                }
                Uyari_Ver("KISA OKUMA");
                ClearReadBuffer(cport);
                timer.Start();
            }
            else
            {
                await this.ShowMessageAsync("Cihaz Seri Numarasını Kontrol Ediniz", "HATA");
                timer.Start();
            }
        }

        private async void cmduzunoku_Click(object sender, RoutedEventArgs e)
        {
            BaglantiKontrol();
            if (!cport.IsOpen)
            {
                return;
            }
            btnwrap.IsEnabled = false;
            if (txtKomutCihazNo.Text != "" || txtKomutCihazNo.Text.Length >= 1)
            {
                Ekrani_Temizle();
                loglst.Items.Add("Uzun Okuma için Haberleşme Başladı");
                UyandirmaGonder(false);
                if (SHata == 0)
                {
                    Thread.Sleep(500);
                    Komut_Gonder("Z", 0, "", 256, 5000);
                }
                if (SHata == 0)
                {
                    GelenUzunBilgiDoldur();
                }
                Uyari_Ver("UZUN OKUMA");
                ClearReadBuffer(cport);
                timer.Start();
            }
            else
            {
                await this.ShowMessageAsync("Cihaz Seri Numarasını Kontrol Ediniz", "HATA");
                timer.Start();
            }
        }

        public void UyandirmaGonder(bool SfrKontrol)
        {
            string GidenData;
            for (int i = 1; i <= 1024; i++)
            {
                Gel_Numeric[i] = 0;
            }
            ClearReadBuffer(cport);
            Long2Byte(Convert.ToInt64(txtKomutCihazNo.Text), 0);
            CS = Convert.ToChar(StartChar) ^ OutBuF[0] ^ OutBuF[1] ^ OutBuF[2] ^ OutBuF[3] ^ Convert.ToChar(5) ^ Convert.ToChar("S") ^ OutBuF[0] ^ OutBuF[1] ^ OutBuF[2] ^ OutBuF[3];
            GidenData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)(5) + "S" +
                        ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)(CS % 256);
            SendBuffer(cport, GidenData);
            GelData1 = ReadBuffer(cport, 11, 3000);
            if (GelData1.Length >= 10)
            {
                loglst.Items.Add("Sayac Uyandi...");
                SayCNo = Byte2Long(Encoding.Default.GetBytes(GelData1.Substring(1, 1))[0], Encoding.Default.GetBytes(GelData1.Substring(2, 1))[0], Encoding.Default.GetBytes(GelData1.Substring(3, 1))[0], Encoding.Default.GetBytes(GelData1.Substring(4, 1))[0]);
                if (txtKomutCihazNo.Text != Convert.ToString(SayCNo))
                {
                    loglst.Items.Add("Okunan Sayaç No Hatalı!  - " + System.Convert.ToString(SayCNo));
                    SHata = 2;
                }
                if (SfrKontrol == true)
                {
                    BuF[0] = Encoding.Default.GetBytes(GelData1.Substring(5, 1))[0];
                    BuF[1] = Encoding.Default.GetBytes(GelData1.Substring(6, 1))[0];
                    SfR = Sifrele_RS485(ref BuF);
                }
            }
            else
            {
                SHata = 1;
                loglst.Items.Add("Sayac Uyanamadi!...");
                timer.Start();
                return;
            }
        }

        private void Komut_Gonder(string KoMuT, int KmtLen, string KmtData, int KmtOkuLen, float KmtOkuTimeout)
        {
            string GidenData;
            long KmtCSM;
            if (SHata == 0)
            {
                Long2Byte(Convert.ToInt64(txtKomutCihazNo.Text), 0);
                if (KoMuT.Length != 0)
                {
                    CS = Convert.ToChar(StartChar) ^ OutBuF[0] ^ OutBuF[1] ^ OutBuF[2] ^ OutBuF[3] ^ (KmtLen + 3);
                    KmtCSM = Convert.ToChar(KoMuT) + KmtLen;
                    CS = CS ^ Convert.ToChar(KoMuT) ^ KmtLen;
                }
                else
                {
                    CS = Convert.ToChar(StartChar) ^ OutBuF[0] ^ OutBuF[1] ^ OutBuF[2] ^ OutBuF[3] ^ (KmtLen + 2);
                    KmtCSM = KmtLen;
                    CS = CS ^ KmtLen;
                }
                if (KmtLen > 0)
                {
                    if (KmtData.Length != KmtLen)
                    {
                        SHata = 50;
                        loglst.Items.Add("Komut Data Uzunluğu Hatalı!");
                        return;
                    }
                    for (int i = 1; i <= KmtLen; i++)
                    {
                        KmtCSM = KmtCSM + Encoding.Default.GetBytes(KmtData.Substring(i - 1, 1))[0];
                        CS = CS ^ Encoding.Default.GetBytes(KmtData.Substring(i - 1, 1))[0];
                    }
                }
                KmtCSM = (KmtCSM % 256) ^ 255;
                CS = (CS ^ KmtCSM) % 256;
                GidenData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + ((char)(KmtLen + 3)) + KoMuT + ((char)KmtLen) + KmtData + (Encoding.Default.GetString(new byte[] { Convert.ToByte(KmtCSM) })) + (Encoding.Default.GetString(new byte[] { Convert.ToByte(CS % 256) }));
                SendBuffer(cport, GidenData);
                GelData2 = ReadBuffer(cport, KmtOkuLen, KmtOkuTimeout);
                if (GelData2.Length >= KmtOkuLen)
                {
                    loglst.Items.Add("Sayac Okundu...");
                    for (int i = 1; i <= GelData2.Length; i++)
                    {
                        Gel_Numeric[i - 1] = Encoding.Default.GetBytes(GelData2.Substring(i - 1, 1))[0];
                    }

                    if (KoMuT == "V" && KmtData == "A" || KmtData == "K")
                    {
                        if (Gel_Numeric[0] != Convert.ToByte("T"))
                        {
                            loglst.Items.Add("Vana Komutuna T Dönmedi. " + GelData2);
                            SHata = 4;
                        }
                    }
                    if (KoMuT == "Y" && KmtData == "I")
                    {
                        if (Gel_Numeric[0] != Convert.ToByte("T"))
                        {
                            loglst.Items.Add("Kredi Iade Komutuna T Dönmedi. " + GelData2);
                            SHata = 5;
                        }
                    }
                }
                else
                {
                    SHata = 3;
                    loglst.Items.Add("Yanıt Gelmedi!");
                }
            }
        }

        public void ClearReadBuffer(SerialPort cport)
        {
            cport.DiscardInBuffer();
            cport.DiscardOutBuffer();
        }

        public void SendBuffer(SerialPort cport, string s)
        {
            byte[] bytes = Encoding.Default.GetBytes(s);
            cport.Write(bytes, 0, bytes.Length);
        }

        public string ReadBuffer(SerialPort cport, int N, float tout)
        {
            string reader;
            double T1, T2;
            tout = tout / 1000;
            T1 = DateTime.Now.TimeOfDay.TotalSeconds;
            T2 = DateTime.Now.TimeOfDay.TotalSeconds;
            while (!(cport.BytesToRead > N - 1 || (T2 - T1) > tout))
            {
                T2 = DateTime.Now.TimeOfDay.TotalSeconds;
            }
            byte[] comBuffer = new byte[cport.BytesToRead];
            cport.Read(comBuffer, 0, comBuffer.Length);
            reader = Encoding.Default.GetString(comBuffer);
            return reader;
        }

        public bool NibbleSec(string H)
        {
            bool tempNibbleSec = true;
            switch (H)
            {
                case "0":
                    {
                        NBytE = 0;
                        break;
                    }
                case "1":
                    {
                        NBytE = 1;
                        break;
                    }
                case "2":
                    {
                        NBytE = 2;
                        break;
                    }
                case "3":
                    {
                        NBytE = 3;
                        break;
                    }
                case "4":
                    {
                        NBytE = 4;
                        break;
                    }
                case "5":
                    {
                        NBytE = 5;
                        break;
                    }
                case "6":
                    {
                        NBytE = 6;
                        break;
                    }
                case "7":
                    {
                        NBytE = 7;
                        break;
                    }
                case "8":
                    {
                        NBytE = 8;
                        break;
                    }
                case "9":
                    {
                        NBytE = 9;
                        break;
                    }
                case "A":
                    {
                        NBytE = 10;
                        break;
                    }
                case "B":
                    {
                        NBytE = 11;
                        break;
                    }
                case "C":
                    {
                        NBytE = 12;
                        break;
                    }
                case "D":
                    {
                        NBytE = 13;
                        break;
                    }
                case "E":
                    {
                        NBytE = 14;
                        break;
                    }
                case "F":
                    {
                        NBytE = 15;
                        break;
                    }
                default:
                    tempNibbleSec = false;
                    break;
            }
            return tempNibbleSec;
        }

        public byte Hex2Byte(string H)
        {
            byte tempHex2Byte = 0;
            H = H.ToUpper();
            switch (H.Length)
            {
                case 0:
                    {
                        CS = 0;
                        break;
                    }
                case 1:
                    {
                        if (!NibbleSec(H))
                        {
                            return tempHex2Byte;
                        }
                        CS = Convert.ToInt64(NBytE);
                        break;
                    }
                default:
                    {
                        if (!NibbleSec(H.Substring(0, 1)))
                        {
                            return tempHex2Byte;
                        }
                        CS = Convert.ToInt64(NBytE * 16);
                        if (!NibbleSec(H.Substring(1, 1)))
                        {
                            return tempHex2Byte;
                        }
                        CS = CS + Convert.ToInt64(NBytE);
                        break;
                    }
            }
            tempHex2Byte = Convert.ToByte(CS % 256);
            return tempHex2Byte;
        }

        public long Byte2Long(byte b0, byte b1, byte b2, byte b3)
        {
            long functionReturnValue = 0;
            if (b3 > 127)
            {
                CS = b3 - 128;
                functionReturnValue = Convert.ToInt64(CS * 16777216.0 + b2 * 65536.0 + b1 * 256.0 + b0);
                functionReturnValue = Convert.ToInt64(functionReturnValue - 2147483648.0);
            }
            else
            {
                functionReturnValue = Convert.ToInt64(b3 * 16777216.0 + b2 * 65536.0 + b1 * 256.0 + b0);
            }
            CS = (long)b0 + (long)b1 + (long)b2 + (long)b3;
            CS = (CS % 256) ^ 255;
            return functionReturnValue;
        }

        public void Long2Byte(long L, int ST)
        {
            byte B;
            if (L == 0)
            {
                OutBuF[ST] = 0; OutBuF[ST + 1] = 0; OutBuF[ST + 2] = 0; OutBuF[ST + 3] = 0; CS = 0xFF;
                return;
            }
            if (L < 0)
            {
                L = 2147483648 + L;
                B = Convert.ToByte(L / 16777216);
                OutBuF[ST + 3] = Convert.ToByte(B + 0x80);
                L = L % 16777216;
            }
            else
            {
                B = Convert.ToByte(L / 16777216);
                OutBuF[ST + 3] = B;
                L = L % 16777216;
            }
            B = Convert.ToByte(L / 65536);
            OutBuF[ST + 2] = B;
            L = L % 65536;
            B = Convert.ToByte(L / 256);
            OutBuF[ST + 1] = B;
            OutBuF[ST] = Convert.ToByte(L % 256);
            CS = Convert.ToInt64(OutBuF[ST]) + Convert.ToInt64(OutBuF[ST + 1]) + Convert.ToInt64(OutBuF[ST + 2]) + Convert.ToInt64(OutBuF[ST + 3]);
            CS = (CS % 256) ^ 255;
        }

        public string Byte2DateL(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5)
        {
            string tempbyte2date;
            tempbyte2date = b3.ToString() + "/" + b4.ToString() + "/" + (Convert.ToInt32(b5) + 2000) + "  " + b2.ToString() + " : " + b1.ToString() + " : " + b0.ToString();
            CS = Convert.ToInt64(b0) + Convert.ToInt64(b1) + Convert.ToInt64(b2) + Convert.ToInt64(b3) + Convert.ToInt64(b4) + Convert.ToInt64(b5);
            CS = (CS % 256) ^ 255;
            return tempbyte2date;
        }

        private string Sifrele_RS485(ref byte[] sy_RF)
        {
            string sifrele;
            int sum, A;
            int[] Out = new int[4];
            int[] sYn = new int[4];
            int[] sy = new int[4];

            sifrele = "";
            sy[0] = (sy_RF[0] % 16) + 0x30;
            sy[1] = (sy_RF[0] - (sy_RF[0] % 16)) / 16 + 0x30;
            sy[2] = (sy_RF[1] % 16) + 0x30;
            sy[3] = (sy_RF[1] - (sy_RF[1] % 16)) / 16 + 0x30;

            sum = 0;
            for (int i = 0; i <= 3; i++)
            {
                sYn[i] = sy[i];
                sum = (sum + sYn[i]) % 256;
                Out[i] = sum % 256;
            }

            for (int i = 0; i < 3; i++)
            {
                sYn[i] = (sYn[i] + sum + MaP_Other[sum % 16] + Out[i]) % 256;

                A = (i == 3) ? 0 : i + 1;

                Out[i] = sYn[i] + (sYn[A] ^ MaP_Other[sYn[i] % 16]) + sy[i];

                sum = sum + Out[i];
            }

            for (int i = 0; i <= 3; i++)
            {
                Out[i] = Out[i] % 256;
                sifrele = sifrele + Byte2Hex(Convert.ToByte(Out[i]));
            }
            return sifrele;
        }

        public string Byte2Hex(byte data)
        {
            int a = data / 16; int b = data % 16;
            string hexValue = a.ToString("X");
            string hexValues = b.ToString("X");
            return hexValue + hexValues;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Ekrani_Temizle();
            d_lensu_Kisa = 32;
            d_lensu_Uzun = 272;
            StartChar = "P";
            MaP_Other[0] = 0x38;
            MaP_Other[1] = 0xF6;
            MaP_Other[2] = 0x29;
            MaP_Other[3] = 0xB2;
            MaP_Other[4] = 0x50;
            MaP_Other[5] = 0xB;
            MaP_Other[6] = 0x14;
            MaP_Other[7] = 0xEA;
            MaP_Other[8] = 0x61;
            MaP_Other[9] = 0x43;
            MaP_Other[10] = 0xDF;
            MaP_Other[11] = 0xA5;
            MaP_Other[12] = 0xC7;
            MaP_Other[13] = 0x7E;
            MaP_Other[14] = 0x9C;
            MaP_Other[15] = 0x8D;
        }

        private void vana_ac_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BaglantiKontrol();
                if (!cport.IsOpen)
                {
                    return;
                }
                btnwrap.IsEnabled = false;
                string KomutData;
                Ekrani_Temizle();
                loglst.Items.Add("Haberleşiyor Vana Açılıyor");
                UyandirmaGonder(true);
                if (SHata == 0)
                {
                    KomutData = "A" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                    Thread.Sleep(500);
                    Komut_Gonder("V", KomutData.Length, KomutData, 1, 3000);
                }
                Uyari_Ver("VANA ACMA");
                ClearReadBuffer(cport);
                timer.Start();
            }
            catch (Exception)
            {
                loglst.Items.Add("VANA AÇILAMADI...");
                timer.Start();
            }
        }

        private void vana_kapa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BaglantiKontrol();
                if (!cport.IsOpen)
                {
                    return;
                }
                btnwrap.IsEnabled = false;
                string KomutData;
                Ekrani_Temizle();
                loglst.Items.Add("Haberleşiyor Vana Kapanıyor");
                UyandirmaGonder(true);
                if (SHata == 0)
                {
                    KomutData = "K" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                    Thread.Sleep(500);
                    Komut_Gonder("V", KomutData.Length, KomutData, 1, 3000);
                }
                Uyari_Ver("VANA KAPAT ");
                ClearReadBuffer(cport);
                timer.Start();
            }
            catch (Exception)
            {
                loglst.Items.Add("VANA KAPATILAMADI...");
                timer.Start();
            }
        }

        public void kredi_yukle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BaglantiKontrol();
                if (!cport.IsOpen)
                {
                    return;
                }
                btnwrap.IsEnabled = false;
                string KomutData;
                string GelData;
                int[] KKatH = new int[4];
                byte[] KKatL = new byte[4];
                long[] KadM = new long[3];
                int Dnm;
                long Cx;

                KKatH[0] = 1;
                KKatL[0] = 0;

                KKatH[1] = 1;
                KKatL[1] = 0;

                KKatH[2] = 1;
                KKatL[2] = 0;

                KKatH[3] = 1;
                KKatL[3] = 0;

                KadM[0] = 10000;
                KadM[1] = 20000;
                KadM[2] = 30000;

                Dnm = 30;

                Ekrani_Temizle();
                loglst.Items.Add("Haberleşiyor Kredi Yükleme");
                UyandirmaGonder(true);

                KomutData = "Y" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                // yüklenen kredi
                Cx = (Hex2Byte(SfR.Substring(2, 2)) + 1) % 256;
                Long2Byte(Convert.ToInt64(txtKomutYukKrd.Text), 0);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[0])) });
                Cx = ((Cx + 1) % 256);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[1])) });
                Cx = ((Cx + 1) % 256);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[2])) });
                Cx = ((Cx + 1) % 256);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[3])) });
                // ayar
                KomutData = KomutData + ((char)(Convert.ToInt64(txtKomutAyar.Text) % 256));
                // nokta sayısı
                switch (txtKomutNoktaSayi.Text)
                {
                    case "0":
                        Cx = 226;
                        break;
                    case "1":
                        Cx = 230;
                        break;
                    case "2":
                        Cx = 234;
                        break;
                    default:
                        Cx = 238;
                        break;
                }
                KomutData = KomutData + ((char)Cx);
                //Kritik kredi
                Long2Byte(Convert.ToInt64(txtKomutYKKrd.Text), 0);
                KomutData = KomutData + ((char)OutBuF[0]);
                KomutData = KomutData + ((char)OutBuF[1]);
                KomutData = KomutData + ((char)OutBuF[2]);
                if (SHata == 0)
                {
                    Komut_Gonder("Y", KomutData.Length, KomutData, 1, 3000);
                }
                else
                {
                    timer.Start();
                    return;
                }

                Long2Byte(Convert.ToInt64(txtKomutCihazNo.Text), 0);
                KomutData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)22 + (char)20;
                //tarife değiştir veya değiştirme
                KomutData = KomutData + (char)(201); //buradaki değer 201 ise tarifeleri yükler, değilse tarifeler değiştirilmez.
                                                     //kredi katsayı 1
                Cx = KKatH[0] % 256;
                KomutData = KomutData + ((char)Cx);
                KomutData = KomutData + ((char)((KKatH[0] - Cx) / 256));
                KomutData = KomutData + ((char)KKatL[0]);
                //kredi katsayı 2
                Cx = KKatH[1] % 256;
                KomutData = KomutData + ((char)Cx);
                KomutData = KomutData + ((char)((KKatH[1] - Cx) / 256));
                KomutData = KomutData + ((char)KKatL[1]);
                //kredi katsayı 3
                Cx = KKatH[2] % 256;
                KomutData = KomutData + ((char)Cx);
                KomutData = KomutData + ((char)((KKatH[2] - Cx) / 256));
                KomutData = KomutData + ((char)(KKatL[2]));
                //kredi katsayı 4
                Cx = KKatH[3] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KKatH[3] - Cx) / 256));
                KomutData = KomutData + ((char)(KKatL[3]));
                //donem
                Cx = Dnm % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((Dnm - Cx) / 256));
                //kademe 1
                Cx = KadM[0] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KadM[0] - Cx) / 256));
                //kademe 2
                Cx = KadM[1] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KadM[1] - Cx) / 256));
                //kademe 3
                Cx = KadM[2] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KadM[2] - Cx) / 256));

                CS = 0;
                for (int i = 0; i < KomutData.Length; i++)
                {
                    CS = CS ^ Encoding.Default.GetBytes(KomutData.Substring(i, 1))[0];
                }

                KomutData = KomutData + (char)(CS % 256);
                if (SHata == 0)
                {
                    Thread.Sleep(500);
                    SendBuffer(cport, KomutData);
                    GelData = ReadBuffer(cport, 5, 3000);

                    if (GelData.Length >= 5)
                    {
                        if (GelData.Substring(0, 1) != "T")
                        {
                            SHata = 200;
                            loglst.Items.Add("2. T Paketi Gelmedi!");
                            Uyari_Ver("KREDI YUKLE ");
                            timer.Start();
                            return;
                        }

                        Cx = Byte2Long(Encoding.Default.GetBytes(GelData.Substring(1, 1))[0], Encoding.Default.GetBytes(GelData.Substring(2, 1))[0], Encoding.Default.GetBytes(GelData.Substring(3, 1))[0], Encoding.Default.GetBytes(GelData.Substring(4, 1))[0]);
                        if (Cx != Convert.ToInt64(txtKomutYukKrd.Text))
                        {
                            loglst.Items.Add("Yuklenen ile Gelen Kredi Aynı Değil! " + Convert.ToString(Cx));
                            SHata = 202;
                            Uyari_Ver("KREDI YUKLE ");
                            timer.Start();
                            return;
                        }

                        for (var i = 0; i < GelData.Length; i++)
                        {
                            Gel_Numeric[i] = Encoding.Default.GetBytes(GelData.Substring(i, 1))[0];
                        }
                    }
                    else
                    {
                        SHata = 203;
                        loglst.Items.Add("2. T Paketi Okunamadı!");
                        Uyari_Ver("KREDI YUKLE ");
                        timer.Start();
                        return;
                    }
                }
                else
                {
                    timer.Start();
                    return;
                }

                Long2Byte(Convert.ToInt64(txtKomutCihazNo.Text), 0);
                KomutData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)2 + "OK";
                CS = 0;
                for (var i = 1; i <= KomutData.Length; i++)
                {
                    CS = CS ^ Encoding.Default.GetBytes(KomutData.Substring(i - 1, 1))[0];
                }
                KomutData = KomutData + (char)(CS % 256);
                Thread.Sleep(500);
                SendBuffer(cport, KomutData);
                GelData = ReadBuffer(cport, 1, 3000);

                if (GelData.Length >= 1)
                {
                    if (GelData.Substring(0, 1) != "T")
                    {
                        SHata = 204;
                        loglst.Items.Add("3. T Gelmedi!");
                        Uyari_Ver("KREDI YUKLE ");
                        timer.Start();
                        return;
                    }
                }
                else
                {
                    SHata = 205;
                    loglst.Items.Add("3. T Paketi Okunamadi!");
                    Uyari_Ver("KREDI YUKLE ");
                    timer.Start();
                    return;
                }

                Long2Byte(Convert.ToInt64(txtKomutCihazNo.Text), 0);
                KomutData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)1 + "O";
                CS = 0;
                for (int i = 0; i <= KomutData.Length - 1; i++)
                {
                    CS = CS ^ Encoding.Default.GetBytes(KomutData.Substring(i, 1))[0];
                }
                KomutData = KomutData + (char)(CS % 256);
                Thread.Sleep(500);
                SendBuffer(cport, KomutData);

                GelData = ReadBuffer(cport, 32, 3000);
                if (GelData.Length >= 32)
                {
                    for (int i = 0; i < GelData.Length; i++)
                    {
                        Gel_Numeric[i] = Encoding.Default.GetBytes(GelData.Substring(i, 1))[0];
                    }
                }
                else
                {
                    loglst.Items.Add("Kredi Yükleme Başarılı. \nSayaç Okunamadı.");
                    SHata = 207;
                }

                if (SHata == 0)
                {
                    GelenKisaBilgiDoldur();
                    Uyari_Ver("KREDI YUKLE ");
                    ClearReadBuffer(cport);
                }
                timer.Start();
            }
            catch (Exception)
            {
                loglst.Items.Add("KREDI YÜKLENEMEDI");
                ClearReadBuffer(cport);
                timer.Start();
            }
        }

        public void kredi_iade_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BaglantiKontrol();
                if (!cport.IsOpen)
                {
                    return;
                }
                btnwrap.IsEnabled = false;
                string KomutData;
                string GelData;
                Ekrani_Temizle();

                loglst.Items.Add("Haberlesiyor Kredi Iade Komut Gonder");

                UyandirmaGonder(true);

                KomutData = "I" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);

                if (SHata == 0)
                {
                    Thread.Sleep(500);
                    Komut_Gonder("Y", KomutData.Length, KomutData, 6, 3000);
                }
                CS = Gel_Numeric[0] ^ Gel_Numeric[1] ^ Gel_Numeric[2] ^ Gel_Numeric[3] ^ Gel_Numeric[4];
                if ((CS != Gel_Numeric[5]))
                {
                    SHata = 150;
                    loglst.Items.Add("Iade Kredi Paket CSUM Hatalı!");
                    timer.Start();
                    return;
                }

                if (SHata == 0)
                {
                    Long2Byte(Convert.ToInt64(txtKomutCihazNo.Text), 0);
                    KomutData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)6 + (char)20 + (Encoding.Default.GetString(new byte[] { Gel_Numeric[1] })) + (Encoding.Default.GetString(new byte[] { Gel_Numeric[2] })) + (Encoding.Default.GetString(new byte[] { Gel_Numeric[3] })) + (Encoding.Default.GetString(new byte[] { Gel_Numeric[4] })) + Encoding.Default.GetString(new byte[] { (byte)CS });
                    CS = 0;
                    for (int i = 0; i < KomutData.Length; i++)
                    {
                        CS = CS ^ Encoding.Default.GetBytes(KomutData.Substring(i, 1))[0];
                    }
                    KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(CS % 256)) });
                    Thread.Sleep(500);
                    SendBuffer(cport, KomutData);

                    GelData = ReadBuffer(cport, 33, 3000);
                    if ((GelData.Length >= 33))
                    {
                        for (int i = 1; i < GelData.Length; i++)
                        {
                            Gel_Numeric[i - 1] = Encoding.Default.GetBytes(GelData.Substring(i, 1))[0];
                        }
                    }
                    else
                    {
                        loglst.Items.Add("Kredi Iade Edildi. \n\nOkuma Yapılamadı");
                        SHata = 151;
                    }
                    if (SHata == 0)
                    {
                        Uyari_Ver("KREDI IADE ");
                        GelenKisaBilgiDoldur();
                        ClearReadBuffer(cport);
                    }
                }
                timer.Start();
            }
            catch (Exception)
            {
                loglst.Items.Add("KREDI IADE EDILEMEDI");
                ClearReadBuffer(cport);
                timer.Start();
            }
        }

        private void ekran_Click(object sender, RoutedEventArgs e)
        {
            comcmb.ItemsSource = SerialPort.GetPortNames();
            MetroWindow_Loaded(this, null);
        }

        private void comcmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comcmb.Items.Count > 0)
                cport.PortName = comcmb.SelectedItem.ToString();
        }

        public void GelenKisaBilgiDoldur()
        {
            //KISA OKUMA BİLGİLERİ DOLDURMA
            byte[] GData = new byte[50];
            int j = 1;
            CS = 0;
            for (var i = 0; i <= d_lensu_Kisa - 1; i++)
            {
                GData[j] = Gel_Numeric[i];
                CS = CS + Gel_Numeric[i];
                j = j + 1;
            }
            CS = (CS % 256) ^ 255;
            if (CS != Gel_Numeric[d_lensu_Kisa])
            {
                loglst.Items.Add("CSUM HATALI");
                SHata = 101;
                return;
            }
            // sayac tip
            txtSayTip.Text = Convert.ToChar(GData[3]).ToString();
            // issue
            txtIssue.Text = Convert.ToChar(GData[4]) + Convert.ToChar(GData[5]).ToString();
            // nokta sayısı
            txtNoktaSayisi.Text = Byte2Hex(GData[6]);
            switch (txtNoktaSayisi.Text)
            {
                case "E2": { txtNSayisi.Text = "0"; break; }
                case "E6": { txtNSayisi.Text = "1"; break; }
                case "EA": { txtNSayisi.Text = "2"; break; }
                case "EE": { txtNSayisi.Text = "3"; break; }
                default: { txtNSayisi.Text = "?"; break; }
            }
            //flag 0 - flag 2 
            txtFlag0.Text = GData[8].ToString("X");
            txtFlag2.Text = GData[9].ToString("X");
            //VANA DURUMU
            CS = 1;
            for (int i = 0; i < 4; i++)
            {
                CS = CS * 2;
            }
            if (GData[9] == CS)
            {
                ellipse.Fill = Brushes.Lime;
            }
            else
            {
                ellipse.Fill = Brushes.Red;
            }
            // kalan kredi
            txtKalanKredi.Text = Convert.ToString(Byte2Long(GData[17], GData[18], GData[19], GData[20]));
            // harcanan kredi
            txtHarcananKredi.Text = Convert.ToString(Byte2Long(GData[21], GData[22], GData[23], GData[24]));
            // cihaz no
            txtCihazNo.Text = Convert.ToString(SayCNo);
            // saat sn dk ss
            txtSaat.Text = Convert.ToString(GData[12]) + " : " + Convert.ToString(GData[11]) + " : " + Convert.ToString(GData[10]);
            // gn ay yıl
            txtTarih.Text = Convert.ToString(GData[13]) + "/" + Convert.ToString(GData[14]) + "/" + "20" + Convert.ToString(GData[15]);
            // haftanın gunu
            string txtHaftaGun = (GData[16]).ToString();
            switch (txtHaftaGun)
            {
                case "1": { txtHGun.Text = "Pazartesi"; break; }
                case "2": { txtHGun.Text = "Salı"; break; }
                case "3": { txtHGun.Text = "Çarşamba"; break; }
                case "4": { txtHGun.Text = "Perşembe"; break; }
                case "5": { txtHGun.Text = "Cuma"; break; }
                case "6": { txtHGun.Text = "Cumartesi"; break; }
                case "7": { txtHGun.Text = "Pazar"; break; }
                default: { txtHGun.Text = " "; break; }
            }
        }

        public void GelenUzunBilgiDoldur()
        {
            //UZUN OKUMA BİLGİLERİ DOLDURMA
            int Br, i, j;
            byte[] GData = new byte[300];
            txtSayTip.Text = "S";
            j = 0;
            for (i = 0; i < d_lensu_Uzun - 1; i++)
            {
                if (i % 17 != 0)
                {
                    GData[j] = Gel_Numeric[i];
                    j = j + 1;
                }
            }

            Br = 23;
            txtKalan.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            txtKalanKredi.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            if (CS != GData[Br + 4])
            {
                txtKalan.Background = Brushes.DarkRed;
            }
            else
            {
                txtKalan.Background = Brushes.White;
            }

            Br = 48;
            txtHarcananKrd.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            txtHarcananKredi.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            if (CS != GData[Br + 4])
            {
                txtHarcananKrd.Background = Brushes.DarkRed;
            }
            else
            {
                txtHarcananKrd.Background = Brushes.White;
            }

            Br = 39;
            txtCNo.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            txtCihazNo.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            if (CS != GData[Br + 4])
            {
                txtCNo.Background = Brushes.DarkRed;
            }
            else
            {
                txtCNo.Background = Brushes.White;
            }

            Br = 53;
            txtHarcanan.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            if (CS != GData[Br + 4])
            {
                txtHarcanan.Background = Brushes.DarkRed;
            }
            else
            {
                txtHarcanan.Background = Brushes.White;
            }

            Br = 64;
            txtHarcananTers.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            txtMekanik.Text = "0";
            Br = 28;
            txtKritik.Text = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], 0));
            if (CS != GData[Br + 3])
            {
                txtKritik.Background = Brushes.DarkRed;
            }
            else
            {
                txtKritik.Background = Brushes.White;
            }
            /////
            Br = 0;
            txtTarihSaat.Text = Byte2DateL(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3], GData[Br + 4], GData[Br + 5]);
            txtTarih.Text = Convert.ToInt64(GData[Br + 3]) + "/" + Convert.ToInt64(GData[Br + 4]) + "/" + (Convert.ToInt64(GData[Br + 5]) + 2000);
            txtSaat.Text = Convert.ToInt64(GData[Br + 2]) + " : " + Convert.ToInt64(GData[Br + 1]) + " : " + Convert.ToInt64(GData[Br]);
            /////
            Br = 206;
            txtHGun.Text = GData[Br].ToString();
            switch (txtHGun.Text)
            {
                case "1": { txtHGun.Text = "Pazartesi"; break; }
                case "2": { txtHGun.Text = "Salı"; break; }
                case "3": { txtHGun.Text = "Çarşamba"; break; }
                case "4": { txtHGun.Text = "Perşembe"; break; }
                case "5": { txtHGun.Text = "Cuma"; break; }
                case "6": { txtHGun.Text = "Cumartesi"; break; }
                case "7": { txtHGun.Text = "Pazar"; break; }
                default: { txtHGun.Text = " "; break; }
            }
            Text2.Text = txtHGun.Text;
            //VANA DURUMU
            Br = 0;
            txtFlag0.Text = GData[Br + 69].ToString("X");
            txtFlag2.Text = GData[Br + 71].ToString("X");
            CS = 1;
            for (i = 0; i < 4; i++)
            {
                CS = CS * 2;
            }
            if (GData[Br + 71] == CS)
            {
                ellipse.Fill = Brushes.Lime;
            }
            else
            {
                ellipse.Fill = Brushes.Red;
            }

            Br = 16;
            txtResetT.Text = Byte2DateL(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3], GData[Br + 4], GData[Br + 5]);
            if (CS != GData[Br + 6])
            {
                txtResetT.Background = Brushes.DarkRed;
            }
            else
            {
                txtResetT.Background = Brushes.White;
            }

            Br = 8;
            txtCT.Text = GData[Br + 0].ToString() + "/" + GData[Br + 1].ToString() + "/" + (GData[Br + 2] + 2000).ToString();
            CS = Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]);
            CS = CS % 256;
            CS = CS ^ 255;
            if (CS != GData[Br + 3])
            {
                txtCT.Background = Brushes.DarkRed;
            }
            else
            {
                txtCT.Background = Brushes.White;
            }

            Br = 12;
            txtAT.Text = GData[Br + 0].ToString() + "/" + GData[Br + 1].ToString() + "/" + (GData[Br + 2] + 2000).ToString();
            CS = Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]);
            CS = CS % 256;
            CS = CS ^ 255;
            if (CS != GData[Br + 3])
            {
                txtAT.Background = Brushes.DarkRed;
            }
            else
            {
                txtAT.Background = Brushes.White;
            }

            Br = 61;
            lblIssue.Text = Encoding.Default.GetString(new byte[] { GData[Br + 0] }) + Encoding.Default.GetString(new byte[] { GData[Br + 1] });
            txtIssue.Text = lblIssue.Text;
            CS = (((Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;

            Br = 75;
            lblN.Text = GData[Br + 0].ToString("X");
            CS = (((Convert.ToInt64(GData[Br + 0])) % 256) ^ 255) % 256;
            txtNoktaSayisi.Text = lblN.Text;
            switch (txtNoktaSayisi.Text)
            {
                case "E2": { txtNSayisi.Text = "0"; break; }
                case "E6": { txtNSayisi.Text = "1"; break; }
                case "EA": { txtNSayisi.Text = "2"; break; }
                case "EE": { txtNSayisi.Text = "3"; break; }
                default: { txtNSayisi.Text = "?"; break; }
            }
            switch (lblN.Text)
            {
                case "E2": { lblNoktaS.Text = "0"; break; }
                case "E6": { lblNoktaS.Text = "1"; break; }
                case "EA": { lblNoktaS.Text = "2"; break; }
                case "EE": { lblNoktaS.Text = "3"; break; }
                default: { lblNoktaS.Text = lblN.Text; break; }
            }

            Br = 77;
            lblG1.Text = GData[Br + 0].ToString();
            lblG2.Text = GData[Br + 0].ToString();
            CS = (((Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;

            Br = 80;
            lblASEV.Text = GData[Br + 0].ToString();
            CS = (((Convert.ToInt64(GData[Br + 0])) % 256) ^ 255) % 256;

            Br = 82;
            lblPS.Text = GData[Br + 0].ToString("X");
            lblPA.Text = GData[Br + 1].ToString("X");
            CS = (((Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;

            Br = 176;
            txtDonemHar.Text = Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], 0).ToString();
            if (CS != GData[Br + 3])
            {
                txtDonemHar.Background = Brushes.DarkRed;
            }
            else
            {
                txtDonemHar.Background = Brushes.White;
            }

            Br = 180;
            txtDonemTuk.Text = Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], 0).ToString();
            if (CS != GData[Br + 3])
            {
                txtDonemTuk.Background = Brushes.DarkRed;
            }
            else
            {
                txtDonemTuk.Background = Brushes.White;
            }

            Br = 173;
            txtDonem.Text = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            if (CS != GData[Br + 2])
            {
                txtDonem.Background = Brushes.DarkRed;
            }
            else
            {
                txtDonem.Background = Brushes.White;
            }

            Br = 156;
            txtDonemGun.Text = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            txtAKdm.Text = GData[Br + 2].ToString();
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2])) % 256) ^ 255;
            if (CS != GData[Br + 3])
            {
                txtDonemGun.Background = Brushes.DarkRed;
                txtAKdm.Background = Brushes.DarkRed;
            }
            else
            {
                txtDonemGun.Background = Brushes.White;
                txtAKdm.Background = Brushes.White;
            }

            Br = 160;
            txtKK11.Text = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            txtKK21.Text = Byte2Long(GData[Br + 3], GData[Br + 4], 0, 0).ToString();
            txtKK31.Text = Byte2Long(GData[Br + 6], GData[Br + 7], 0, 0).ToString();
            txtKK41.Text = Byte2Long(GData[Br + 9], GData[Br + 10], 0, 0).ToString();
            CS = 0;
            for (i = Br; i < Br + 11; i++)
            {
                CS = CS + Convert.ToInt64(GData[i]);
            }
            CS = (CS % 256) ^ 255;
            if (CS != GData[Br + 12])
            {
                txtKK11.Background = Brushes.DarkRed;
                txtKK21.Background = Brushes.DarkRed;
                txtKK31.Background = Brushes.DarkRed;
                txtKK41.Background = Brushes.DarkRed;
            }
            else
            {
                txtKK11.Background = Brushes.White;
                txtKK21.Background = Brushes.White;
                txtKK31.Background = Brushes.White;
                txtKK41.Background = Brushes.White;
            }

            Br = 149;
            txtKademe1.Text = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            txtKademe2.Text = Byte2Long(GData[Br + 2], GData[Br + 3], 0, 0).ToString();
            txtKademe3.Text = Byte2Long(GData[Br + 4], GData[Br + 5], 0, 0).ToString();
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]) + Convert.ToInt64(GData[Br + 3]) + Convert.ToInt64(GData[Br + 4]) + Convert.ToInt64(GData[Br + 5])) % 256) ^ 255;
            if (CS != GData[Br + 6])
            {
                txtKademe1.Background = Brushes.DarkRed;
                txtKademe2.Background = Brushes.DarkRed;
                txtKademe3.Background = Brushes.DarkRed;
            }
            else
            {
                txtKademe1.Background = Brushes.White;
                txtKademe2.Background = Brushes.White;
                txtKademe3.Background = Brushes.White;
            }

            Br = 87;
            txtEkran.Text = GData[Br].ToString();
            txtEkran1.Text = GData[Br + 1].ToString();
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255;
            if (CS != GData[Br + 2])
            {
                txtEkran.Background = Brushes.DarkRed;
                txtEkran1.Background = Brushes.DarkRed;
            }
            else
            {
                txtEkran.Background = Brushes.White;
                txtEkran1.Background = Brushes.White;
            }

            Br = 90;
            txtPerVanaSaat.Text = GData[Br].ToString();
            txtPerVanaGun.Text = GData[Br + 1].ToString();
            txtVanaPer.Text = GData[Br + 2].ToString();
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2])) % 256) ^ 255;
            if (CS != GData[Br + 3])
            {
                txtPerVanaSaat.Background = Brushes.DarkRed;
                txtPerVanaGun.Background = Brushes.DarkRed;
                txtVanaPer.Background = Brushes.DarkRed;
            }
            else
            {
                txtPerVanaSaat.Background = Brushes.White;
                txtPerVanaGun.Background = Brushes.White;
                txtVanaPer.Background = Brushes.White;
            }

            Br = 138;
            txtVKT.Text = GData[Br + 2].ToString() + "/" + GData[Br + 3].ToString() + "/" + (GData[Br + 4] + 2000).ToString();
            CS = (((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]) + Convert.ToInt64(GData[Br + 3]) + Convert.ToInt64(GData[Br + 4])) % 256) ^ 255) % 256;
            if (CS != GData[Br + 5])
            {
                txtVKT.Background = Brushes.DarkRed;
            }
            else
            {
                txtVKT.Background = Brushes.White;
            }

            Br = 122;
            txtVAT.Text = GData[Br + 2].ToString() + "/" + GData[Br + 3].ToString() + "/" + (GData[Br + 4] + 2000).ToString();
            CS = (((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]) + Convert.ToInt64(GData[Br + 3]) + Convert.ToInt64(GData[Br + 4])) % 256) ^ 255) % 256;
            if (CS != GData[Br + 5])
            {
                txtVAT.Background = Brushes.DarkRed;
            }
            else
            {
                txtVAT.Background = Brushes.White;
            }

            Br = 109;
            txtVKSay.Text = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            CS = (((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;
            if (CS != GData[Br + 2])
            {
                txtVKSay.Background = Brushes.DarkRed;
            }
            else
            {
                txtVKSay.Background = Brushes.White;
            }
        }
    }
}
