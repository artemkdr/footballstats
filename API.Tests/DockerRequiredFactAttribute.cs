using System.Diagnostics;

namespace API.Tests
{
    public class DockerRequiredFactAttribute : FactAttribute
    {
        public override string? Skip
        {
            get
            {
                if (!IsDockerInstalled() || !IsDockerAvailable())
                {
                    return "Docker is not installed or running.";
                }
                return null; // Don't skip the test
            }
        }

        private bool? dockerAvailable = null;

        private bool IsDockerAvailable()
        {
            if (dockerAvailable != null)
                return dockerAvailable.Value;
            try
            {
                var container = TestContainers.GetPostgresContainer();
                dockerAvailable = container != null;                
            } catch
            {
                dockerAvailable = false;
            }
            return dockerAvailable.Value;
        }

        private bool? dockerInstalled = null;
        private bool IsDockerInstalled()
        {
            if (dockerInstalled != null)
                return dockerInstalled.Value;
            try
            {
                Process.Start(new ProcessStartInfo("docker", "--version") { UseShellExecute = false })?.WaitForExit();
                dockerInstalled = true;
                return true;
            }
            catch (Exception)
            {
                dockerInstalled = false;
                return false;
            }
        }
    }
}
