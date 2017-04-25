using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using HSCore;
using HSCore.Model;

namespace HSWebApi.Controllers
{
    public class StatusController : ApiController
    {
        // GET api/cards
        public string Get()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(NetDecks.IsDownloading ? "Download in progress..." : "Download complete.");
            sb.AppendLine();
            sb.AppendLine("Decks per source:");
            foreach (SourceEnum source in Enum.GetValues(typeof(SourceEnum)))
            {
                sb.AppendLine($"{Enums.GetEnumDescription(source)} : {NetDecks.Decks.Count(x => x.Source == source)}");
            }
            sb.AppendLine($"Total decks: {NetDecks.Decks.Count}");
            sb.AppendLine();
            sb.AppendLine("Pack values");
            foreach (Pack pack in (from SetEnum sType in Enum.GetValues(typeof(SetEnum)) select new Pack(sType)).Where(x => x.CanBuy).OrderByDescending(x=>x.ValueW).ToList())
            {
                sb.AppendLine($"{Enums.GetEnumDescription(pack.Set)} : {pack.ValueW} : {pack.ProbabilityW}");
            }

            return sb.ToString();
        }

        public string Get(string id)
        {
            string logPath = System.Web.Hosting.HostingEnvironment.MapPath("~/HSC.log");
            return logPath==null || !File.Exists(logPath) ? "No Log File" : File.ReadAllText(logPath);
        }
    }
}
