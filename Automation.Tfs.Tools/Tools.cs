using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Net;
using Microsoft.TeamFoundation.Framework.Client;


namespace OpenAgile.Automation.Tfs
{
    public static class Tools
    {
        public static void Subscribe(string collection, string listener, string tfsEvent, string eventTag, string user, string password)
        {
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
            if (!String.IsNullOrWhiteSpace(user))
                nc = new System.Net.NetworkCredential(user, password);

            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(collection), nc);
            // Get Eventing Service
            var eventService = (IEventService)tpc.GetService(typeof(IEventService));

            // Set delivery preferences
            var dPref = new DeliveryPreference { Schedule = DeliverySchedule.Immediate, Address = listener, Type = DeliveryType.Soap };

            // Unsubscribe to all events
            foreach (var s in eventService.GetEventSubscriptions(tpc.AuthorizedIdentity.Descriptor, eventTag))
            {
                eventService.UnsubscribeEvent(s.ID);
            }

            var filter = string.Empty;
            if (tfsEvent.Contains(","))
            {
                var lstEvents = tfsEvent.Split(new char[]{','});
                foreach (var e in lstEvents)
                {
                    eventService.SubscribeEvent(e, filter, dPref, eventTag);
                }
            }
            else
            {
                eventService.SubscribeEvent(tfsEvent, filter, dPref, eventTag);
            }
            

        }
        public static void BuildDefinition(string collection, string teamProject, string buildController, string buildName, string buildDescription, string user, string password)
        {
            string teamProjectPath = "$/" + teamProject;
            // Get a reference to our Team Foundation Server.
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
            if (!String.IsNullOrWhiteSpace(user))
                nc = new System.Net.NetworkCredential(user, password);

            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(collection),nc);
            tpc.EnsureAuthenticated();
            VersionControlServer versionControl = tpc.GetService<VersionControlServer>();
            IBuildServer buildServer = tpc.GetService<IBuildServer>();

            //Create build definition and give it a name and desription
            IBuildDefinition buildDef = buildServer.CreateBuildDefinition(teamProject);
            buildDef.Name = buildName;
            buildDef.Description = buildDescription;
            buildDef.ContinuousIntegrationType = ContinuousIntegrationType.Individual; //CI

            //Controller and default build process template
            buildDef.BuildController = buildServer.GetBuildController(buildController);
            var defaultTemplate = buildServer.QueryProcessTemplates(teamProject).First(p => p.TemplateType == ProcessTemplateType.Default);
            buildDef.Process = defaultTemplate;

            buildDef.Workspace.AddMapping(teamProjectPath, "$(SourceDir)", WorkspaceMappingType.Map);
            buildDef.Workspace.AddMapping(teamProjectPath+"/Drops", "$(SourceDir)", WorkspaceMappingType.Cloak);
            //What to build
            string pattern = "$/*.sln";
            var lists = versionControl.GetItems(pattern, VersionSpec.Latest, RecursionType.Full, DeletedState.NonDeleted, ItemType.File);
            var processParams = new Dictionary<string, string[]>();

            
            if (lists.Items.Any())
            {
                var list = lists.Items
                .Select(i => i.ServerItem)
                .ToList();
                processParams.Add("ProjectsToBuild", list.ToArray());
            }
            processParams.Add("ConfigurationsToBuild", new[] { "Any CPU|Debug" });

            buildDef.ProcessParameters = SerializeParams(processParams);

            buildDef.RetentionPolicyList.Clear();
            buildDef.AddRetentionPolicy(BuildReason.Triggered, BuildStatus.Succeeded, 10, DeleteOptions.All);
            buildDef.AddRetentionPolicy(BuildReason.Triggered, BuildStatus.Failed, 10, DeleteOptions.All);
            buildDef.AddRetentionPolicy(BuildReason.Triggered, BuildStatus.Stopped, 1, DeleteOptions.All);
            buildDef.AddRetentionPolicy(BuildReason.Triggered, BuildStatus.PartiallySucceeded, 10, DeleteOptions.All);

            //Lets save it
            buildDef.Save();
        }
        public static void CheckInFolder(string collection, string teamProject, string workspaceName, string localDir, string user, string password)
        {
            string teamProjectPath = "$/" + teamProject;
            // Get a reference to our Team Foundation Server.
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
            if (!String.IsNullOrWhiteSpace(user))
                nc = new System.Net.NetworkCredential(user, password);

            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(collection), nc);

            tpc.EnsureAuthenticated();
            // Get a reference to Version Control.
            VersionControlServer versionControl = tpc.GetService<VersionControlServer>();

            // Create a workspace.
            Workspace workspace = versionControl.CreateWorkspace(workspaceName, versionControl.AuthorizedUser);

            //Create a mapping
            workspace.Map(teamProjectPath, localDir);

            //Now add everything
            workspace.PendAdd(localDir, true);

            PendingChange[] pendingChanges = workspace.GetPendingChanges();

            //Checkin the items we added
            int changesetNumber = workspace.CheckIn(pendingChanges, "Sample changes");
        }
        private static string SerializeParams(Dictionary<string, string[]> parameters) 
        {
            string output = string.Empty;
            output += "<Dictionary x:TypeArguments=\"x:String, x:Object\" xmlns=\"clr-namespace:System.Collections.Generic;assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";
            foreach (var item in parameters)
            {
                output += "<x:Array x:Key=\""+ item.Key +"\" Type=\"x:String\">";
                foreach (var val in item.Value)
                {
                    output += "<x:String>" + val + "</x:String>";
                }
                output += "</x:Array>";
            }
            output += "</Dictionary>";
            return output;
        } 
    }
}
