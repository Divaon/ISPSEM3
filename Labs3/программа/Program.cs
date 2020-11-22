﻿using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ISPLAB2
{
    class Program
    {

        static int dates(string date, int men, int big)
        {
            int n = 0;
            for (int i = 0; i < date.Length; i++)
            {
                switch (date[i])
                {
                    case '1': n = n * 10 + 1; break;
                    case '2': n = n * 10 + 2; break;
                    case '3': n = n * 10 + 3; break;
                    case '4': n = n * 10 + 4; break;
                    case '5': n = n * 10 + 5; break;
                    case '6': n = n * 10 + 6; break;
                    case '7': n = n * 10 + 7; break;
                    case '8': n = n * 10 + 8; break;
                    case '9': n = n * 10 + 9; break;
                    case '0': n = n * 10 + 0; break;
                    default: n = 0; return n;
                }
            }
            if (n >= men && n <= big)
                return n;
            else
            {
                n = 0;
                return n;
            }
        }


        class Configuration
        {
            public string catalog0 { get; set; }
            public string catalog1 { get; set; }
            public string catalog2 { get; set; }
            public string catalog3 { get; set; }
            public string catalog4 { get; set; }
        }

        static async Task Main(string[] args)
        {
            Configuration catalog=new Configuration();
            Random rnd = new Random();
            int choose = rnd.Next(0, 1);



            if (choose == 0)
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(@"E:\sourcecat\conf\XMLFile1.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                XmlNodeList childnodes = xRoot.SelectNodes("*");
                foreach (XmlNode n in childnodes)
                {
                    if (n.Name == "catalog0")
                    {
                        catalog.catalog0 = n.InnerText;
                    }
                    if (n.Name == "catalog1")
                    {
                        catalog.catalog1 = n.InnerText;
                    }
                    if (n.Name == "catalog2")
                    {
                        catalog.catalog2 = n.InnerText;
                    }
                    if (n.Name == "catalog3")
                    {
                        catalog.catalog3 = n.InnerText;
                    }
                    if (n.Name == "sdvig")
                    {
                        catalog.catalog4 = n.InnerText;
                    }
                }
            }
            if (choose == 1)
            {
                using (FileStream fs = new FileStream("E:\\sourcecat\\conf\\json1.json", FileMode.OpenOrCreate))
                {
                    catalog = await JsonSerializer.DeserializeAsync<Configuration>(fs);
                }
            }
            string filename = "*.*";
             int sdvid = Convert.ToInt32(catalog.catalog4);
            DateTime date = new DateTime(1, 1, 1);

            Console.WriteLine(catalog.catalog0+" "+ catalog.catalog1 + " " + catalog.catalog2 + " " + catalog.catalog3 + " " + catalog.catalog4 + " " );

            while (true)
            {

                foreach (string findedfile in Directory.EnumerateFiles(catalog.catalog0, filename, SearchOption.AllDirectories))
                {
                    FileInfo FI;
                    try
                    {
                        FI = new FileInfo(findedfile);

                        if (FI.LastWriteTime > date)
                        {
                            Console.WriteLine(FI.FullName);
                            Console.WriteLine(FI.LastWriteTime);
                            char[] text = new char[7];
                            char[] name = new char[7];
                            using (FileStream fstream = File.OpenRead(FI.FullName))
                            {
                                byte[] array = new byte[fstream.Length];
                                fstream.Read(array, 0, array.Length);
                                string textFromFile = System.Text.Encoding.Default.GetString(array);
                                Array.Resize(ref text, textFromFile.Length);
                                for (int i = 0; i < textFromFile.Length; i++)
                                {
                                    text[i] = textFromFile[i];
                                    text[i] = (char)((int)text[i] + sdvid);
                                }

                            }
                            string[] words = FI.FullName.Split(new char[] { '\\' });

                            if (words.Length != 8)
                                break;

                            int n1 = dates(words[4], 1900, 2100);
                            int n2 = dates(words[5], 1, 12);
                            int n3 = dates(words[6], 1, 30);
                            string[] checkfilename = FI.Name.Split(new char[] { '_' });
                            if (checkfilename.Length != 7)
                                break;

                            if (checkfilename[1] != words[4] || checkfilename[2] != words[5] || checkfilename[3] != words[6])
                                break;
                            int n7 = dates(checkfilename[4], 0, 23);
                            int n8 = dates(checkfilename[5], 0, 59);
                            string[] sec = checkfilename[6].Split(new char[] { '.' });
                            int n9 = dates(sec[0], 0, 59);
                            if (n1 == 0 || n2 == 0 || n3 == 0 || n7 == 0 || n8 == 0 || n9 == 0)
                                break;

                            string arh = Path.Combine(catalog.catalog1 + '\\' + words[7]);
                            File.Copy(FI.FullName, arh, true);
                            string text2 = new string(text);
                            File.WriteAllText(arh, text2);
                            using (FileStream fstream = File.OpenRead(arh))
                            {
                                byte[] array = new byte[fstream.Length];
                                fstream.Read(array, 0, array.Length);
                                string textFromFile = System.Text.Encoding.Default.GetString(array);
                                Array.Resize(ref text, textFromFile.Length);
                                for (int i = 0; i < textFromFile.Length; i++)
                                {
                                    text[i] = (char)((int)text[i] - sdvid);
                                }


                            }
                            Console.WriteLine(arh);
                            string[] finishfilename = arh.Split(new char[] { '\\' });
                          
                            string fin = Path.Combine(catalog.catalog2 + '\\' + words[4] + '\\' + words[5] + '\\' + words[6]);
                            System.IO.Directory.CreateDirectory(fin);
                            string fin2 = Path.Combine(fin + '\\' + finishfilename[4]);
                            File.Copy(arh, fin2, true);
                            string text3 = new string(text);
                            File.WriteAllText(fin2, text3);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                date = DateTime.Now;
                try
                {
                    string a = Path.Combine(catalog.catalog3 + '\\' + "zarchive.zip");
                    File.Delete(a);
                    System.IO.Compression.ZipFile.CreateFromDirectory(catalog.catalog1, a);
                }
                catch
                {
                    continue;
                }
                Thread.Sleep(1000);

            }

        }
    }
}


