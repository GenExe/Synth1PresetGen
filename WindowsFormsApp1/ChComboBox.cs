using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MidiVstTest.Test
{
    class ChComboBox : ComboBox
    {
        public class ComboboxData
        {
            string TotalData;

            private bool _checked;
            public bool Checked
            {
                set { _checked = value; }
                get { return _checked; }
            }

            private string _data;
            public string Data
            {
                set { _data = value; }
                get { return _data; }
            }

            public ComboboxData(string value, bool ischeck)
            {
                Data = value;
                Checked = ischeck;
            }

            public override string ToString()
            {
                return Data;
            }


        }

        public event EventHandler Checkchanged;

        public void CheckComboBox()
        {
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        }

        public List<ComboboxData> CheckItems
        {
            get
            {

                List<ComboboxData> newitems = new List<ComboboxData>();
                foreach (var item in Items)
                {
                    if (item is ComboboxData)
                    {
                        newitems.Add(item as ComboboxData);
                    }
                }
                return newitems;

            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            ComboboxData data = (ComboboxData)SelectedItem;
            data.Checked = !data.Checked;

            if (Checkchanged != null)
            {
                Checkchanged(data, e);
            }


        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            if (Items[e.Index] is ComboboxData)
            {
                ComboboxData data = Items[e.Index] as ComboboxData;
                CheckBoxRenderer.RenderMatchingApplicationState = true;
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.X, e.Bounds.Y), e.Bounds, data.Data, this.Font,
                    (e.State & DrawItemState.Focus) == 0, data.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
            }
            else
            {
                e.Graphics.DrawString(Items[e.Index].ToString(), this.Font, Brushes.Black, new Point(e.Bounds.X, e.Bounds.Y));
                return;
            }

            base.OnDrawItem(e);
        }
    }
}
