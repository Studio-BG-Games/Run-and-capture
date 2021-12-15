using Runtime.Controller;

namespace Controller
{
    public interface IFixedExecute : IController
    {
        void FixedExecute();
    }
}