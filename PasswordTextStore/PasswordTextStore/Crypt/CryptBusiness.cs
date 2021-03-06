﻿using PasswordTextStore.WPF.Password;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordTextStore.Crypt
{
    public class CryptBusiness
    {
        public static bool OpenFile()
        {
            var open = new OpenWindow();
            var result = open.ShowDialog();

            if (result ?? false)
            {
                var file = CryptFile.Open(open.GetPassword);
                if (file == null)
                {
                    Program.Warning("Permission denied!");
                }
                else
                {
                    if (!file.Data.IsCreated)
                    {
                        file.Save(open.GetPassword);
                    }

                    Program.File = file;
                    Program.PasswordUsed = open.GetPassword;

                    return true;
                }
            }

            return false;
        }
    }
}