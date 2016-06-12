using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager.SampleConsole
{
    public class SampleObject : Tests.TestTarget, IEquatable<SampleObject>
    {
        /// <summary>
        /// プライベートフィールド
        /// </summary>
        private int m_Value = 100;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<int> Scores { get; set; }

        public SampleEnum MyEnum;

        private static int Number
        {
            get;
            set;
        }

        public static int WrapperNumber
        {
            get
            {
                return Number;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(SampleObject other)
        {
            var result = (
                Id == other.Id &&
                Name == other.Name &&
                Address == other.Address &&
                MyEnum == other.MyEnum &&
                Scores.Count == other.Scores.Count &&
                m_Value == other.m_Value);
            if (!result)
                return false;
            for (int i = 0; i < Scores.Count; i++)
            {
                if (Scores[i] != other.Scores[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is SampleObject)
                return Equals((SampleObject)obj);
            return base.Equals(obj);
        }

        public override string ToString()
        {
            Clear();
            Add(nameof(m_Value), m_Value);
            Add(nameof(Id), Id);
            Add(nameof(Name), Name);
            Add(nameof(Address), Address);
            Add(nameof(Scores.Count), Scores.Count);
            Scores.ForEach(o => { Add("Scores.Value = ", o); });
            return base.ToString();
        }
    }

}
