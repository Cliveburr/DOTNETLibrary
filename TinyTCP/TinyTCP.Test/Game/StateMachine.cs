using System;
using System.Collections.Generic;
using System.Text;

namespace TinyTCP.Test.Game
{
    /* lista de possiveis ações
     * ao detectar o comando de uma possivel ação
     *  verificar se nenhuma ação já foi definida
     *  verificar com os states se aquela ação pode ser efetivada no client side (ex de não poder: uma ação sendo executada)
     *      definir como proxima ação
     *      efetuar chamada ao server para confirmar ação
     *  ao receber confirmação da ação do server
     *      dar inicio a ação efetuar a ação e limpar a proxima ação
     * 
     * 
     * se já não tiver nenhuma proxima mudança de state
     * verificar quando o client solicitar uma mudança de state
     *      definir como proxima state
     *      efetuar chamado ao server processar o proximo state
     * ao receber retorno do server
     *      liberar proxima state
     *      se aceito, enfileirar para executar
     * 
     * 
     */

    public abstract class BaseNode
    {
        public StateMachine MovingState { get; set; }
        public StateMachine FireState { get; set; }

        public void Process(float delta)
        {
            MovingState.Process(delta);
            FireState.Process(delta);
        }
    }

    public class StateMachine
    {
        public State ActualState { get; private set; }
        public State NextState { get; private set; }

        public void Process(float delta)
        {
            if (NextState == null)
            {
                NextState = ActualState.checkTransictions();
            }

            if (NextState != null)
            {
                if (NextState.canEnter() && ActualState.canLeave())
                {
                    ActualState.leave();
                    NextState.enter();
                    ActualState = NextState;
                    NextState = null;
                }
            }

            ActualState.process(delta);
        }

        public void SetFirstState(State state)
        {
            if (ActualState == null)
            {
                ActualState = state;
            }
        }
    }

    public interface State
    {
        void process(float delta);
        State checkTransictions();
        bool canEnter();
        void enter();
        bool canLeave();
        void leave();
    }
}