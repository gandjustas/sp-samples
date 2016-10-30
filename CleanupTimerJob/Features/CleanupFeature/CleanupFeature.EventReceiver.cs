using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace CleanupTimerJob.Features.CleanupFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("3da72aa9-213b-4d67-9672-9f63630333a1")]
    public class CleanupFeatureEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SetCleanupFlag(properties, true);
        }



        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SetCleanupFlag(properties, false);
        }



        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        private void SetCleanupFlag(SPFeatureReceiverProperties properties, bool flag)
        {
            var web = properties.Feature.Parent as SPWeb;
            web.Properties[Constants.FlagPropertyName] = flag.ToString();
            web.Properties.Update();
        }
    }
}
