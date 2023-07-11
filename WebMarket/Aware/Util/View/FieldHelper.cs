using System.Collections.Generic;
using System.Linq;
using Aware.Util.Model;

namespace Aware.Util.View
{
    public class FieldHelper
    {
        private IList<Field> _fields;
        public int TitleColumnCount { get; set; }
        public Enums.FieldDirection Direction { get; set; }
        public bool AllowNextField { get; set; }

        public FieldHelper(Enums.FieldDirection direction = Enums.FieldDirection.Vertical, int titleColumnCount = 2)
        {
            Direction = direction;
            AllowNextField = true;
            TitleColumnCount = titleColumnCount;
        }

        public IList<Field> Fields
        {
            get
            {
                if (_fields == null) { _fields = new List<Field>(); }
                return _fields;
            }
        }

        public Field Label(string title, string value, string css = "")
        {
            return SetField<Field>(Enums.FieldType.Label, title, value, string.Empty, css);
        }

        public Field Text(string id, string title, string value, string css = "", int maxLength = 0)
        {
            return SetField<Field>(Enums.FieldType.TextBox, title, value, id, css, maxLength);
        }

        public Field Pasword(string id, string title, string value, string css = "", int maxLength = 0)
        {
            return SetField<Field>(Enums.FieldType.Password, title, value, id, css, maxLength);
        }

        public Field CheckBox(string id, string title, bool value, string css = "")
        {
            return SetField<Field>(Enums.FieldType.CheckBox, title, value ? "true" : "false", id, css);
        }

        public Field Number(string id, string title, string value, string css = "", int maxLength = 0)
        {
            return SetField<Field>(Enums.FieldType.NumberBox, title, value, id, "text-right numeric " + css, maxLength);
        }


        public Field TxtArea(string id, string title, string value, string css = "", int maxLength = 0)
        {
            return SetField<Field>(Enums.FieldType.Textarea, title, value, id, css, maxLength);
        }

        public Field Image(string id, string title, string name, string path, string css = "")
        {
            var field = SetField<Field>(Enums.FieldType.Image, title, path, id, css);
            field.Extra = name;
            return field;
        }

        public BtnGroupField BtnGroup(string title, string id, List<Item> data, string value, string css = "")
        {
            var field = SetField<BtnGroupField>(Enums.FieldType.ButtonGroup, title, value, id, css);
            field.DataSource = data ?? new List<Item>();
            return field;
        }

        public BtnGroupField BtnGroup(string title, string id, List<Lookup.Lookup> data, string value, string css = "")
        {
            var dataList = data.Select(i => new Item(i.Value, i.Name)).ToList();
            var field = SetField<BtnGroupField>(Enums.FieldType.ButtonGroup, title, value, id, css);
            field.DataSource = dataList;
            return field;
        }

        public Field Select(string title, string id, IEnumerable<Item> data, int selectedID, string css = "", string blankOption = "Seçiniz", bool isMulti = false)
        {
            return Select(title, id, data, selectedID.ToString(), css, blankOption, isMulti);
        }

        public Field Select(string title, string id, IEnumerable<Lookup.Lookup> data, int selectedID, string css = "", string blankOption = "Seçiniz", bool isMulti = false)
        {
            var dataList = data.Select(i => new Item(i.Value, i.Name));
            return Select(title, id, dataList, selectedID.ToString(), css, blankOption, isMulti);
        }

        public Field Select(string title, string id, IEnumerable<Item> data, string selectedInfo, string css = "", string blankOption = "Seçiniz", bool isMulti = false)
        {
            var field = SetField<SelectField>(Enums.FieldType.Select, title, selectedInfo, id, css);
            field.DataSource = data != null ? data.ToList() : new List<Item>();
            field.BlankOption = blankOption;
            field.IsMulti = isMulti;
            field.Placeholder = Direction == Enums.FieldDirection.Inline ? title : string.Empty;
            return field;
        }

        public FieldHelper If(bool value)
        {
            AllowNextField = value;
            return this;
        }

        private T SetField<T>(Enums.FieldType type, string title, string value, string id, string css = "", int maxLength = 0) where T : Field, new()
        {
            if (!AllowNextField)
            {
                AllowNextField = true;
                return new T();
            }

            var field = new T
            {
                Type = type,
                Title = title,
                Value = value,
                ID = id,
                Placeholder = Direction == Enums.FieldDirection.Inline ? title : string.Empty,
                Css = css,
                Attributes = maxLength > 0 ? string.Format(" maxlength='{0}'", maxLength) : string.Empty
            };

            Fields.Add(field);
            return field;
        }

        public string TitleCss
        {
            get
            {
                if (Direction == Enums.FieldDirection.Horizantal)
                {
                    return string.Format("title col-sm-{0} control-label", TitleColumnCount);
                }
                return "title";
            }
        }
    }
}