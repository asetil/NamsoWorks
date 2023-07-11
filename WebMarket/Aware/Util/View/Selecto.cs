//ADD BELOW CSS TO PAGE
//body .wrp-select > div.preview { cursor:pointer;display:block;border:1px solid #78C12A; width: 134px; background: url('/resource/img/Icons/down.png') no-repeat 129px 11px !important; height: 20px; overflow: hidden;cursor: pointer;margin-right:10px;padding:2px 5px; }
//body .wrp-span { float:left;padding-right:10px;margin-top:4px;color: #76BF28; margin-left: 0; }
//body .wrp-select{position:relative;float:left;}
//body .wrp-select ul { z-index: 999999;position: absolute;top:26px;background-color: #fff;display: none;list-style:none;background:#fff;width:146px; }
//body .wrp-select ul.overflow { max-height: 200px; overflow-y: scroll; border: 1px solid #AADE70; z-index: 9999;}
//body .wrp-select ul > li { cursor:pointer;border:1px solid #AADE70; padding:3px 5px;border-top:0 none;}
//body .wrp-select ul > li:hover { background: #AADE70; color: #fff; }

using System.Collections.Generic;
using System.Text;
using Aware.Util.Model;

namespace Aware.Util.View
{
    public class Selecto
    {
        public string ID { get; set; }
        public string Css { get; set; }
        public string Title { get; set; }
        public string BlankOption { get; set; }
        public List<Item> DataSource { get; set; }
        public int SelectedIndex { get; set; }
        public string Attributes { get; set; }

        public Selecto(string title, List<Item> datasource, int selectedID = 0, string css = "",string id="",string blankOption="")
        {
            Title = title;
            DataSource = datasource;
            SelectedIndex = selectedID;
            ID = id;
            Css = css;
            BlankOption = blankOption;
        }

        public string Draw()
        {
            DataSource = DataSource ?? new List<Item>();
            var content = new StringBuilder();
            if (!string.IsNullOrEmpty(Title)) { content.AppendFormat("<span class='sbx-title'>{0}</span>", Title); }
            content.AppendFormat("<select id='{0}' name='{1}' class='sbx {2}' {3}>", ID, ID, Css, Attributes);

            if (!string.IsNullOrEmpty(BlankOption)) { content.AppendFormat("<option value='0'>{0}</option>", BlankOption); }
            foreach (var item in DataSource)
            {
                content.AppendFormat("<option value='{0}' {2}>{1}</option>", item.ID,item.Value, item.ID == SelectedIndex ?"selected='selected'":""); 
            }
            content.AppendFormat("</select>");
            return content.ToString();
        }

        public static string DrawIncremental(string id, string css,string value, string title="",string suffix="")
        {
            var content = new StringBuilder();
            if (!string.IsNullOrEmpty(title)) { content.AppendFormat("<span class='sbx-title'>{0}</span>", title); }
            content.AppendFormat("<div id='{0}' class='sbx {1}' data-unit='{2}'>", id, css,suffix);
            content.AppendFormat("<span class='nv fl left'>-</span>");
            content.AppendFormat("<span class='txt'>{0} {1}</span>", value, suffix);
            content.AppendFormat("<span class='nv fr right'>+</span>");
            content.AppendFormat("<input type='hidden' id='SelectedQuantity' value='1'>");
            content.AppendFormat("</div>");
            return content.ToString();
        }

        public string DrawFormatted()
        {
            DataSource = DataSource ?? new List<Item>();
            var content = new StringBuilder();
            if (!string.IsNullOrEmpty(Title)) { content.AppendFormat("<span class='wrp-span'>{0}</span>", Title); }
            content.AppendFormat("<div class='wrp-select {0}' {1}>", Css, Attributes);

            var selectedID = SelectedIndex;
            var selectedValue = "Seçiniz";

            var subContent = new StringBuilder();
            subContent.AppendFormat("<ul {0}>", DataSource.Count > 8 ? "class='overflow'" : "");
            if (!string.IsNullOrEmpty(BlankOption)) { subContent.AppendFormat("<li data-id='0'>{0}</li>", BlankOption); }
            foreach (var item in DataSource)
            {
                subContent.AppendFormat("<li data-id='{0}'>{1}</li>", item.ID, item.Value);
                if (item.ID == SelectedIndex) { selectedValue = item.Value; }
            }
            subContent.AppendFormat("</ul>");

            var idInfo = string.IsNullOrEmpty(ID) ? "" : string.Format("id='{0}'", ID);
            content.AppendFormat("<div {0} data-id='{1}' class='preview'>{2}</div>", idInfo, selectedID, selectedValue);
            content.AppendFormat(subContent.ToString());
            content.AppendFormat("</div>");
            return content.ToString();
        }
    }
}