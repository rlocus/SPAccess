using SP2013Access.Controls.Utilities;
using System;
using System.ComponentModel;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        private readonly LocalizationHelper _localizationHelper;
        private readonly string _descriptionResourceKey;

        public string CategoryValue
        {
            get
            {
                return this._descriptionResourceKey;
            }
        }

        public string LocalizedCategory
        {
            get
            {
                return this._localizationHelper.GetString(this._descriptionResourceKey);
            }
        }

        public LocalizedCategoryAttribute(string descriptionResourceKey, Type resourceClassType)
            : base(descriptionResourceKey)
        {
            this._localizationHelper = new LocalizationHelper(resourceClassType);
            this._descriptionResourceKey = descriptionResourceKey;
        }

        protected override string GetLocalizedString(string value)
        {
            return this.LocalizedCategory ?? base.GetLocalizedString(value);
        }
    }
}