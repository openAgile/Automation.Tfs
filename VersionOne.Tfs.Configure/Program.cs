using CLAP;
using VersionOne.Tfs;

namespace VersionOne.Tfs
{
    class Configure
    {
        static void Main(string[] args)
        {
            Parser.RunConsole<Commands>(args);
            
        }
    }
    class Commands
    {
        [Verb(Description="Creates a build definition")]
        public static void BuildDefinition(
            [Required]
            [Description("Tfs complete URL including collection")]
            string tfsTeamProjectCollection,
            [Required]
            [Description("Tfs Team Project name")]
            string teamProject,
            [Required]
            [Description("Tfs Build Controller name")]
            string buildController,
            [Required]
            [Description("Build name")]
            string buildName,
            [Required]
            [Description("Build description")]
            string buildDescription,
            [Required]
            [Description("Tfs Project Administrator user")]
            string user,
            [Required]
            [Description("Tfs Project Administrator user")]
            string password)
        {
            Tools.BuildDefinition(tfsTeamProjectCollection, teamProject, buildController, buildName, buildDescription, user, password);
        }
        [Verb(Description = "Checks In a complete folder")]
        public static void CheckInFolder(
            [Required]
            [Description("Tfs complete URL including collection")]
            string tfsTeamProjectCollection,
            [Required]
            [Description("Tfs Team Project name")]
            string teamProject,
            [Required]
            [Description("Tfs Workspace name")]
            string workspaceName,
            [Required]
            [Description("Local folder to upload where .sln file is located")]
            string localDir,
            [Required]
            [Description("Tfs Project Administrator user")]
            string user,
            [Required]
            [Description("Tfs Project Administrator user")]
            string password)
        {
            Tools.CheckInFolder(tfsTeamProjectCollection, teamProject, workspaceName, localDir, user, password);
        }
    }
}