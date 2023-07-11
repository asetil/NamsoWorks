using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CleanCode.DesignPattern
{
    //Originator Class, we want to store original state
    public class GameWorld
    {
        public string Name { get; set; }
        public long Popluation { get; set; }

        public GameWorldMemento Save()
        {
            return new GameWorldMemento(this.Name, this.Popluation);
        }

        public void Load(GameWorldMemento deneme)
        {
            this.Name = deneme.Name;
            this.Popluation = deneme.Popluation;
        }

        public override string ToString()
        {
            return string.Format("{0} dünyasında {1} insan var.", Name, Popluation.ToString());
        }
    }

    //Memento Class
    public class GameWorldMemento
    {
        public string Name { get; set; }
        public long Popluation { get; set; }

        public GameWorldMemento(string name, long population)
        {
            this.Name = name;
            this.Popluation = population;
        }
    }

    //CareTaker Class, memento tipini güvenli şekilde saklar ve öğeleri üzerinde değişiklik yapmaz.
    public class GameWorldCareTaker
    {
        public GameWorldMemento World { get; set; }
    }

    public class Note
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateSaved { get; set; }
    }

    public class NoteTaker
    {
        private Note _note;
        private NoteHistory _history;

        public NoteTaker()
        {
            _history = new NoteHistory();
        }

        public void CreateNote()
        {
            _note = new Note();
        }

        public void Paste(string value)
        {
            _note.Content += value;
            _history.AddHistory("Paste", _note);
        }

        public void TypeText(string typedText)
        {
            _note.Content += typedText;
            _history.AddHistory("TypeText", _note);
        }

        public void ClearText()
        {
            _note.Content = string.Empty;
            _history.AddHistory("Clear", _note);
        }

        public void PreviousStep()
        {
            _note = _history.GetPrevious() ?? _note;
        }

        public void ToHistory(string historyAction)
        {
            _note = _history.GetHistoryItem(historyAction) ?? _note;
        }

        public void Show()
        {
            Console.WriteLine(_note.Content);
            Console.WriteLine("-----------------------------------------");
        }
    }

    public class NoteHistory
    {
        private Dictionary<string, Note> _noteList = new Dictionary<string, Note>();

        public void AddHistory(string actionType, Note note)
        {
            var noteToKeep = new Note() { Content = note.Content, DateSaved = DateTime.Now };
            var action = string.Format("{0}_{1}", actionType, DateTime.Now.ToShortTimeString());
            _noteList.Add(action, noteToKeep);
        }

        public Note GetPrevious()
        {
            var key = _noteList.Keys.ElementAt(_noteList.Count - 2);
            if (key != null)
            {
                var lastNote = _noteList[key];
                _noteList.Remove(key);
                return lastNote;
            }
            return null;
        }

        public Note GetHistoryItem(string action)
        {
            if (_noteList.ContainsKey(action))
            {
                return _noteList[action];
            }
            return null;
        }
    }
}
