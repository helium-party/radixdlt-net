namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// Dummy class to fullfill the interface policies to always return a <see cref="IRadixNodeAction"/> in <see cref="System.IObservable{T}"/> 
    /// </summary>
    public class DummyAction : IRadixNodeAction
    {
        public RadixNode Node => throw new System.InvalidOperationException("This action does not contain any valid information");
    }
}
