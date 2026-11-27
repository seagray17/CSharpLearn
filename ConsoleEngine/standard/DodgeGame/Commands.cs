using ConsoleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeGame2
{
 

    class MoveCommand : ICommand
    {
        private readonly DodgeGame _game;
        private readonly int _dx;

        public MoveCommand(DodgeGame game, int dx)
        {
            _game = game;
            _dx = dx;
        }

        public void Execute()
        {
            _game.MovePlayer(_dx);
        }
    }
}
