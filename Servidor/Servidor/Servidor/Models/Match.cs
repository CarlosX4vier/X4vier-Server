using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor.Models
{
    public class Match
    {
        public List<Player> Players = new List<Player>();
        public bool Finalized = false;
        public int Time = 0;
    }
}
