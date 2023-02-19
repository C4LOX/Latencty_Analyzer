using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;


namespace GLatencty_Analyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test Başlıyor Lütfen Bekleyiniz!");
            Console.WriteLine("Bu işlem yaklaşık 5 dakika sürecektir.");
            Console.WriteLine("Lütfen İşlem tamamlandı yazana kadar programı kapatmayınız!");



            string[] ipAddresses = new string[] {
            "100.127.254.1",
            "1.1.1.1",
            "8.8.8.8",
            "146.66.155.1",
            "3.64.0.0"
        };

            string fileName = $"packetloss_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Tarih: {DateTime.Now}");
                foreach (string ipAddress in ipAddresses)
                {
                    int sentPackets = 10;
                    int receivedPackets = 0;

                    for (int i = 0; i < sentPackets; i++)
                    {
                        PingReply reply = new Ping().Send(ipAddress);
                        if (reply.Status == IPStatus.Success)
                        {
                            receivedPackets++;
                        }
                    }

                    int packetLoss = (int)Math.Round(((double)(sentPackets - receivedPackets) / sentPackets) * 100);
                    writer.WriteLine($"IP Address: {ipAddress}, Packet Loss: {packetLoss}%");

                }
                writer.WriteLine(" ");


            }

            Console.WriteLine(" ");
            Console.Write("Paketloss Testi");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("        OK");
            Console.ResetColor();

            List<string> targetHosts = new List<string> {
              "localhost",
              "100.127.254.1",
              "1.1.1.1",
              "8.8.8.8",
              "146.66.155.1",
              "3.64.0.0"
            };

            var pingTimes = new List<List<long>>();
            Ping ping = new Ping();
            int pingCount = 1;

            // Dosya adı ve yolunu belirleyin
            string dosyaAdi = "sonuclar.txt";

            // StreamWriter örneğini oluşturun
            StreamWriter dosyaYazici = new StreamWriter(dosyaAdi);

            foreach (string host in targetHosts)
            {
                // Ping işlemini gerçekleştirin
                var pingTimesForHost = new List<long>();
                for (int j = 0; j < pingCount; j++)
                {
                    PingReply reply = ping.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        pingTimesForHost.Add(reply.RoundtripTime);
                    }
                }
                pingTimes.Add(pingTimesForHost);


                // Ping sonuçlarını dosyaya yazdırın
                dosyaYazici.WriteLine("Tarih: " + DateTime.Now);
                dosyaYazici.WriteLine($"Ping Sonuçları ({host}):");
                dosyaYazici.WriteLine($"Ortalama Ping Süresi: {CalculateAverage(pingTimesForHost)}ms");
                dosyaYazici.WriteLine(" ");
                Console.WriteLine(" ");
                Console.Write("Ping Testi");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("             OK");
                Console.ResetColor();
                // Tracert işlemini gerçekleştirin
                Process tracertProcess = new Process();
                tracertProcess.StartInfo.FileName = "tracert";
                tracertProcess.StartInfo.Arguments = host;
                tracertProcess.StartInfo.RedirectStandardOutput = true;
                tracertProcess.StartInfo.UseShellExecute = false;
                tracertProcess.Start();
                string tracertResult = tracertProcess.StandardOutput.ReadToEnd();
                tracertProcess.WaitForExit();

                // Tracert sonuçlarını dosyaya yazdırın
                dosyaYazici.WriteLine($"Tracert Sonuçları ({host}):");
                dosyaYazici.WriteLine(tracertResult);
                dosyaYazici.WriteLine(" ");
            }

            // StreamWriter örneğini kapatın
            dosyaYazici.Close();
            // StreamWriter örneğini kapatın
            dosyaYazici.Close();



            // İşlem tamamlandı mesajı yazdırın
            Console.WriteLine(" ");
            Console.WriteLine("İşlem tamamlandı programı kapatbilirsiniz!");


            Console.ReadLine();
        }

        static double CalculateAverage(List<long> pingTimes)
        {
            long totalTime = pingTimes.Sum();
            return (double)totalTime / pingTimes.Count;
        }
    }
}
