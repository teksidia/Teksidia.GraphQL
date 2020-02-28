using System;

namespace DataModel
{
    public class EntityAttribute : Attribute
    {
    }

    public class ChildAttribute : Attribute
    {
        public string LocalProperty { get; private set; }

        public string ForeignProperty { get; private set; }

        public string LocalProperty2 { get; private set; }

        public string ForeignProperty2 { get; private set; }

        public string LocalProperty3 { get; private set; }

        public string ForeignProperty3 { get; private set; }

        public string LocalProperty4 { get; private set; }

        public string ForeignProperty4 { get; private set; }

        public string LocalProperty5 { get; private set; }

        public string ForeignProperty5 { get; private set; }

        public ChildAttribute(
            string local,
            string foreign,
            string local2 = null,
            string foreign2 = null,
            string local3 = null,
            string foreign3 = null,
            string local4 = null,
            string foreign4 = null,
            string local5 = null,
            string foreign5 = null)
        {
            this.LocalProperty = local;
            this.ForeignProperty = foreign;
            this.LocalProperty2 = local2;
            this.ForeignProperty2 = foreign2;
            this.LocalProperty3 = local3;
            this.ForeignProperty3 = foreign3;
            this.LocalProperty4 = local4;
            this.ForeignProperty4 = foreign4;
            this.LocalProperty5 = local5;
            this.ForeignProperty5 = foreign5;
        }
    }
}
