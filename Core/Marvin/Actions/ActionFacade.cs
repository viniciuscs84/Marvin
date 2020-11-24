using System;

namespace Marvin.Actions
{
    public interface IActionFacade
    {
        bool DoAction();
        //Task<bool> DoActionAsync();
    }

    public abstract class ActionFacade : IActionFacade
    {
        public Exception Exception { get; private set; }

        protected virtual void PrepareExecute() { }

        protected virtual void Execute() { }

        protected virtual void PostExecute() { }

        public bool DoAction()
        {
            try
            {
                PrepareExecute();
                Execute();
                PostExecute();
                return true;
            }
            catch(Exception ex)
            {
                Exception = ex;
            }
            return false;
        }
    }
}
