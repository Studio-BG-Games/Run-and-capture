namespace Runtime.Controller
{
    public interface ILateExecute : IController
    {
        void LateExecute();
    }
}