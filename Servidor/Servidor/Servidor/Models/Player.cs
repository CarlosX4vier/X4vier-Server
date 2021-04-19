using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor.Models
{
    public class Player
    {
        public int Id;
        public string Name;
        public Position Position = new Position();
    }
}
