﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime;
using System.Runtime.InteropServices;

using ItzWarty;

namespace RAFManager
{
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
          string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
          string key, string def, StringBuilder retVal,
          int size, string filePath);

        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }

        public string this[string sectionDotKey]
        {
            get
            {
                string[] parts = sectionDotKey.Split(".");
                return IniReadValue(parts[0], parts[1]);
            }
            set
            {
                string[] parts = sectionDotKey.Split(".");
                IniWriteValue(parts[0], parts[1], value);
            }
        }
    }
}
