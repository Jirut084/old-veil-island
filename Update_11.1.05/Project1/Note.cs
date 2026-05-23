using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.NPC
{
    public class Note
    {
        public string Id;       // note1, note2 …
        public string Text;     // ข้อความเฉพาะ note
        public bool Collected;  // เก็บแล้วหรือไม่

        public Note(string id, string text)
        {
            Id = id;
            Text = text;
            Collected = false;
        }
    }
}

