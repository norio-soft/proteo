using System.Web.Optimization;

namespace Orchestrator.WebUI
{
    public class LessTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            response.Content = dotless.Core.Less.Parse(response.Content);
            response.ContentType = "text/css";
        }
    }

    public class LessBundle : Bundle
    {
        public LessBundle(string virtualPath) : base(virtualPath, new IBundleTransform[2]
            {
                new LessTransform(),
                new CssMinify()
            })
        { }

        public LessBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath, new IBundleTransform[2]
            {
                new LessTransform(),
                new CssMinify()
            })
        { }
    }
}