using SP2013Access.Controls.PropertyGrid.Editors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class PropertyGridUtilities
    {
        internal static T GetAttribute<T>(PropertyDescriptor property) where T : Attribute
        {
            return property.Attributes.OfType<T>().FirstOrDefault<T>();
        }

        internal static bool IsSameForAllObject(IEnumerable objectList, Func<object, object> f, out object result)
        {
            result = null;
            bool flag = false;
            foreach (object current in objectList)
            {
                object obj = f(current);
                if (!flag)
                {
                    result = obj;
                    flag = true;
                }
                else
                {
                    if (!object.Equals(result, obj))
                    {
                        result = null;
                        return false;
                    }
                }
            }
            return true;
        }

        internal static ITypeEditor CreateDefaultEditor(Type propertyType, TypeConverter typeConverter)
        {
            ITypeEditor result;
            if (propertyType == typeof(string))
            {
                result = new TextBoxEditor();
            }
            else
            {
                if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                {
                    result = new CheckBoxEditor();
                }
                else
                {
                    if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                    {
                        result = new TextBoxEditor();
                        //result = new DecimalUpDownEditor();
                    }
                    else
                    {
                        if (propertyType == typeof(double) || propertyType == typeof(double?))
                        {
                            result = new TextBoxEditor();
                            //result = new DoubleUpDownEditor();
                        }
                        else
                        {
                            if (propertyType == typeof(int) || propertyType == typeof(int?))
                            {
                                result = new TextBoxEditor();
                                //result = new IntegerUpDownEditor();
                            }
                            else
                            {
                                if (propertyType == typeof(short) || propertyType == typeof(short?))
                                {
                                    result = new TextBoxEditor();
                                    //result = new ShortUpDownEditor();
                                }
                                else
                                {
                                    if (propertyType == typeof(long) || propertyType == typeof(long?))
                                    {
                                        result = new TextBoxEditor();
                                        //result = new LongUpDownEditor();
                                    }
                                    else
                                    {
                                        if (propertyType == typeof(float) || propertyType == typeof(float?))
                                        {
                                            result = new TextBoxEditor();
                                            //result = new SingleUpDownEditor();
                                        }
                                        else
                                        {
                                            if (propertyType == typeof(byte) || propertyType == typeof(byte?))
                                            {
                                                result = new TextBoxEditor();
                                                //result = new ByteUpDownEditor();
                                            }
                                            else
                                            {
                                                if (propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
                                                {
                                                    result = new TextBoxEditor();
                                                    //result = new SByteUpDownEditor();
                                                }
                                                else
                                                {
                                                    if (propertyType == typeof(uint) || propertyType == typeof(uint?))
                                                    {
                                                        result = new TextBoxEditor();
                                                        //result = new UIntegerUpDownEditor();
                                                    }
                                                    else
                                                    {
                                                        if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
                                                        {
                                                            result = new TextBoxEditor();
                                                            //result = new ULongUpDownEditor();
                                                        }
                                                        else
                                                        {
                                                            if (propertyType == typeof(ushort) || propertyType == typeof(ushort?))
                                                            {
                                                                result = new TextBoxEditor();
                                                                //result = new UShortUpDownEditor();
                                                            }
                                                            else
                                                            {
                                                                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                                                                {
                                                                    result = new TextBoxEditor();
                                                                    //result = new DateTimeUpDownEditor();
                                                                }
                                                                else
                                                                {
                                                                    if (propertyType == typeof(Color))
                                                                    {
                                                                        result = new TextBoxEditor();
                                                                        //result = new ColorEditor();
                                                                    }
                                                                    else
                                                                    {
                                                                        //if (propertyType.IsEnum)
                                                                        //{
                                                                        //    result = new EnumComboBoxEditor();
                                                                        //}
                                                                        //else
                                                                        //{
                                                                        if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
                                                                        {
                                                                            result = new TextBoxEditor();
                                                                            //result = new TimeSpanUpDownEditor();
                                                                        }
                                                                        else
                                                                        {
                                                                            if (propertyType == typeof(FontFamily) || propertyType == typeof(FontWeight) || propertyType == typeof(FontStyle) || propertyType == typeof(FontStretch))
                                                                            {
                                                                                result = new TextBoxEditor();
                                                                                //result = new FontComboBoxEditor();
                                                                            }
                                                                            else
                                                                            {
                                                                                if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                                                                                {
                                                                                    result = new TextBoxEditor();
                                                                                    //    result = new MaskedTextBoxEditor
                                                                                    //    {
                                                                                    //        ValueDataType = propertyType,
                                                                                    //        Mask = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"
                                                                                    //    };
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (propertyType == typeof(char) || propertyType == typeof(char?))
                                                                                    {
                                                                                        result = new TextBoxEditor();
                                                                                        //result = new MaskedTextBoxEditor
                                                                                        //{
                                                                                        //    ValueDataType = propertyType,
                                                                                        //    Mask = "&"
                                                                                        //};
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (propertyType == typeof(object))
                                                                                        {
                                                                                            result = new TextBoxEditor();
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            //Type listItemType = ListUtilities.GetListItemType(propertyType);
                                                                                            //if (listItemType != null)
                                                                                            //{
                                                                                            //    if (!listItemType.IsPrimitive && !listItemType.Equals(typeof(string)) && !listItemType.IsEnum)
                                                                                            //    {
                                                                                            //        result = new CollectionEditor();
                                                                                            //    }
                                                                                            //    else
                                                                                            //    {
                                                                                            //        result = new PrimitiveTypeCollectionEditor();
                                                                                            //    }
                                                                                            //}
                                                                                            //else
                                                                                            //{
                                                                                            ITypeEditor arg_43B_0;
                                                                                            if (typeConverter == null || !typeConverter.CanConvertFrom(typeof(string)))
                                                                                            {
                                                                                                ITypeEditor typeEditor = new TextBlockEditor();
                                                                                                arg_43B_0 = typeEditor;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                arg_43B_0 = new TextBoxEditor();
                                                                                            }
                                                                                            result = arg_43B_0;
                                                                                            //}
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        //}
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        internal static BindingBase GetDefaultBinding(PropertyItemBase propertyItem)
        {
            return propertyItem.ParentNode.ContainerHelper.CreateChildrenDefaultBinding(propertyItem);
        }

        internal static FrameworkElement GenerateSystemDefaultEditingElement(Type propertyType, PropertyItemBase propertyItem)
        {
            EditorDefinitionBase editorDefinitionBase = PropertyGridUtilities.GetDefaultEditorDefinition(propertyType);
            if (editorDefinitionBase == null)
            {
                IEnumerable defaultComboBoxDefinitionItems = PropertyGridUtilities.GetDefaultComboBoxDefinitionItems(propertyType);
                if (defaultComboBoxDefinitionItems != null)
                {
                    editorDefinitionBase = new EditorComboBoxDefinition
                    {
                        ItemsSource = defaultComboBoxDefinitionItems,
                        SelectedItemBinding = PropertyGridUtilities.GetDefaultBinding(propertyItem)
                    };
                }
                //else
                //{
                //    Type listItemType = ListUtilities.GetListItemType(propertyType);
                //    if (listItemType != null)
                //    {
                //        if (!listItemType.IsPrimitive && !listItemType.Equals(typeof(string)) && !listItemType.IsEnum)
                //        {
                //            editorDefinitionBase = new EditorCollectionDefinition
                //            {
                //                NewItemTypes = new List<Type>
                //                {
                //                    listItemType
                //                }
                //            };
                //        }
                //        else
                //        {
                //            editorDefinitionBase = new EditorPrimitiveTypeCollectionDefinition();
                //        }
                //    }
                //}
            }
            if (editorDefinitionBase == null)
            {
                return null;
            }
            return editorDefinitionBase.GenerateEditingElementInternal(propertyItem);
        }

        internal static FrameworkElement GenerateSystemDefaultEditingElement(PropertyItemBase propertyItem)
        {
            PropertyGridEditorTextBlock propertyGridEditorTextBlock = new PropertyGridEditorTextBlock();
            propertyGridEditorTextBlock.Margin = new Thickness(5.0, 0.0, 0.0, 0.0);
            BindingOperations.SetBinding(propertyGridEditorTextBlock, TextBlock.TextProperty, PropertyGridUtilities.GetDefaultBinding(propertyItem));
            return propertyGridEditorTextBlock;
        }

        internal static EditorDefinitionBase GetDefaultEditorDefinition(Type propertyType)
        {
            //Func<Type, object> func = delegate(Type t)
            //{
            //    if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
            //    {
            //        return Activator.CreateInstance(t);
            //    }
            //    return null;
            //};
            if (propertyType == typeof(string))
            {
                return new EditorTextDefinition();
            }
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return new EditorCheckBoxDefinition
                {
                    IsThreeState = propertyType == typeof(bool?)
                };
            }
            //if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            //{
            //    return new EditorDateTimeUpDownDefinition
            //    {
            //        DefaultValue = (DateTime?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(Color))
            //{
            //    return new EditorColorPickerDefinition();
            //}
            //if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            //{
            //    return new EditorDecimalUpDownDefinition
            //    {
            //        DefaultValue = (decimal?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(double) || propertyType == typeof(double?))
            //{
            //    return new EditorDoubleUpDownDefinition
            //    {
            //        DefaultValue = (double?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(float) || propertyType == typeof(float?))
            //{
            //    return new EditorSingleUpDownDefinition
            //    {
            //        DefaultValue = (float?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(byte) || propertyType == typeof(byte?))
            //{
            //    return new EditorByteUpDownDefinition
            //    {
            //        DefaultValue = (byte?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
            //{
            //    return new EditorSByteUpDownDefinition
            //    {
            //        DefaultValue = (sbyte?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(short) || propertyType == typeof(short?))
            //{
            //    return new EditorShortUpDownDefinition
            //    {
            //        DefaultValue = (short?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(ushort) || propertyType == typeof(ushort?))
            //{
            //    return new EditorUShortUpDownDefinition
            //    {
            //        DefaultValue = (ushort?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(int) || propertyType == typeof(int?))
            //{
            //    return new EditorIntegerUpDownDefinition
            //    {
            //        DefaultValue = (int?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(uint) || propertyType == typeof(uint?))
            //{
            //    return new EditorUIntegerUpDownDefinition
            //    {
            //        DefaultValue = (uint?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(long) || propertyType == typeof(long?))
            //{
            //    return new EditorLongUpDownDefinition
            //    {
            //        DefaultValue = (long?)func(propertyType)
            //    };
            //}
            //if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
            //{
            //    return new EditorULongUpDownDefinition
            //    {
            //        DefaultValue = (ulong?)func(propertyType)
            //    };
            //}
            return null;
        }

        internal static IEnumerable GetDefaultComboBoxDefinitionItems(Type propertyType)
        {
            //if (propertyType == typeof(FontFamily))
            //{
            //    return FontUtilities.Families;
            //}
            //if (propertyType == typeof(FontWeight))
            //{
            //    return FontUtilities.Weights;
            //}
            //if (propertyType == typeof(FontStyle))
            //{
            //    return FontUtilities.Styles;
            //}
            //if (propertyType == typeof(FontStretch))
            //{
            //    return FontUtilities.Stretches;
            //}
            if (propertyType != null && propertyType.IsEnum)
            {
                return Enum.GetValues(propertyType);
            }
            return null;
        }
    }
}