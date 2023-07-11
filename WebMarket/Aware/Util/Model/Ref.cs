using System;

namespace Aware.Util.Model
{
    public class Ref
    {
        //Operator overloading
        public static bool operator ==(Ref x, Ref y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(Ref x, Ref y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public delegate void RowAddedHandler(object sender, RowEventArgs args);
        public event RowAddedHandler RowAdded;

        public void OnRowAdded(Object row)
        {
            if (RowAdded != null)
            {
                var args = new RowEventArgs(row);
                RowAdded(this, args);
            }
        }
    }

    public class RowEventArgs : System.EventArgs
    {
        public Object Row { get; set; }
        public RowEventArgs(Object row)
        {
            Row = row;
        }
    }
}
