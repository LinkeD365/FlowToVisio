using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace LinkeD365.FlowToVisio
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin))]
    [ExportMetadata("Name", "Flow Visio Builder")]
    [ExportMetadata("Description", "Tool to document Power Automate Flows in Visio")]
    // Please specify the base64 content of a 32x32 pixels image
    [ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANSSURBVEhL7ZVLKK1RFMe3Z0yUEgMcA2+S1/EohIkixQApBkwk6owwoJzJSZmYmZgYyKMUUiQDeUQZeCTPyEB5hBKSN/d3zt6Xe30fn3QNbvkP9llr7f2t/97rdZyen5/Fd8JZ/X4bfggM8akkLy0tnZ+fOzv/dZunpycvL6+EhASlvwMDgoeHh5GRkfv7ex8fHy3B6empq6trfn4+q7JqYEAwOjp6c3OTl5fn4eGhTH+ALQ6wxQFl0uCjHHDBw8PDrKysF+82m00KEtizs7M5c3JyokwafEQwMDBA6A8ODpQuxMXFhZJ+Y39/H+Pg4KDSNfiIICAgICgoaG9vT+l6YNdkMvn7+ytdAwMCMpmbm6t0Ie7u7ra2toiJ0oUg+mTxiwQxMTF8PD09rXQhqqurZ2ZmVldXlS4Eu4+Pj7GxsUrXwKCKdnZ2FhcXvb2909PTPT09ldUBSgiys7Oz+Pj40NBQZdXAuNGI8vr6OpGB5qUVCB2uw8PDIyMjyZM06uJTnQy47/HxMX6lCpOvr69uc7zBZwm+jFeC3d3d+fl5Jwf8/PzS0tLc3NywHx0dDQ0NXV5ehoWFUTPSKEfI9vZ2cHBwQUEBD6K0JicnpTcmR0lJid0pwCTR2dmJmpSUNDw8HBgYGB0dTX9eXV0lJyeTzNbWVnb5jJO3t7c0MGdWVlbq6ura2towdnR0OPzZERUV5XBpx1uCnJwc5KamJuSGhgYy3NzcjEW2G/WO3N7ejlxTU4M8Ozvb19eHYLFYurq6oAQEA4uEfh/Ixpmbm+OajY2Nvb29tbW1lKkkHh8fZ93Y2EhJSamvr+etqDTH1NRUaWlpVVXV8vIyFgl9Ahno6+trVsK9trZGJpj+skwpUFaSxFO4RGVlJWpERER5efnExAQni4uL+QQj0CcgpawMGd47NjbGEGUsU6a8gxFLCbAbEhLCLEHY3NxkhJCqzMxMtuLi4ujthYUFu6P3CCgn1oqKCgh4ODIuCKiLi4u7u3thYSEWSgBKBLPZzNBtaWlBxjVdyUNf/+lkKkB/fz89mZiYWFZWlpGR0d3djZGnoPJ2Opa+lUbazWq14oJjBIpxgpFaKCoqSk1NJVY9PT12jw58e6Pph+gf4ofAEP87gRC/AD2r9paj2iJuAAAAAElFTkSuQmCC")]
    // Please specify the base64 content of a 80x80 pixels image
    [ExportMetadata("BigImageBase64", "iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAIAAAABc2X6AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAApwSURBVHhe7Zp3TFTNFsCv7RN7L8/yBHtBokZjF7soSuxCbBjUaNTELgghUWNNUGPUWGJBEzUaRWMHTVQssSCCgr0idsWGvbxf7hw3l2Xhw91L8rLs7w+YOTP37j0zZ065u/l+//6t5SXyy/88g0thZ8elsLPjUtjZcSns7LgUdnbynMK5Xjw8fPjw2LFjycnJ6enpP3/+FKkt8uXLV6hQocqVK7du3bpdu3Zubm4yYCq5qPCpU6fWrl1brVq13r17N2/eHAVQScZswZP8+vXr6dOnLFBMTEydOnWmTJlSqlQpGTaJXFH4/fv3ISEh7u7ukyZNevDgwd69e+/evStj/0aRIkXat2/v6+t7+/btxYsXDx06lLaMmQIKm0tqauqAAQOSkpJQctSoUeHh4Xfu3GHrZPjf+Pz58759+9Bz9erV3759mzt37rJly2TMDExW+NWrVwMHDnz27Bm7GhQU9Pr1axn4e6KiooYPH56WlrZmzZoVK1aI1GHMVPjHjx88Ivu5Y8eO0NDQnO9qVmDVLB8HJDg4+OTJkyJ1DDMVRs/IyMj4+PiJEyc6rq3i+vXrnAvsHM2xcJE6gGkKs73+/v48U0BAwIcPH0RqC0x9/fr10skB69at44Bg4Zs3bxaRA5iWeJw9e5bgSSjy9vYuXry4SG0RFxfHvkknBwQGBu7atatXr17R0dEicgDTFD569CjxFgfLPovIJMhG6tat+/jx40qVKr1580ak9mKawo8ePSLH4IFMTxUAqzlx4oSXl1diYqKI7MWExOPFixekRBhqs2bNeKAWLVoUKFBgxowZ9erVkxkZadq0aefOnSMiIqSfNTzb8uXLExISPn78mJKSUrp06a9fv+K9cIoyww70k2wyxGFyLOlkokmTJlOnTpVOtrx8+XLMmDHSMQkTdvj58+eod+/evVq1alEqeHh4kDOz52y1zMhIzneY2BYWFsY9CUvYUbFixb5//+7n5zd58mSZYQdKb8cZNmwYkYmUUPpZk/MdthAbG0u+RZA/fvy4iOzFNKdVpUoVtrpEiRKUgSIyj9OnT3fo0OHatWuenp4ishfTqiWCJJGjQoUKb9++JcEUqS2IXtSAOB5VLeLh+vXrl03opooeOXIkWceIESO2bdsmUrtRG+04+E9yLP4OGjToy5cvIrVFcnIyVo2GipIlS7KBMmaL7du3b9myhQp55cqVInIAM720ygFJtmbPni2irOHA44GAhohsQXhnHVnBwYMHc1hE6gBmKkwizfZirmyFKUUst+rbty/+ecmSJeRwInUMk+MwG8JWvHv3Dqc6bdo0R/aE5Jxb4Rd2794dHh4uUocxWWG4efMm+5yamnrp0iWeGM3JN2UsBxB7L168OG7cOJTEkqmrOCBmFZuQK++0MMWZM2f6+PgMGTKEEMrBpmDEG1MGZPMeD2/MkeZ5GjduTDzH/82bN69ly5aUkzLDDHJFYeC21HR79uxp06YNlV3NmjVRVW2UzMhEfh2W5sKFC6wRM0NCQqpWrSrDJpFbCiu4OYZN5Xj37t2CBQuKNGtYETc3t1atWvn6+pYtW1akppK7Cv8fkue+anEp7OzkOYVz0WnhcskK1Ws3KoSKFSv+888/aih7KPepNPlbunRpriKAy4ApoLBNSIzr169PGKxWrVr16tX/+4caNWo0atTIz89v0aJFDx8+lNkZoUIk8ahUqZIxzSDedOvWbceOHVmlTcgJv127drWsC5dTb3KrzC+6ExMTqcDhP3+onJGtW7fK1IxkqTCJDgusPljRuXNnKtIuXbqQMCkJT0YCqNIjC2xpgwYNGO3UqROp0ty5c9u1a6fmK/z9/TNXSKhEHqomoCcFMEkL1S95C5KOHTtaXYI+anJWnDt3TqZm5C8UjoqKUkMUgMZvq0ePHm3cNDYE4bBhwyxCcsb+/furyYoNGzaoIQVLZvxOlGtlQH+PV7RoUYTR0dEi0pk1a5aabJPChQtTwMjUjNijMIwdO1ak+oZYajeUrF27NkKrau7AgQNqsgL1ZECHWlIGdM6cOSMDOspeFi9eLH0d0lWExYsXL2WLJk2ayLxM2Omlu3fvLi3dC/A0qk198+zZMxoWs1dY5Yn4JGlpGsY8f/586WhaiRIlmjVrJh3d83369AkhiEgvM65evUrizV/8RWbi4+NlaibsVBh/Ji0d0v0nT57QKFKkiPrNAr5NDSmoGaWl4+7uLi1NO3To0KtXr6SjafhFq193cILS0tLGjx8vfU2jy6fg+XFaIsoxdiqM82SBpaNpeBR0Vm3MDMfWsGFD1QXO4ZIlS6SjaVQREyZMkI6msTrS0mGlXr9+TQjAaHv06DF9+nS8oFVkunXrFp9Yp04d7Oj8+fNz5syhnAwICOBgHzlyhCGZZxNl2ZnJ/gx//PjRqvqJiIiQMR0eeufOnZGRkfgwgoRM0gMyQpmkY/W+npjHakpHB22xeaNfXLt2LXKKKuXDrfD09Lx8+bJMzYSdCpMVWBleWFiYjOlQFcqAAfVrCJmhw2nEhmVYBxdIJOMQ4vxFpAs3btwo1/z+bfluibjYr1+/qVOn+vj4GK0Av5WVzvYrzHGVAR2KdRnTSU5ObtmyJZtgVcGz/AcPHpRJ+rtL8hMZ02nevLnaTGzEuKakFnyouqpnz55IMByWVUmAUKdmKnDUVqFbYafC6enpVn54wYIFMpYRnh63VKZMGZmH28if3xKHM++w8VsYLy8vkepYvmdBk7i4uKSkJNVVEMyNvhCjsJl72Om0CD9oIh0d0k9pZYQPZkOWLl0qfT3STJs27f3797RR3urEkslKS9PKly8vLR3Lj72wXkKX0S8CPkVFbAW6YdXSMWCnwgQGo8JopYJnTEwMXodNI9FRQwp0Zo50NI1QafHqVhHOWGDYLBuI24R93MHt27dF9Aer+RwBaRmwU+H79++zhNLRtFq1aqnnTkhI4PRy9vDGakhRrFgxqyNg+c7N29tbNRTGpyQWSEtHxfYrV64EBwfv37/fyjRA5TwWbBqdnQrHxsZKS4e4qsKyioHsknE/gVBM+SUdHct5o/BS2bIiNTVVNbCgx48fqzawgrhAGpwm/nJ/YyIAWI3xtzL4VEoO6RjRT7INsnFaPL0xxcEbM1kN4YGRoEBKSoqSKMgN1GQF5mB0oaGhoTKgR1clfPDggVEljr2SU5Mq0z18+LCSKBYuXKhmKqZMmSIDGfkLhTEkdF61apXx1LGKVPlyjV5Ft23bFjkeZdeuXTdu3MCdEqKNWQrLQbYoF+jwWX369FGjKElVjGEb6xPuafzWJjAwECGBatOmTfhqnBPPZjzA3M0Sw6z4C4WNcCYpj6lXiStywR8wraCgIKsorUBtMgTOuUw1wEpRPKsaA3O1XI474LxYvQDAqole5LBqjhH85cqVKzM/lYUsX/EgpxbhSun/gYcmqLK6Nl2oBR6RKo9AoupS1o5Di7mWK1dOZtiCVcY7sGnkz9S0Hh4e3bp1swpOFthzPgJfzRJjFyQwJBtgdbatcL2Id3ZcCjs7LoWdHZfCzo5LYWfHpbCz41LY2XEp7OzkMYU17X8u2MCzBPAX9AAAAABJRU5ErkJggg==")]
    [ExportMetadata("BackgroundColor", "Lavender")]
    [ExportMetadata("PrimaryFontColor", "Black")]
    [ExportMetadata("SecondaryFontColor", "Gray")]
    public class FlowToVisioPlugin : PluginBase, IPayPalPlugin


    {
        public string DonationDescription => "Flow to Visio Fans";

        public string EmailAccount => "carl.cookson@gmail.com";

        public override IXrmToolBoxPluginControl GetControl()
        {
            return new FlowToVisioControl();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FlowToVisioPlugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                else
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
            }

            return loadAssembly;
        }
    }
}