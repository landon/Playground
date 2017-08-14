using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class Template
    {
        public List<int> Sizes { get; private set; }

        public Template(List<int> sizes)
        {
            Sizes = sizes;
        }

        public Template(string s, Graph g)
        { }
        //public Template(string s, Graph g)
        //    : this(Regex.Split(s ?? "", @"\W+").Select((x, v) =>
        //        {
        //            var xx = x.ToLower();

        //            if (xx.StartsWith("d"))
        //                return g.Degree(v) + (x.Length > 1 ? xx.Substring(1).TryParseInt().Value : 0);
        //            if (xx.StartsWith("p"))
        //                return g.Degree(v) - (x.Length > 1 ? xx.Substring(1).TryParseInt().Value : 0);

        //            return xx.TryParseInt().Value;
        //        }).ToList())
        //{
        //}

        public override bool Equals(object o)
        {
            return Equals(o as Template);
        }

        public bool Equals(Template t)
        {
            if (!t.Exists())
                return false;

            return Sizes.SequenceEqual(t.Sizes);
        }

        public override int GetHashCode()
        {
            return Sizes.Aggregate(Sizes.Count, (t, s) => 31 * t + s);
        }

        public override string ToString()
        {
            return string.Join(",", Sizes);
        }

        public static implicit operator Template(string s)
        {
            return new Template(s, null);
            //return new Template(Regex.Split(s ?? "", @"\D+").Select(MetaKnowledge.TryParseInt).Where(x => x.HasValue).Select(x => x.Value).ToList());
        }
    }
}
