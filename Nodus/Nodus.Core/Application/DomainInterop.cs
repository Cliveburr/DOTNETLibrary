using Nodus.Core.Application.Tag;
using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodus.Core.Application
{
    [Serializable()]
    public class DomainInterop : MarshalByRefObject
    {
        public IDictionary<string, ITag> Scripts { get; set; } = new Dictionary<string, ITag>();
        public IDictionary<string, Type> Tags { get; set; } = new Dictionary<string, Type>();

        public void LoadTags()
        {
            var type = typeof(ITag);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface);

            Tags = types.ToDictionary(
                (t) => t.Name,
                (t) => t);
        }

        public void AddScript(string name, string script)
        {
            Scripts.Add(name, Tokenizer(name, script));
        }

        private ITag Tokenizer(string name, string script)
        {
            var doc = new XmlDocument();
            doc.LoadXml(script);
            var node = doc as XmlNode;
            var root = new Script(name, doc);

            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    var token = TokenizerRecur(child, root);
                    if (token != null)
                        root.Childs.Add(token);
                }
            }

            return root;
        }

        private ITag TokenizerRecur(XmlNode node, ITag parent)
        {
            if (node.NodeType == XmlNodeType.Comment)
                return null;

            var tagName = node.Name;

            if (!Tags.ContainsKey(tagName))
                throw new Exception($"Tag not found: {tagName}");

            var tag = Tags[tagName];
            var inst = Activator.CreateInstance(tag) as ITag;
            inst.Node = node;

            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    var token = TokenizerRecur(child, inst);
                    if (token != null)
                        inst.Childs.Add(token);
                }
            }

            return inst;
        }

        public Result Run()
        {
            var rootScript = Scripts["root"];
            var context = new Context
            {
                Result = new Result()
            };

            foreach (var child in rootScript.Childs)
            {
                RunRecur(child, context.Clone());
            }

            return context.Result;
        }

        private void RunRecur(ITag tag, Context context)
        {
            tag.Execute(context);

            foreach (var child in tag.Childs)
            {
                RunRecur(child, context.Clone());
            }
        }
    }
}