using System;
using System.Collections.Generic;
using System.Text;

namespace TinyTCP.Test.Game
{
    public class PlayerNode
    {
        public StateMachine MainState { get; private set; }
        public string Input { get; set; }

        public PlayerNode()
        {
            MainState = new StateMachine();
            MainState.SetFirstState(new PlayerMainIdle(this));
        }

        public void Process(float delta)
        {
            MainState.Process(delta);
        }
    }

    public class PlayerMainIdle : State
    {
        private PlayerNode _player;

        public PlayerMainIdle(PlayerNode player)
        {
            _player = player;
        }

        public bool canEnter()
        {
            return true;
        }

        public bool canLeave()
        {
            return true;
        }

        public State checkTransictions()
        {
            var input = _player.Input;
            if (input == "key_up")
            {
                return new PlayerMainMoving(_player, Directions.Foward);
            }
            return null;
        }

        public void enter()
        {
        }

        public void leave()
        {
        }

        public void process(float delta)
        {
        }
    }

    public enum Directions
    {
        Foward = 0,
        Backward = 1,
        Left = 2,
        Right = 3
    }

    public class PlayerMainMoving : State
    {
        private PlayerNode _player;

        public Directions Direction { get; private set; }

        public PlayerMainMoving(PlayerNode player, Directions direction)
        {
            _player = player;
            Direction = direction;
        }

        public bool canEnter()
        {
            return true;
        }

        public bool canLeave()
        {
        }

        public State checkTransictions()
        {
            var input = _player.Input;
            if (input == "key_up")
            {
                return new PlayerMainMoving(_player, Directions.Foward);
            }
            return null;
        }

        public void enter()
        {
        }

        public void leave()
        {
        }

        public void process(float delta)
        {
        }
    }
}