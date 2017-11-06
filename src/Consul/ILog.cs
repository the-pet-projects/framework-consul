namespace PetProjects.Framework.Consul
{
    using System;

    public interface ILog
    {
        void Error(string keyCouldnTBeLoadedFromConsulAndIsnTBeingWatched, Func<object> func);
        void Info(string successfulyRetrievedInitialValueFromConsul, Func<object> func);
        void Warning(string keyDoesnTExistInConsulActionWillBeExecutedWhenTheKeyIsCreated, Func<object> func);
        void Warning(string v);
    }
}