using AutAndAutV10.Services.Interfaces;
using ModelsBuilder;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace AutAndAutV10.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {

        public SiteSettingsService()
        {

        }
        public SiteSettings GetSiteSettings(IPublishedContent currentPage, IPublishedValueFallback publishedValueFallback)
        {
            var rootSite = currentPage.Root();
            var siteSettings = rootSite.Children.FirstOrDefault(x => x.ContentType.Alias.Equals("siteSettings"));

            return new SiteSettings(siteSettings, publishedValueFallback); ;
        }
    }
}
