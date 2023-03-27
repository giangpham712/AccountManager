using System;

namespace AccountManager.Domain
{
    public class SoftwareComponentAttribute : Attribute
    {
        public SoftwareComponentAttribute(string displayName, string repoName, string buildFolderName)
        {
            DisplayName = displayName;
            RepoName = repoName;
            BuildFolderName = buildFolderName;
        }

        public string DisplayName { get; set; }
        public string RepoName { get; set; }
        public string BuildFolderName { get; set; }
    }
}