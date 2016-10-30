using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace jqWidgetWebPart.Features.EnsureSiteAssetsFeature
{
    /// <summary>
    /// Этот класс обрабатывает события, возникающие в ходе активации, деактивации, установки, удаления и обновления компонентов.
    /// </summary>
    /// <remarks>
    /// GUID, присоединенный к этому классу, может использоваться при создании пакета и не должен изменяться.
    /// </remarks>

    [Guid("1e4852c0-faf6-42ef-909a-79ae39307207")]
    public class EnsureSiteAssetsFeatureEventReceiver : SPFeatureReceiver
    {
        // Раскомментируйте ниже метод для обработки события, возникающего после активации компонента.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            (properties.Feature.Parent as SPSite).RootWeb.Lists.EnsureSiteAssetsLibrary();
        }


        // Раскомментируйте ниже метод для обработки события, возникающего перед деактивацией компонента.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Раскомментируйте ниже метод для обработки события, возникающего после установки компонента.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Раскомментируйте ниже метод для обработки события, возникающего перед удалением компонента.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Раскомментируйте ниже метод для обработки события, возникающего при обновлении компонента.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
