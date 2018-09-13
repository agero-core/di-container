using Agero.Core.DIContainer.Attributes;

namespace Agero.Core.DIContainer.Tests.Types
{
    public class InjectAttributeHelperTestClass
    {
        #region Constructors

        [Inject]
        public InjectAttributeHelperTestClass(int i) { }

        [Inject]
        internal InjectAttributeHelperTestClass(double d) { }

        [Inject]
        protected InjectAttributeHelperTestClass(decimal d) { }

        [Inject]
        private InjectAttributeHelperTestClass(string s) { }

        public InjectAttributeHelperTestClass(bool b) { }
        
        #endregion


        #region Properties
        
        [Inject]
        public bool PublicProperty { get; set; }

        [Inject]
        public bool PublicPropertyWithInternalSettter { get; internal set; }

        [Inject]
        public bool PublicPropertyWithProtectedSettter { get; protected set; }

        [Inject]
        public bool PublicPropertyWithPrivateSettter { get; private set; }

        [Inject]
        public bool PublicPropertyWithoutSetter
        {
            get { return true; }
        }

        [Inject]
        internal bool InternalProperty { get; set; }

        [Inject]
        protected bool ProtectedProperty { get; set; }

        [Inject]
        private bool PrivateProperty { get; set; }
        
        public bool Property { get; set; }

        #endregion


        #region Methods
        
        public void Method() { }

        #endregion
    }
}