using SP2013Access.Controls.PropertyGrid.Converters;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SP2013Access.Controls.PropertyGrid
{
    public abstract class PropertyDefinitionBase : DefinitionBase
    {
        private IList _targetProperties;

        internal PropertyDefinitionBase()
        {
            _targetProperties = new List<object>();
        }

        [TypeConverter(typeof(ListConverter))]
        public IList TargetProperties
        {
            get { return _targetProperties; }
            set
            {
                this.ThrowIfLocked(() => this.TargetProperties);
                _targetProperties = value;
            }
        }

        internal override void Lock()
        {
            if (this.IsLocked)
                return;

            base.Lock();

            // Just create a new copy of the properties target to ensure
            // that the list doesn't ever get modified.

            var newList = new List<object>();
            if (_targetProperties != null)
            {
                foreach (object p in _targetProperties)
                {
                    object prop = p;
                    // Convert all TargetPropertyType to Types
                    var targetType = prop as TargetPropertyType;
                    if (targetType != null)
                    {
                        prop = targetType.Type;
                    }
                    newList.Add(prop);
                }
            }

            //In Designer Mode, the Designer is broken if using a ReadOnlyCollection
            _targetProperties = DesignerProperties.GetIsInDesignMode(this)
                                ? new Collection<object>(newList)
                                : new ReadOnlyCollection<object>(newList) as IList;
        }
    }
}