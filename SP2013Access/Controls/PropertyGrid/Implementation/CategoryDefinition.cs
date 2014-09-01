namespace SP2013Access.Controls.PropertyGrid
{
    public class CategoryDefinition : DefinitionBase
    {
        private string _name;
        private int? _displayOrder = new int?(2147483647);

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                base.ThrowIfLocked<string>(() => this.Name);
                this._name = value;
            }
        }

        public int? DisplayOrder
        {
            get
            {
                return this._displayOrder;
            }
            set
            {
                base.ThrowIfLocked<int?>(() => this.DisplayOrder);
                this._displayOrder = value;
            }
        }
    }
}