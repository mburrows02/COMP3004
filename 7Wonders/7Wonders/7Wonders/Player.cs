﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _7Wonders
{
    public class Player
    {
        string name;

        public Player(string _name)
        {
            name = _name;
        }

        public string getName()
        {
            return name;
        }

    }
}