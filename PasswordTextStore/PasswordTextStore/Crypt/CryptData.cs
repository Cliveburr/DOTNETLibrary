using PasswordTextStore.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordTextStore.Crypt
{
    [Serializable()]
    public class CryptData
    {
        public static uint ActualVersion { get => 1; }

        public uint Version { get; set; }
        public string Text { get; set; }
        public bool IsCreated { get; set; }
        public List<WindowConfigData> Windows { get; set; }
        public List<CryptText> Texts { get; set; }

        public void InitializeData()
        {
            Windows = Windows ?? new List<WindowConfigData>();

            if (Version < ActualVersion)
            {
                Version = ActualVersion;

                Texts = new List<CryptText>
                {
                    new CryptText { Name = "Old", Text = Text }
                };
            }
        }
    }

    [Serializable]
    public class CryptText
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }
}