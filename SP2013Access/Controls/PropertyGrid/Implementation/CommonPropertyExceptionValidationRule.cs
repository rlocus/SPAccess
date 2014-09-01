﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using SP2013Access.Controls.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class CommonPropertyExceptionValidationRule : ValidationRule
    {
        private readonly TypeConverter _propertyTypeConverter;
        private readonly Type _type;

        internal CommonPropertyExceptionValidationRule(Type type)
        {
            _propertyTypeConverter = TypeDescriptor.GetConverter(type);
            _type = type;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);

            if (GeneralUtilities.CanConvertValue(value, _type))
            {
                try
                {
                    _propertyTypeConverter.ConvertFrom(value);
                }
                catch (Exception e)
                {
                    // Will display a red border in propertyGrid
                    result = new ValidationResult(false, e.Message);
                }
            }
            return result;
        }
    }
}