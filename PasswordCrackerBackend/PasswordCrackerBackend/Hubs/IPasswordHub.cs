namespace PasswordCrackerBackend.Hubs
{
    public interface IPasswordHub
    {
        Task ProgressChanged(double progress);
    }
}
