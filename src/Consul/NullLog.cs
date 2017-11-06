namespace PetProjects.Framework.Consul
{
    using System;

    public class NullLog : ILog
    {
        public void Error(string keyCouldnTBeLoadedFromConsulAndIsnTBeingWatched, Func<object> func)
        {
        }

        public void Info(string successfulyRetrievedInitialValueFromConsul, Func<object> func)
        {
        }

        public void Warning(string keyDoesnTExistInConsulActionWillBeExecutedWhenTheKeyIsCreated, Func<object> func)
        {
        }

        public void Warning(string v)
        {
        }
    }
}