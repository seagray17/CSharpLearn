using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class KeyMap
    {
        private readonly Dictionary<ConsoleKey, ICommand> _bindings = new Dictionary<ConsoleKey, ICommand>();

        public void Bind(ConsoleKey key, ICommand command)
        {
            _bindings[key] = command;               // 있으면 바꾸고, 없으면 더한다
        }

        public void Unbind(ConsoleKey key)
        {
            _bindings.Remove(key);
        }

        public bool IsBound(ConsoleKey key)
        {
            return _bindings.ContainsKey(key);
        }

        /// <summary>oldKey에 묶인 명령을 newKey로 옮긴다. 조작 바꾸기.</summary>
        public void Rebind(ConsoleKey oldKey, ConsoleKey newKey)
        {
            if (!_bindings.TryGetValue(oldKey, out ICommand? command)) return;
            _bindings.Remove(oldKey);
            _bindings[newKey] = command;
        }

        /// <summary>key에 묶인 명령이 있으면 실행하고 true, 없으면 false.</summary>
        public bool Handle(ConsoleKey key)
        {
            if (!_bindings.TryGetValue(key, out ICommand? command)) return false;
            command.Execute();
            return true;
        }
    }
}
