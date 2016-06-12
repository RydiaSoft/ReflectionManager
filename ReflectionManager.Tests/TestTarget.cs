using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.CompilerServices;

namespace ReflectionManager.Tests
{
    public abstract class TestTarget
    {
        private List<TargetItem> m_List = new List<TargetItem>();

        public static TestTargets.FieldTestTarget<T> CreateFieldTestTarget<T>()
        {
            return new TestTargets.FieldTestTarget<T>();
        }

        public static TestTargets.MethodTestTarget CreateMethodTestTarget()
        {
            return new TestTargets.MethodTestTarget();
        }

        public static TestTargets.PropertyTestTarget<T> CreatePropertyTestTarget<T>()
        {
            return new TestTargets.PropertyTestTarget<T>();
        }

        private class TargetItem
        {

            private string m_Name;
            private object m_Object;
            private Type m_Type;

            public TargetItem(string name, object value)
            {
                m_Name = name;
                m_Object = value == null ? "null" : value;
                m_Type = value == null ? typeof(object) : value.GetType();
            }

            public override string ToString()
            {
                return string.Format("[ Name = {0}, Value = {1}, Type = {2} ]", m_Name, m_Object, m_Type.Name);
            }
        }

        protected void Add(string name, object value)
        {
            m_List.Add(new TargetItem(name, value));
        }

        protected void Clear()
        {
            m_List.Clear();
        }

        protected static void TraceMessageRun(
            [CallerMemberName]string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            var s = string.Format("{0}:{1} - {2}: {3}", file, line, member, "実行されました。");
            Console.WriteLine(s);
        }

        protected static void TraceMessage(string message,
            [CallerMemberName]string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            var s = string.Format("{0}:{1} - {2}: {3}", file, line, member, message);
            Console.WriteLine(s);
        }


        public override string ToString()
        {
            var builder = new StringBuilder();
            int count = 0;
            foreach (var item in m_List)
            {
                builder.Append("[").Append(count).Append("]:").AppendLine(item.ToString());
                count++;
            }
            return builder.ToString();
        }

    }

}
