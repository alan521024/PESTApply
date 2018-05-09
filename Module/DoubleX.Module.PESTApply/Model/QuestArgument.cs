namespace DoubleX.Module.PESTApply
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using DoubleX.Resource.Culture;
    using DoubleX.Infrastructure.Utility;
    using DoubleX.Framework;

    public class QuestArgument<TValue>
    {
        public QuestArgument() { }
        public QuestArgument(string key, TValue value) : this(key, value, "") { }
        public QuestArgument(string key, TValue value, string name) { Key = key; Value = value; Name = name; }

        public string Key { get; set; }
        public TValue Value { get; set; }
        public string Name { get; set; }

    }

    public class QuestArgument : QuestArgument<string>
    {
        public QuestArgument() { }
        public QuestArgument(string key, string value) : this(key, value, "") { }
        public QuestArgument(string key, string value, string name) : base(key, value, name) { }
    }
}
