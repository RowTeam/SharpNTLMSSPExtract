using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpDetectionNTLMSSP
{
    public class Wantprefixlen
    {
        /// <summary>
        /// 网段处理
        /// </summary>
        /// <param name="txt_target"></param>
        /// <returns></returns>
        public static HashSet<string> wantprefixlen(String text_target)
        {
            HashSet<string> list_target = new HashSet<string>();
            // 处理 ip
            if (!"".Equals(text_target))
            {
                // 如果输入单个 IP
                if (Regex.IsMatch(text_target, "^([\\w\\-\\.]{1,100}[a-zA-Z]{1,8})$|^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})$"))
                {
                    list_target.Clear();
                    list_target.Add(text_target);
                }
                // 如果同时输入多个
                else if (text_target.Contains(','))
                {
                    list_target.Clear();
                    foreach(var list in text_target.Split(',').ToList())
                        list_target.Add(list);
                }
                // 如果输入自定义 IP 段，仅支持 B 段自定义范围（ex: 192.168.1.1-192.168.20.2）
                else if (Regex.IsMatch(text_target, "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\-\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$"))
                {
                    B(text_target, ref list_target);
                }
                // 如果输入 Prefix(cidr) 格式
                else if (Regex.IsMatch(text_target, "^([\\w\\-\\.]{1,100}[a-zA-Z]{1,8})$|^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\/\\d{1,3})$"))
                {
                    CIDR(text_target, ref list_target);
                }
                else if (text_target.EndsWith(".txt"))
                {
                    readFileToList(text_target, ref list_target);
                }

                if (list_target.Count <= 0)
                {
                    Info.ShowUsage();
                    Environment.Exit(0);
                }
            }

            return list_target;
        }

        private static void B(String txt_target, ref HashSet<string> list_target)
        {
            list_target.Clear();
            string[] arrayall = txt_target.Split(new char[]
            {
                        '-'
            });
            if (arrayall.Length == 2)
            {
                string text1 = arrayall[0];
                string text2 = arrayall[1];
                string[] array1 = text1.Split(new char[]
                {
                            '.'
                });
                string[] array2 = text2.Split(new char[]
                {
                            '.'
                });
                if (array1.Length == 4 && array2.Length == 4)
                {
                    int num1_3 = int.Parse(array1[2]);
                    int num2_3 = int.Parse(array2[2]);
                    int num1_4 = int.Parse(array1[3]);
                    int num2_4 = int.Parse(array2[3]);


                    if (num2_3 >= num1_3 && num2_3 <= 255 && num2_4 <= 255)
                    {
                        for (int i = num1_3; i <= num2_3; i++)
                        {
                            if (num1_3 == num2_3)
                            {
                                if (num1_4 <= num2_4)
                                {
                                    for (int j = num1_4; j <= num2_4; j++)
                                    {
                                        string item = string.Concat(new string[]
                                        {
                                                    array2[0],
                                                    ".",
                                                    array2[1],
                                                    ".",
                                                    array2[2],
                                                    ".",
                                                    j.ToString()
                                        });
                                        list_target.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                int num5 = 0;
                                int num6 = 255;
                                if (i == num1_3)
                                {
                                    num5 = num1_4;
                                }
                                if (i == num2_3)
                                {
                                    num6 = num2_4;
                                }
                                for (int k = num5; k <= num6; k++)
                                {
                                    string item2 = string.Concat(new string[]
                                    {
                                                array2[0],
                                                ".",
                                                array2[1],
                                                ".",
                                                i.ToString(),
                                                ".",
                                                k.ToString()
                                    });
                                    list_target.Add(item2);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CIDR(String txt_target, ref HashSet<string> list_target)
        {
            list_target.Clear();
            string ip;
            if (txt_target.Contains("/24"))
            {
                ip = txt_target.Substring(0, txt_target.LastIndexOf('.'));
                for (int i = 1; i < 255; i++)
                {
                    list_target.Add(String.Format("{0}.{1}", ip, i));
                }
            }
            else if (txt_target.Contains("/16"))
            {
                ip = txt_target.Substring(0, txt_target.LastIndexOf('.'));
                ip = ip.Substring(0, ip.LastIndexOf('.'));
                for (int i = 0; i < 255; i++)
                {
                    for (int j = 1; j < 255; j++)
                    {
                        list_target.Add(String.Format("{0}.{1}.{2}", ip, i, j));
                    }
                }
            }
            else if (txt_target.Contains("/8"))
            {
                ip = txt_target.Substring(0, txt_target.IndexOf('.'));
                for (int i = 0; i < 255; i++)
                {
                    for (int j = 0; j < 255; j++)
                    {
                        for (int k = 1; k < 255; k++)
                        {
                            list_target.Add(String.Format("{0}.{1}.{2}.{3}", ip, i, j, k));
                        }
                    }
                }

            }
        }

        public static void readFileToList(String path, ref HashSet<string> list_target)
        {
            list_target.Clear();
            FileStream fs_dir = null;
            StreamReader reader = null;
            try
            {
                fs_dir = new FileStream(path, FileMode.Open, FileAccess.Read);

                reader = new StreamReader(fs_dir);

                String lineStr;

                while ((lineStr = reader.ReadLine()) != null)
                {
                    if (!lineStr.Equals(""))
                    {
                        list_target.Add(lineStr);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[!] Error: An exception occurred while reading the file list!" + e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (fs_dir != null)
                {
                    fs_dir.Close();
                }
            }
        }
    }
}

