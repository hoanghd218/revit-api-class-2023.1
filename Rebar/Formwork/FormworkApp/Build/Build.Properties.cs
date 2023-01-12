partial class Build
{
    readonly string[] Projects =
    {
        "FormworkApp"
    };

    public const string InstallerProject = "Installer";

    public const string BuildConfiguration = "Release";
    public const string InstallerConfiguration = "Installer";

    const string AddInBinPrefix = "AddIn";
    const string ArtifactsFolder = "output";
}