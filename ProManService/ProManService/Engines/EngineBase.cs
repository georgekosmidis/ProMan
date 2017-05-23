using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProManService.Engines
{
    public abstract class EngineBase
    {
        public EngineBase()
        {

        }
        public int GetCodeBytes(string s, string regexp)
        {

            Regex rgx = new Regex("( |\t)+");
            while (rgx.IsMatch(s))
                s = rgx.Replace(s, "");

            rgx = new Regex("([\n|\r|\n\r|\r\n].{1,2}[\n|\r|\n\r|\r\n])");
            while (rgx.IsMatch(s))
                s = rgx.Replace(s, "");

            var i = 0;
            rgx = new Regex("([\n|\r|\n\r|\r\n]{2,})");
            while (++i < 10)
                s = rgx.Replace(s, Environment.NewLine);//

            rgx = new Regex(regexp);
            s = rgx.Replace(s, "");

            rgx = new Regex("([\n|\r|\n\r|\r\n])");
            while (rgx.IsMatch(s))
                s = rgx.Replace(s, "");//Environment.NewLine

            return s.Trim().Length;//.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Count();
        }
    }
}
