using ModelsBuilder;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace AutAndAutV10.Services.Interfaces
{
    public interface ISiteSettingsService
    {
        SiteSettings GetSiteSettings(IPublishedContent currentPage, IPublishedValueFallback publishedValueFallback);
    }
}
