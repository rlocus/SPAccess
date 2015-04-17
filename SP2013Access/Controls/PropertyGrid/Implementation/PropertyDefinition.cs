using System;

namespace SP2013Access.Controls.PropertyGrid
{
    public class PropertyDefinition : PropertyDefinitionBase
    {
        private bool? _isBrowsable = true;
        private bool? _isExpandable = null;
        private string _displayName = null;
        private string _description = null;
        private string _category = null;
        private int? _displayOrder = null;

        public string Category
        {
            get { return _category; }
            set
            {
                this.ThrowIfLocked(() => this.Category);
                _category = value;
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                this.ThrowIfLocked(() => this.DisplayName);
                _displayName = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                this.ThrowIfLocked(() => this.Description);
                _description = value;
            }
        }

        public int? DisplayOrder
        {
            get { return _displayOrder; }
            set
            {
                this.ThrowIfLocked(() => this.DisplayOrder);
                _displayOrder = value;
            }
        }

        public bool? IsBrowsable
        {
            get { return _isBrowsable; }
            set
            {
                this.ThrowIfLocked(() => this.IsBrowsable);
                _isBrowsable = value;
            }
        }

        public bool? IsExpandable
        {
            get { return _isExpandable; }
            set
            {
                this.ThrowIfLocked(() => this.IsExpandable);
                _isExpandable = value;
            }
        }

    //    internal override void Lock()
    //    {
    //        if (this.TargetProperties != null
    //          && this.TargetProperties.Count > 0)
    //        {
    //            throw new InvalidOperationException(
    //              string.Format(
    //                @"{0}: When using 'TargetProperties' property, do not use 'Name' property.",
    //                typeof(PropertyDefinition)));
    //        }
    //        base.Lock();
    //    }
    }
}