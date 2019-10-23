using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace PEDESTALSUSERVICE
{
    public partial class Service1 : ServiceBase
    {
        public SerialPort cport = new SerialPort();
        public System.Timers.Timer timer = new System.Timers.Timer();
        TcpListener TcpDinleyicisi;
        Socket IstemciSoketi;
        NetworkStream AgAkimi;
        StreamWriter AkimYazici;
        StreamReader AkimOkuyucu;
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
        string txtKomutCihazNo;
        string txtKomutNoktaSayi = "3";
        string txtKomutYKKrd = "1000";
        string txtKomutAyar = "0";
        string txtKomutYukKrd = "1000";

        public Service1()
        {
            InitializeComponent();
        }

        public void Basla()
        {
            cport.Handshake = Handshake.None;
            cport.DiscardNull = false;
            cport.RtsEnable = false;
            cport.DtrEnable = true;
            cport.ParityReplace.ToString("?");
            cport.BaudRate = 9600;
            cport.Parity = Parity.None;
            cport.StopBits = StopBits.One;
            cport.DataBits = 8;
            IssuE[0] = Encoding.Default.GetBytes("D")[0];
            IssuE[1] = Encoding.Default.GetBytes("X")[0];

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

            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Interval = 250;

            TcpDinleyicisi = new TcpListener(1234);
            TcpDinleyicisi.Start();

            IstemciSoketi = TcpDinleyicisi.AcceptSocket();

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            if (IstemciSoketi != null)
            {
                if (!IstemciSoketi.Connected)
                    IstemciSoketi = TcpDinleyicisi.AcceptSocket();
                else
                {
                    try
                    {
                        AgAkimi = new NetworkStream(IstemciSoketi);
                        AkimYazici = new StreamWriter(AgAkimi);
                        AkimOkuyucu = new StreamReader(AgAkimi);
                        string IstemciString = AkimOkuyucu.ReadLine();
                        switch (IstemciString.Substring(0, 2))
                        {
                            case "KO":
                                {
                                    txtKomutCihazNo = IstemciString.Substring(3, 7);
                                    ComSor();
                                    cmdOku_Click();
                                    break;
                                }
                            case "UO":
                                {
                                    txtKomutCihazNo = IstemciString.Substring(3, 7);
                                    ComSor();
                                    cmduzunoku_Click();
                                    break;
                                }
                            case "VA":
                                {
                                    txtKomutCihazNo = IstemciString.Substring(3, 7);
                                    ComSor();
                                    vana_ac_Click();
                                    break;
                                }
                            case "VK":
                                {
                                    txtKomutCihazNo = IstemciString.Substring(3, 7);
                                    ComSor();
                                    vana_kapa_Click();
                                    break;
                                }
                            case "KY":
                                {
                                    txtKomutCihazNo = IstemciString.Substring(3, 7);
                                    ComSor();
                                    txtKomutYukKrd = IstemciString.Substring(10);
                                    kredi_yukle_Click();
                                    break;
                                }
                            case "KI":
                                {
                                    txtKomutCihazNo = IstemciString.Substring(3, 7);
                                    ComSor();
                                    kredi_iade_Click();
                                    break;
                                }
                            default:
                                {
                                    AkimYazici.WriteLine("Gönderilen Mesaj Hatalı");
                                    AkimYazici.Flush();
                                    break;
                                }
                        }
                    }
                    catch (Exception)
                    {
                        TcpDinleyicisi.Stop();
                        TcpDinleyicisi.Start();
                    }
                }
            }
            timer.Start();
        }

        public void ComSor()
        {
            cport.Close();
            if (txtKomutCihazNo == "6011691")
                cport.PortName = "COM3";
            else
                cport.PortName = "COM4";
        }

        public void onDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            Basla();
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
        }

        public void BaglantiKontrol()
        {
            if (!cport.IsOpen)
                cport.Open();
            Thread.Sleep(500);
        }

        private void cmdOku_Click()
        {
            BaglantiKontrol();
            UyandirmaGonder(false);
            if (SHata == 0)
            {
                Thread.Sleep(500);
                Komut_Gonder("O", 0, "", 32, 5000);
            }
            if (SHata == 0)
            {
                GelenKisaBilgiDoldur();
                AkimYazici.WriteLine("KISA OKUMA TAMAM");
                AkimYazici.Flush();
            }
        }

        private void cmduzunoku_Click()
        {
            BaglantiKontrol();
            UyandirmaGonder(false);
            if (SHata == 0)
            {
                Thread.Sleep(500);
                Komut_Gonder("Z", 0, "", 256, 5000);
            }
            if (SHata == 0)
            {
                GelenUzunBilgiDoldur();
                AkimYazici.WriteLine("UZUN OKUMA TAMAM");
                AkimYazici.Flush();
            }
        }

        public void UyandirmaGonder(bool SfrKontrol)
        {
            string GidenData;
            for (int i = 1; i <= 1024; i++)
                Gel_Numeric[i] = 0;
            ClearReadBuffer(cport);
            Long2Byte(Convert.ToInt64(txtKomutCihazNo), 0);
            CS = Convert.ToChar(StartChar) ^ OutBuF[0] ^ OutBuF[1] ^ OutBuF[2] ^ OutBuF[3] ^ Convert.ToChar(5) ^ Convert.ToChar("S") ^ OutBuF[0] ^ OutBuF[1] ^ OutBuF[2] ^ OutBuF[3];
            GidenData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)(5) + "S" +
                        ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)(CS % 256);
            SendBuffer(cport, GidenData);
            GelData1 = ReadBuffer(cport, 11, 3000);
            if (GelData1.Length >= 10)
            {
                SayCNo = Byte2Long(Encoding.Default.GetBytes(GelData1.Substring(1, 1))[0], Encoding.Default.GetBytes(GelData1.Substring(2, 1))[0], Encoding.Default.GetBytes(GelData1.Substring(3, 1))[0], Encoding.Default.GetBytes(GelData1.Substring(4, 1))[0]);
                if (txtKomutCihazNo != Convert.ToString(SayCNo))
                {
                    SHata = 2;
                    AkimYazici.WriteLine("HATA Okunan Sayac No Hatalı");
                    AkimYazici.Flush();
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
                AkimYazici.WriteLine("HATA Sayac Uyanamadı.");
                AkimYazici.Flush();
                return;
            }
        }

        private void Komut_Gonder(string KoMuT, int KmtLen, string KmtData, int KmtOkuLen, float KmtOkuTimeout)
        {
            string GidenData;
            long KmtCSM;
            if (SHata == 0)
            {
                Long2Byte(Convert.ToInt64(txtKomutCihazNo), 0);
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
                        AkimYazici.WriteLine("HATA Komut Data Uzunluğu Uyuşmazlığı.");
                        AkimYazici.Flush();
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
                    for (int i = 1; i <= GelData2.Length; i++)
                    {
                        Gel_Numeric[i - 1] = Encoding.Default.GetBytes(GelData2.Substring(i - 1, 1))[0];
                    }

                    if (KoMuT == "V" && KmtData == "A" || KmtData == "K")
                    {
                        if (Gel_Numeric[0] != Convert.ToByte("T"))
                        {
                            SHata = 4;
                        }
                    }
                    if (KoMuT == "Y" && KmtData == "I")
                    {
                        if (Gel_Numeric[0] != Convert.ToByte("T"))
                        {
                            SHata = 5;
                        }
                    }
                }
                else
                {
                    SHata = 3;
                    AkimYazici.WriteLine("HATA Sayactan Yanıt Gelmedi.");
                    AkimYazici.Flush();
                }
            }
        }

        public void ClearReadBuffer(SerialPort cport)
        {
            SHata = 0;
            cport.DiscardInBuffer();
            cport.DiscardOutBuffer();
            AkimOkuyucu.DiscardBufferedData();
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
            string sifrele = "";
            int sum, A;
            int[] Out = new int[4];
            int[] sYn = new int[4];
            int[] sy = new int[4];
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

        private void vana_ac_Click()
        {
            try
            {
                BaglantiKontrol();
                string KomutData;
                UyandirmaGonder(true);
                if (SHata == 0)
                {
                    KomutData = "A" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                    Thread.Sleep(500);
                    Komut_Gonder("V", KomutData.Length, KomutData, 1, 3000);
                }
                if (SHata == 0)
                {
                    AkimYazici.WriteLine("VANA ACMA TAMAM");
                    AkimYazici.Flush();
                }
            }
            catch (Exception)
            {
                AkimYazici.WriteLine("HATA VANA AÇILAMADI...");
                AkimYazici.Flush();
            }
        }

        private void vana_kapa_Click()
        {
            try
            {
                BaglantiKontrol();
                string KomutData;
                UyandirmaGonder(true);
                if (SHata == 0)
                {
                    KomutData = "K" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                    Thread.Sleep(500);
                    Komut_Gonder("V", KomutData.Length, KomutData, 1, 3000);
                }
                if (SHata == 0)
                {
                    AkimYazici.WriteLine("VANA KAPAT TAMAM");
                    AkimYazici.Flush();
                }
            }
            catch (Exception)
            {
                AkimYazici.WriteLine("HATA VANA KAPATILAMADI...");
                AkimYazici.Flush();
            }
        }

        public void kredi_yukle_Click()
        {
            try
            {
                BaglantiKontrol();
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
                UyandirmaGonder(true);
                KomutData = "Y" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                Cx = (Hex2Byte(SfR.Substring(2, 2)) + 1) % 256;
                Long2Byte(Convert.ToInt64(txtKomutYukKrd), 0);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[0])) });
                Cx = ((Cx + 1) % 256);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[1])) });
                Cx = ((Cx + 1) % 256);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[2])) });
                Cx = ((Cx + 1) % 256);
                KomutData = KomutData + Encoding.Default.GetString(new byte[] { ((byte)(Cx ^ OutBuF[3])) });
                KomutData = KomutData + ((char)(Convert.ToInt64(txtKomutAyar) % 256));
                switch (txtKomutNoktaSayi)
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
                Long2Byte(Convert.ToInt64(txtKomutYKKrd), 0);
                KomutData = KomutData + ((char)OutBuF[0]);
                KomutData = KomutData + ((char)OutBuF[1]);
                KomutData = KomutData + ((char)OutBuF[2]);
                if (SHata == 0)
                {
                    Komut_Gonder("Y", KomutData.Length, KomutData, 1, 3000);
                }
                else
                {
                    return;
                }
                Long2Byte(Convert.ToInt64(txtKomutCihazNo), 0);
                KomutData = StartChar + ((char)OutBuF[0]) + ((char)OutBuF[1]) + ((char)OutBuF[2]) + ((char)OutBuF[3]) + (char)22 + (char)20;
                KomutData = KomutData + (char)(201); 
                Cx = KKatH[0] % 256;
                KomutData = KomutData + ((char)Cx);
                KomutData = KomutData + ((char)((KKatH[0] - Cx) / 256));
                KomutData = KomutData + ((char)KKatL[0]);
                Cx = KKatH[1] % 256;
                KomutData = KomutData + ((char)Cx);
                KomutData = KomutData + ((char)((KKatH[1] - Cx) / 256));
                KomutData = KomutData + ((char)KKatL[1]);
                Cx = KKatH[2] % 256;
                KomutData = KomutData + ((char)Cx);
                KomutData = KomutData + ((char)((KKatH[2] - Cx) / 256));
                KomutData = KomutData + ((char)(KKatL[2]));
                Cx = KKatH[3] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KKatH[3] - Cx) / 256));
                KomutData = KomutData + ((char)(KKatL[3]));
                Cx = Dnm % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((Dnm - Cx) / 256));
                Cx = KadM[0] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KadM[0] - Cx) / 256));
                Cx = KadM[1] % 256;
                KomutData = KomutData + ((char)(Cx));
                KomutData = KomutData + ((char)((KadM[1] - Cx) / 256));
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
                            AkimYazici.WriteLine("HATA 2. T Paketi Gelmedi.");
                            AkimYazici.Flush();
                            return;
                        }

                        Cx = Byte2Long(Encoding.Default.GetBytes(GelData.Substring(1, 1))[0], Encoding.Default.GetBytes(GelData.Substring(2, 1))[0], Encoding.Default.GetBytes(GelData.Substring(3, 1))[0], Encoding.Default.GetBytes(GelData.Substring(4, 1))[0]);
                        if (Cx != Convert.ToInt64(txtKomutYukKrd))
                        {
                            SHata = 202;
                            AkimYazici.WriteLine("HATA Yüklenen ile Gelen Kredi Aynı değil AÇILAMADI...");
                            AkimYazici.Flush();
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
                        AkimYazici.WriteLine("HATA 2.T Paketi Okunamadı");
                        AkimYazici.Flush();
                        return;
                    }
                }
                else
                {
                    return;
                }
                Long2Byte(Convert.ToInt64(txtKomutCihazNo), 0);
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
                        AkimYazici.WriteLine("HATA 3. T Paketi Gelmedi.");
                        AkimYazici.Flush();
                        return;
                    }
                }
                else
                {
                    SHata = 205;
                    AkimYazici.WriteLine("HATA 3. T Paketi Okunamadı.");
                    AkimYazici.Flush();
                    return;
                }
                Long2Byte(Convert.ToInt64(txtKomutCihazNo), 0);
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
                    SHata = 207;
                    AkimYazici.WriteLine("HATA Kredi Yükleme Başarılı Sayaç Okunamadı.");
                    AkimYazici.Flush();
                }
                if (SHata == 0)
                {
                    GelenKisaBilgiDoldur();
                    AkimYazici.WriteLine("KREDI YUKLE TAMAM");
                    AkimYazici.Flush();
                }
            }
            catch (Exception)
            {
            }
        }

        public void kredi_iade_Click()
        {
            try
            {
                BaglantiKontrol();
                string KomutData;
                string GelData;
                UyandirmaGonder(true);
                KomutData = "I" + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(2, 2)) })) + (Encoding.Default.GetString(new byte[] { Hex2Byte(SfR.Substring(4, 2)) })) + (char)(IssuE[0]) + (char)(IssuE[1]);
                if (SHata == 0)
                {
                    Thread.Sleep(500);
                    Komut_Gonder("Y", KomutData.Length, KomutData, 6, 3000);
                    CS = Gel_Numeric[0] ^ Gel_Numeric[1] ^ Gel_Numeric[2] ^ Gel_Numeric[3] ^ Gel_Numeric[4];
                    if ((CS != Gel_Numeric[5]))
                    {
                        SHata = 150;
                        AkimYazici.WriteLine("HATA IADE Kredi Paket CSUM Hatalı");
                        AkimYazici.Flush();
                        return;
                    }
                }
                if (SHata == 0)
                {
                    Long2Byte(Convert.ToInt64(txtKomutCihazNo), 0);
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
                        SHata = 151;
                        AkimYazici.WriteLine("HATA Kredi IADE Edildi ama Okuma Yapılamadı");
                        AkimYazici.Flush();
                    }
                    if (SHata == 0)
                    {
                        GelenKisaBilgiDoldur();
                        AkimYazici.WriteLine("KREDI IADE TAMAM");
                        AkimYazici.Flush();
                    }
                }
            }
            catch (Exception)
            {
                AkimYazici.WriteLine("HATA KREDI IADE EDILEMEDI.");
                AkimYazici.Flush();
            }
        }

        string txtFlag0 = string.Empty;
        string txtFlag2 = string.Empty;
        string txtSayTip = string.Empty;
        string txtIssue = string.Empty;
        string txtNoktaSayisi = string.Empty;
        string txtHarcananKredi = string.Empty;
        string txtCihazNo = string.Empty;
        string txtKalanKredi = string.Empty;
        string txtSaat = string.Empty;
        string txtTarih = string.Empty;
        string txtMekanik = string.Empty;
        string txtNSayisi = string.Empty;
        string txtHGun = string.Empty;
        string txtCNo = string.Empty;
        string txtKalan = string.Empty;
        string txtHarcananKrd = string.Empty;
        string txtHarcanan = string.Empty;
        string txtHarcananTers = string.Empty;
        string txtKritik = string.Empty;
        string txtTarihSaat = string.Empty;
        string Text2 = string.Empty;
        string txtCT = string.Empty;
        string txtAT = string.Empty;
        string txtResetT = string.Empty;
        string txtVKT = string.Empty;
        string txtVAT = string.Empty;
        string txtVKSay = string.Empty;
        string s = string.Empty;
        string txtPerVanaSaat = string.Empty;
        string txtPerVanaGun = string.Empty;
        string txtVanaPer = string.Empty;
        string txtEkran = string.Empty;
        string txtEkran1 = string.Empty;
        string txtKK11 = string.Empty;
        string txtKK21 = string.Empty;
        string txtKK31 = string.Empty;
        string txtKK41 = string.Empty;
        string txtKademe1 = string.Empty;
        string txtKademe2 = string.Empty;
        string txtKademe3 = string.Empty;
        string txtDonem = string.Empty;
        string txtDonemHar = string.Empty;
        string txtDonemTuk = string.Empty;
        string txtDonemGun = string.Empty;
        string txtAKdm = string.Empty;
        string lblG1 = string.Empty;
        string lblG2 = string.Empty;
        string lblPA = string.Empty;
        string lblPS = string.Empty;
        string lblN = string.Empty;
        string lblNoktaS = string.Empty;
        string lblIssue = string.Empty;
        string lblASEV = string.Empty;
        string vanadurum = string.Empty;

        public void GelenKisaBilgiDoldur()
        {
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
                SHata = 101;
                return;
            }
            txtSayTip = Convert.ToChar(GData[3]).ToString();
            AkimYazici.WriteLine("SayTip " + txtSayTip);
            txtIssue = Convert.ToChar(GData[4]) + Convert.ToChar(GData[5]).ToString();
            AkimYazici.WriteLine("txtIss " + txtIssue);
            txtNoktaSayisi = Byte2Hex(GData[6]);
            AkimYazici.WriteLine("txtNok " + txtNoktaSayisi);
            switch (txtNoktaSayisi)
            {
                case "E2": { txtNSayisi = "0"; break; }
                case "E6": { txtNSayisi = "1"; break; }
                case "EA": { txtNSayisi = "2"; break; }
                case "EE": { txtNSayisi = "3"; break; }
                default: { txtNSayisi = "?"; break; }
            }
            AkimYazici.WriteLine("txtNsa " + txtNSayisi);
            txtFlag0 = GData[8].ToString("X");
            AkimYazici.WriteLine("tFlag0 " + txtFlag0);
            txtFlag2 = GData[9].ToString("X");
            AkimYazici.WriteLine("tFlag2 " + txtFlag2);
            CS = 1;
            for (int i = 0; i < 4; i++)
            {
                CS = CS * 2;
            }
            if (GData[9] == CS)
            {
                vanadurum = "true";
            }
            else
            {
                vanadurum = "false";
            }
            AkimYazici.WriteLine("VanDur " + vanadurum.ToString());
            txtKalanKredi = Convert.ToString(Byte2Long(GData[17], GData[18], GData[19], GData[20]));
            AkimYazici.WriteLine("KalKre " + txtKalanKredi);
            txtHarcananKredi = Convert.ToString(Byte2Long(GData[21], GData[22], GData[23], GData[24]));
            AkimYazici.WriteLine("HarKre " + txtHarcananKredi);
            txtCihazNo = Convert.ToString(SayCNo);
            AkimYazici.WriteLine("CihaNo " + txtCihazNo);
            txtSaat = Convert.ToString(GData[12]) + " : " + Convert.ToString(GData[11]) + " : " + Convert.ToString(GData[10]);
            AkimYazici.WriteLine("txSaat " + txtSaat);
            txtTarih = Convert.ToString(GData[13]) + "/" + Convert.ToString(GData[14]) + "/" + "20" + Convert.ToString(GData[15]);
            AkimYazici.WriteLine("txtTar " + txtTarih);
            string txtHaftaGun = (GData[16]).ToString();
            switch (txtHaftaGun)
            {
                case "1": { txtHGun = "Pazartesi"; break; }
                case "2": { txtHGun = "Salı"; break; }
                case "3": { txtHGun = "Çarşamba"; break; }
                case "4": { txtHGun = "Perşembe"; break; }
                case "5": { txtHGun = "Cuma"; break; }
                case "6": { txtHGun = "Cumartesi"; break; }
                case "7": { txtHGun = "Pazar"; break; }
                default: { txtHGun = " "; break; }
            }
            AkimYazici.WriteLine("txHGun " + txtHGun);
            AkimYazici.Flush();
        }

        public void GelenUzunBilgiDoldur()
        {
            int Br, i, j;
            byte[] GData = new byte[300];
            txtSayTip = "S";
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
            txtKalan = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("txtKal " + txtKalan);
            txtKalanKredi = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("KalKre " + txtKalanKredi);

            Br = 48;
            txtHarcananKrd = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("HarKrd " + txtHarcananKrd);
            txtHarcananKredi = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("HarKre " + txtHarcananKredi);

            Br = 39;
            txtCNo = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("txtCNo " + txtCNo);
            txtCihazNo = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("CihaNo " + txtCihazNo);

            Br = 53;
            txtHarcanan = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("txtHar " + txtHarcanan);

            Br = 64;
            txtHarcananTers = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3]));
            AkimYazici.WriteLine("HarTer " + txtHarcananTers);

            Br = 28;
            txtKritik = Convert.ToString(Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], 0));
            AkimYazici.WriteLine("txtKri " + txtKritik);

            Br = 0;
            txtTarihSaat = Byte2DateL(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3], GData[Br + 4], GData[Br + 5]);
            AkimYazici.WriteLine("txtThS " + txtTarihSaat);
            txtTarih = Convert.ToInt64(GData[Br + 3]) + "/" + Convert.ToInt64(GData[Br + 4]) + "/" + (Convert.ToInt64(GData[Br + 5]) + 2000);
            AkimYazici.WriteLine("txtTar " + txtTarih);
            txtSaat = Convert.ToInt64(GData[Br + 2]) + " : " + Convert.ToInt64(GData[Br + 1]) + " : " + Convert.ToInt64(GData[Br]);
            AkimYazici.WriteLine("txSaat " + txtSaat);
            Br = 206;
            txtHGun = GData[Br].ToString();
            switch (txtHGun)
            {
                case "1": { txtHGun = "Pazartesi"; break; }
                case "2": { txtHGun = "Salı"; break; }
                case "3": { txtHGun = "Çarşamba"; break; }
                case "4": { txtHGun = "Perşembe"; break; }
                case "5": { txtHGun = "Cuma"; break; }
                case "6": { txtHGun = "Cumartesi"; break; }
                case "7": { txtHGun = "Pazar"; break; }
                default: { txtHGun = " "; break; }
            }
            Text2 = txtHGun;
            AkimYazici.WriteLine("txHGun " + txtHGun);
            Br = 0;
            txtFlag0 = GData[Br + 69].ToString("X");
            AkimYazici.WriteLine("tFlag0 " + txtFlag0);
            txtFlag2 = GData[Br + 71].ToString("X");
            AkimYazici.WriteLine("tFlag2 " + txtFlag2);
            CS = 1;
            for (i = 0; i < 4; i++)
            {
                CS = CS * 2;
            }
            if (GData[Br + 71] == CS)
            {
                vanadurum = "true";
            }
            else
            {
                vanadurum = "false";
            }
            AkimYazici.WriteLine("VanDur " + vanadurum.ToString());
            Br = 16;
            txtResetT = Byte2DateL(GData[Br + 0], GData[Br + 1], GData[Br + 2], GData[Br + 3], GData[Br + 4], GData[Br + 5]);
            AkimYazici.WriteLine("txtRes " + txtResetT);

            Br = 8;
            txtCT = GData[Br + 0].ToString() + "/" + GData[Br + 1].ToString() + "/" + (GData[Br + 2] + 2000).ToString();
            AkimYazici.WriteLine("txtCTT " + txtCT);
            CS = Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]);
            CS = CS % 256;
            CS = CS ^ 255;

            Br = 12;
            txtAT = GData[Br + 0].ToString() + "/" + GData[Br + 1].ToString() + "/" + (GData[Br + 2] + 2000).ToString();
            AkimYazici.WriteLine("txtATT " + txtAT);
            CS = Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]);
            CS = CS % 256;
            CS = CS ^ 255;

            Br = 61;
            lblIssue = Encoding.Default.GetString(new byte[] { GData[Br + 0] }) + Encoding.Default.GetString(new byte[] { GData[Br + 1] });
            AkimYazici.WriteLine("lblIss " + lblIssue);
            txtIssue = lblIssue;
            CS = (((Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;

            Br = 75;
            lblN = GData[Br + 0].ToString("X");
            AkimYazici.WriteLine("lblNNN " + lblN);
            CS = (((Convert.ToInt64(GData[Br + 0])) % 256) ^ 255) % 256;
            txtNoktaSayisi = lblN;
            switch (txtNoktaSayisi)
            {
                case "E2": { txtNSayisi = "0"; break; }
                case "E6": { txtNSayisi = "1"; break; }
                case "EA": { txtNSayisi = "2"; break; }
                case "EE": { txtNSayisi = "3"; break; }
                default: { txtNSayisi = "?"; break; }
            }
            AkimYazici.WriteLine("txtNSa " + txtNSayisi);
            switch (lblN)
            {
                case "E2": { lblNoktaS = "0"; break; }
                case "E6": { lblNoktaS = "1"; break; }
                case "EA": { lblNoktaS = "2"; break; }
                case "EE": { lblNoktaS = "3"; break; }
                default: { lblNoktaS = lblN; break; }
            }
            AkimYazici.WriteLine("lblNok " + lblNoktaS);
            Br = 77;
            lblG1 = GData[Br + 0].ToString();
            AkimYazici.WriteLine("lblG11 " + lblG1);
            lblG2 = GData[Br + 0].ToString();
            AkimYazici.WriteLine("lblG22 " + lblG2);
            CS = (((Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;

            Br = 80;
            lblASEV = GData[Br + 0].ToString();
            AkimYazici.WriteLine("lblASE " + lblASEV);
            CS = (((Convert.ToInt64(GData[Br + 0])) % 256) ^ 255) % 256;

            Br = 82;
            lblPS = GData[Br + 0].ToString("X");
            AkimYazici.WriteLine("lblPSS " + lblPS);
            lblPA = GData[Br + 1].ToString("X");
            AkimYazici.WriteLine("lblPAA " + lblPA);
            CS = (((Convert.ToInt64(GData[Br + 0]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;

            Br = 176;
            txtDonemHar = Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], 0).ToString();
            AkimYazici.WriteLine("txtDnH " + txtDonemHar);

            Br = 180;
            txtDonemTuk = Byte2Long(GData[Br + 0], GData[Br + 1], GData[Br + 2], 0).ToString();
            AkimYazici.WriteLine("txtDnT " + txtDonemTuk);

            Br = 173;
            txtDonem = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            AkimYazici.WriteLine("txtDnm " + txtDonem);

            Br = 156;
            txtDonemGun = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            AkimYazici.WriteLine("txtDnG " + txtDonemGun);
            txtAKdm = GData[Br + 2].ToString();
            AkimYazici.WriteLine("txtAKd " + txtAKdm);
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2])) % 256) ^ 255;

            Br = 160;
            txtKK11 = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            AkimYazici.WriteLine("txtKK1 " + txtKK11);
            txtKK21 = Byte2Long(GData[Br + 3], GData[Br + 4], 0, 0).ToString();
            AkimYazici.WriteLine("txtKK2 " + txtKK21);
            txtKK31 = Byte2Long(GData[Br + 6], GData[Br + 7], 0, 0).ToString();
            AkimYazici.WriteLine("txtKK3 " + txtKK31);
            txtKK41 = Byte2Long(GData[Br + 9], GData[Br + 10], 0, 0).ToString();
            AkimYazici.WriteLine("txtKK4 " + txtKK41);
            CS = 0;
            for (i = Br; i < Br + 11; i++)
            {
                CS = CS + Convert.ToInt64(GData[i]);
            }
            CS = (CS % 256) ^ 255;

            Br = 149;
            txtKademe1 = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            AkimYazici.WriteLine("txtKd1 " + txtKademe1);
            txtKademe2 = Byte2Long(GData[Br + 2], GData[Br + 3], 0, 0).ToString();
            AkimYazici.WriteLine("txtKd2 " + txtKademe2);
            txtKademe3 = Byte2Long(GData[Br + 4], GData[Br + 5], 0, 0).ToString();
            AkimYazici.WriteLine("txtKd3 " + txtKademe3);
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]) + Convert.ToInt64(GData[Br + 3]) + Convert.ToInt64(GData[Br + 4]) + Convert.ToInt64(GData[Br + 5])) % 256) ^ 255;

            Br = 87;
            txtEkran = GData[Br].ToString();
            AkimYazici.WriteLine("txtEkr " + txtEkran);
            txtEkran1 = GData[Br + 1].ToString();
            AkimYazici.WriteLine("txtEk1 " + txtEkran1);
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255;

            Br = 90;
            txtPerVanaSaat = GData[Br].ToString();
            AkimYazici.WriteLine("txtPVS " + txtPerVanaSaat);
            txtPerVanaGun = GData[Br + 1].ToString();
            AkimYazici.WriteLine("txtPVG " + txtPerVanaGun);
            txtVanaPer = GData[Br + 2].ToString();
            AkimYazici.WriteLine("txtVNP " + txtVanaPer);
            CS = ((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2])) % 256) ^ 255;

            Br = 138;
            txtVKT = GData[Br + 2].ToString() + "/" + GData[Br + 3].ToString() + "/" + (GData[Br + 4] + 2000).ToString();
            AkimYazici.WriteLine("txtVKT " + txtVKT);
            CS = (((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]) + Convert.ToInt64(GData[Br + 3]) + Convert.ToInt64(GData[Br + 4])) % 256) ^ 255) % 256;

            Br = 122;
            txtVAT = GData[Br + 2].ToString() + "/" + GData[Br + 3].ToString() + "/" + (GData[Br + 4] + 2000).ToString();
            AkimYazici.WriteLine("txtVAT " + txtVAT);
            CS = (((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1]) + Convert.ToInt64(GData[Br + 2]) + Convert.ToInt64(GData[Br + 3]) + Convert.ToInt64(GData[Br + 4])) % 256) ^ 255) % 256;

            Br = 109;
            txtVKSay = Byte2Long(GData[Br], GData[Br + 1], 0, 0).ToString();
            AkimYazici.WriteLine("txtVKS " + txtVKSay);
            CS = (((Convert.ToInt64(GData[Br]) + Convert.ToInt64(GData[Br + 1])) % 256) ^ 255) % 256;
            AkimYazici.Flush();
        }
    }
}
