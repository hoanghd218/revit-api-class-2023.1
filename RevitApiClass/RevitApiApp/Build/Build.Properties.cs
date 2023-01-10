partial class Build
{
    readonly string[] Projects =
    {
        "RevitApiApp"
    };

    public const string InstallerProject = "Installer";

    public const string BuildConfiguration = "Release";
    public const string InstallerConfiguration = "Installer";
    public const string BundleConfiguration = "";

    const string AddInBinPrefix = "AddIn";
    const string ArtifactsFolder = "output";
}