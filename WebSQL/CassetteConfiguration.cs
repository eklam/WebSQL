using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;
using System.IO;
using System.Text.RegularExpressions;

namespace WebSQL
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            // TODO: Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

            // This default configuration treats each file as a separate 'bundle'.
            // In production the content will be minified, but the files are not combined.
            // So you probably want to tweak these defaults!
            //bundles.AddPerIndividualFile<StylesheetBundle>("Content");


            // To combine files, try something like this instead:
            bundles.Add<StylesheetBundle>("Content");
           // bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            //bundles.AddPerIndividualFile<ScriptBundle>("Scripts", new FileSearch
            //{
            //    Pattern = "*.js;*.coffee",
            //    SearchOption = SearchOption.AllDirectories, // The default
            //    Exclude = new Regex("-vsdoc\\.js$") // The default
            //});
            // In production mode, all of ~/Content will be combined into a single bundle.
            
            // If you want a bundle per folder, try this:
            //bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            
            // Each immediate sub-directory of ~/Scripts will be combined into its own bundle.
            // This is useful when there are lots of scripts for different areas of the website.
        }
    }
}