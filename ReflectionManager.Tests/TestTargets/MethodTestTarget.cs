using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager.Tests.TestTargets
{
    public class MethodTestTarget : TestTarget
    {
        #region Static Action
        private static void PrivateStaticAction()
        {
            TraceMessageRun();
        }

        internal static void InternalStaticAction()
        {
            TraceMessageRun();
        }

        protected static void ProtectedStaticAction()
        {
            TraceMessageRun();
        }

        public static void PublicStaticAction()
        {
            TraceMessageRun();

        }

        #endregion

        #region Instance Action
        private void PrivateInstanceAction()
        {
            TraceMessageRun();

        }

        internal void InternalInstanceAction()
        {
            TraceMessageRun();

        }

        protected void ProtectedInstanceAction()
        {
            TraceMessageRun();

        }

        public void PublicInstanceAction()
        {
            TraceMessageRun();

        }
        #endregion

        #region Static Func<bool>

        private static bool PrivateStaticFunc()
        {
            TraceMessageRun();
            return true;
        }

        internal static bool InternalStaticFunc()
        {
            TraceMessageRun();
            return true;
        }

        protected static bool ProtectedStaticFunc()
        {
            TraceMessageRun();
            return true;
        }

        public static bool PublicStaticFunc()
        {
            TraceMessageRun();
            return true;
        }
        #endregion

        #region Instance Func<bool>

        private bool PrivateInstanceFunc()
        {
            TraceMessageRun();
            return true;
        }

        internal bool InternalInstanceFunc()
        {
            TraceMessageRun();
            return true;
        }

        protected bool ProtectedInstanceFunc()
        {
            TraceMessageRun();
            return true;
        }

        public bool PublicInstanceFunc()
        {
            TraceMessageRun();
            return true;
        }
        #endregion

        public override string ToString()
        {
            Clear();
            Add(nameof(PrivateStaticAction), null);
            Add(nameof(InternalStaticAction), null);
            Add(nameof(ProtectedStaticAction), null);
            Add(nameof(PublicStaticAction), null);

            Add(nameof(PrivateInstanceAction), null);
            Add(nameof(InternalInstanceAction), null);
            Add(nameof(ProtectedInstanceAction), null);
            Add(nameof(PublicInstanceAction), null);

            Add(nameof(PrivateStaticFunc), PrivateStaticFunc());
            Add(nameof(InternalStaticFunc), InternalStaticFunc());
            Add(nameof(ProtectedStaticFunc), ProtectedStaticFunc());
            Add(nameof(PublicStaticFunc), PublicStaticFunc());

            Add(nameof(PrivateInstanceFunc), PrivateInstanceFunc());
            Add(nameof(InternalInstanceFunc), InternalInstanceFunc());
            Add(nameof(ProtectedInstanceFunc), ProtectedInstanceFunc());
            Add(nameof(PublicInstanceFunc), PublicInstanceFunc());
            return base.ToString();
        }
    }

}
