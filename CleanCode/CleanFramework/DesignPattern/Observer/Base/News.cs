namespace CleanCode.DesignPattern.Observer.Base
{
    public class News
    {
        private NewsCategory _category;
        private string _title;
        private string _content;
        private Publisher _publisher;

        public Publisher Publisher
        {
            get { return _publisher; }
            set { _publisher = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public NewsCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public News(Publisher publisher, NewsCategory category, string title, string content) {
            this._publisher = publisher;
            this._category = category;
            this._title = title;
            this._content = content;
        }

        public News() {
            this._publisher = null;
            this._category = NewsCategory.None;
            this._title = "";
            this._content = "";
        }
    }

    public enum NewsCategory
    {
        None=0,
        Sport = 1,
        Magazine = 2,
        WeatherCast = 3,
        Economy = 4,
        International = 5
    }
}
