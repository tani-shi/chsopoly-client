using System;
using Chsopoly.GameScene.Ingame.Object.State;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object
{
    public abstract class BaseObject<STATE, ANIM, OBJ> : MonoBehaviour where STATE : struct, IConvertible where ANIM : struct, IConvertible where OBJ : BaseObject<STATE, ANIM, OBJ>
    {
        public BaseStateMachine<STATE, ANIM, OBJ> StateMachine
        {
            get
            {
                if (_stateMachine == null)
                {
                    _stateMachine = GetComponent<BaseStateMachine<STATE, ANIM, OBJ>> ();
                }
                return _stateMachine;
            }
        }

        public BaseObjectModel<ANIM> Model
        {
            get
            {
                if (_model == null)
                {
                    _model = GetComponent<BaseObjectModel<ANIM>> ();
                }
                return _model;
            }
        }

        public bool IsReady
        {
            get
            {
                return _initialized;
            }
        }

        private BaseStateMachine<STATE, ANIM, OBJ> _stateMachine = null;
        private BaseObjectModel<ANIM> _model = null;
        private bool _initialized = false;

        public virtual void Initialize (int id)
        {
            _initialized = true;
        }

        protected virtual void Update ()
        {
            if (!_initialized)
            {
                return;
            }
            StateMachine.Execute (this as OBJ);
        }
    }
}