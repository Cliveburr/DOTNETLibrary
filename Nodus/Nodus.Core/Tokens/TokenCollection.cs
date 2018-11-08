using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Tokens
{
    public class TokenCollection<T>
    {
        public string TokenChars { get; set; }
        public int TokenLength { get; set; }

        private IDictionary<string, TokenStorage<T>> _dic;

        public TokenCollection(string tokenChars, int tokenLength)
        {
            TokenChars = tokenChars;
            TokenLength = tokenLength;
            _dic = new Dictionary<string, TokenStorage<T>>();
        }

        public TokenCollection()
            : this("0123456789ABCDEFGHIJLMNOPRSTUVXZWYabcdefghijlmnoprstuvxzwy", 20)
        {
        }

        private string GetFreeToken()
        {
            var tr = "";
            var rnd = new Random(DateTime.Now.Millisecond);
            do
            {
                tr = "";
                for (int i = 0; i < TokenLength; i++)
                    tr += TokenChars.Substring(rnd.Next(0, TokenChars.Length), 1);
            } while (_dic.Where(i => i.Key == tr).Count() > 0);
            return tr;
        }

        public TokenStorage<T> Add(T obj)
        {
            var storage = new TokenStorage<T>
            {
                Token = GetFreeToken(),
                Value = obj
            };
            _dic.Add(storage.Token, storage);
            return storage;
        }

        public bool Has(string token)
        {
            return _dic.ContainsKey(token);
        }

        public T Get(string token)
        {
            var has = _dic[token];
            if (has != null)
            {
                return has.Value;
            }
            return default(T);
        }

        public TokenStorage<T> GetStorage(string token)
        {
            return _dic[token];
        }
    }
}