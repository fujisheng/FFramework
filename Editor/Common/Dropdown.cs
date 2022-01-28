using System;
using UnityEngine;

namespace Framework.Editor
{
    public class Dropdown
    {
        bool open;
        Rect position;
        string[] names;
        int[] values;

        public int SelectedIndex { get; private set; }
        public int SelectedValue { get; private set; }

        public Dropdown(Rect position, params string[] names)
        {
            this.position = position;
            this.names = names;
            this.values = new int[names.Length];
            for (int i = 0; i < this.values.Length; i++)
            {
                values[i] = i;
            }
        }

        public Dropdown(Rect position, Type enumType)
        {
            this.position = position;
            this.names = enumType.GetEnumNames();
            var values = enumType.GetEnumValues();
            this.values = new int[values.Length];
            for (int i = 0; i < names.Length; i++)
            {
                this.values[i] = (int)values.GetValue(i);
            }
        }

        public void Draw()
        {
            if (GUI.Button(position, names[SelectedIndex]))
            {
                open = !open;
            }

            if (open)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (GUI.Button(new Rect(position.x, position.y + position.height * (i + 1), position.width, position.height), names[i]))
                    {
                        SelectedIndex = i;
                        SelectedValue = values[i];
                        open = false;
                    }
                }
            }
        }
    }
}